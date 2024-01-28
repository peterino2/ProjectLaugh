using System.Collections;
using System.Collections.Generic;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjectArchitecture;
using ScriptableObjectArchitecture.Examples;


public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem gDialogueSystem;

    public DialogueChoiceViewer viewer;

    public delegate void BasicEvent();
    public BasicEvent dialogueFinished;

    public static DialogueSystem Get()
    {
        return gDialogueSystem;
    }

    public FloatReference playerHealth;
    public GameEvent onPlayerDamagedEvent;
    public bool inDialogue;
    public TextMeshProUGUI displayText; 
    public TextMeshProUGUI speakerName; 
    public Canvas canvasRoot;

    public DialogueSession endSequenceDS;
    
    private DialogueSession activeDialogue;

    private DialogueNode activeNode;
    private int dialogueIndex = 0;
    
    public Image portraitImage;
    public TextMeshProUGUI portaitDisplayText; // the text used when a portrait is displayed

    private float timeTilNextChar = 0.0f;

    public bool isInSpecialEvent = false;

    [SerializeField]
    private AudioClipGameEvent _onSpeechAudioEvent = default(AudioClipGameEvent);


    private void Awake()
    {
        gDialogueSystem = this;
    }

    public void startDialogue(DialogueSession session)
    {
        inDialogue = true;
        displayTextIndex = 0;
        if (session.NodesByName.Count != session.DialogueLines.Count)
        {
            Debug.LogWarning("Had to force a rebuild of the node map? number of nodes in map did not equal number of dialogue lines... expected " 
                             + session.DialogueLines.Count + " got " 
                             + session.NodesByName.Count );
            
            session.BuildRefmap();
        }
        
        Debug.Log("Session started " + session.ToString());
        dialogueIndex = 0;
        
        show();
        activeDialogue = session;
        setDialogueNode(dialogueIndex);
    }
    
    public List<SpeakerEntry> speakers;
    public Dictionary<string, SpeakerEntry> speakersById = new Dictionary<string, SpeakerEntry>();

    private SpeakerEntry activeSpeaker;

    public void setDialogueNode(int nodeIndex)
    {
        displayTextIndex = 0;
        dialogueIndex = nodeIndex;
        activeNode = activeDialogue.DialogueLines[nodeIndex];
        displayText.enabled = false;
        activeSpeaker = speakersById[activeNode.speaker];
        speakerName.text = activeSpeaker.DisplayName;
        portraitImage.sprite = activeSpeaker.DisplayImage;
        portaitDisplayText.enabled = true;
        portaitDisplayText.text = "";
        choicesCanvas.SetActive(false);
        
        if (activeNode.choicesText.Count > 0)
        {
            showChoices();
        }

        if (activeNode.isMovement)
        {
            if (activeNode.movementDescription == "OpenAttributes")
            {
                HandleOpenAttributes();
            }
            else
            {
                SceneSystem.Get().ExecuteActionSequence(activeNode.movementDescription);
            }
        }
    }

    public GameObject choicesCanvas;

    public void showChoices()
    {
        choicesCanvas.SetActive(true);
        viewer.activeNode = activeNode;
        viewer.updateNode();
    }

    public void sendChoice(int choiceID)
    {
        if (activeNode.choicesText.Count == 0)
        {
            return;
        }

        var choice = activeNode.choicesText[choiceID];
        var destination = choice.nodeRef;
        
        if (destination == "noref")
        {
            return;
        }

        Debug.Log(choiceID);
        Debug.Log(destination);
        Debug.Log(activeNode.choicesText[choiceID].nodeRef);
        Debug.Log(activeNode.choicesText[choiceID].choiceString);
        if (!activeDialogue.NodesByName.ContainsKey(destination))
        {
            activeDialogue.BuildRefmap();
        }

        setDialogueNode(activeDialogue.NodesByName[destination].nodeIndex);
    }

    private float debounce = 0.2f;
    
    public void forward(bool force=false)
    { 
        inDialogue = true;
        if (isInSpecialEvent)
        {
            return;
        }
        
        debounce = 0.2f;
        if (displayTextIndex >= activeNode.displayText.Length || force)
        {
            if (force)
            {
                Debug.Log("Forwarding with force parameter");
            }
            if (activeNode.choicesText.Count > 0 && !force)
            {
                return;
            }
            choicesCanvas.SetActive(false);
            dialogueIndex += 1;
            displayTextIndex = 0;
            if (dialogueIndex >= activeDialogue.DialogueLines.Count || activeNode.endDialogueAfter)
            {
                hide();
                inDialogue = false;
                if (dialogueFinished != null)
                {
                    dialogueFinished();
                }
                return;
            }
            _onSpeechAudioEvent.Raise(); // Play Boops
            setDialogueNode(dialogueIndex);
        }
        else
        {
            displayTextIndex = activeNode.displayText.Length;
            portaitDisplayText.text = activeNode.displayText;
        }
    }

    public void show()
    {
        canvasRoot.enabled = true;
    }

    public void hide()
    {
        canvasRoot.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var speaker in speakers)
        {
            speakersById[speaker.SpeakerId] = speaker;
        }
        hide();
    }

    private int displayTextIndex = 0;
    

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && debounce < 0 && activeNode != null)
        {
            forward();
        }
        else
        {
            debounce -= Time.deltaTime;
        }
        
        if (activeNode != null && displayTextIndex < activeNode.displayText.Length)
        {
            timeTilNextChar -= Time.deltaTime;
            while (timeTilNextChar < 0 && displayTextIndex < activeNode.displayText.Length || (displayTextIndex < activeNode.displayText.Length && activeNode.displayText[displayTextIndex] == ' ' ))
            {
                timeTilNextChar += Mathf.Clamp(1.0f / activeNode.characterRate, 0.01f, 1.0f);
                portaitDisplayText.text += activeNode.displayText[displayTextIndex];
                displayTextIndex += 1;
            }
        }
    }

    public void TestChoices()
    {
        viewer.testFunction();
    }

    public void moveSelectorUp()
    {
        
    }

    public void moveSelectorDown()
    {
        
    }

    /// ================================================================= SPECIAL EVENTS ==============================

    // these are triggered as soon as the node starts
    public void handleSpecialEvent(string eventString)
    {
        isInSpecialEvent = true;
        Debug.Log("HANDLING SPECIAL EVENT " + eventString);
        if (eventString == "BanditSpawnAndAttack")
        {
            var monster = FindObjectOfType<MonsterController>();
            monster.attackOne();
            playerHealth.Value -= 0.5f;
            onPlayerDamagedEvent.Raise();
        }
    }

    public void EndSequence()
    {
        Debug.Log("End Sequence Started...");
        // fade to black,
        // start the DS_EndSequence
    }

    public void HandleOpenAttributes()
    {
        isInSpecialEvent = true;
        GagAttributeAllocator.Get().OpenAllocator();
        hide();
    }

    public void HandleAttributesAllocated(DialogueSession sessionToStart)
    {
        isInSpecialEvent = false;
        StartCoroutine(DelayThenStartSession(sessionToStart));
    }

    public DialogueSession postAttributesAllocatedDialogue;
    IEnumerator DelayThenStartSession(DialogueSession sessionToStart)
    {
        yield return new WaitForSeconds(1.0f);
        startDialogue(sessionToStart);
    }
}

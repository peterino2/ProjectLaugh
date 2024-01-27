using System;
using Dialogue;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Profiling;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem gDialogueSystem;

    public DialogueChoiceViewer viewer;

    public static DialogueSystem Get()
    {
        return gDialogueSystem;
    }
    
    public TextMeshProUGUI displayText; 
    public TextMeshProUGUI speakerName; 
    public Canvas canvasRoot;
    
    private DialogueSession activeDialogue;
    
    public SpeakerDB speakerDatabase;

    private DialogueNode activeNode;
    private int dialogueIndex = 0;
    
    public Image portraitImage;
    public TextMeshProUGUI portaitDisplayText; // the text used when a portrait is displayed

    private float timeTilNextChar = 0.0f;

    public bool isInSpecialEvent = false;

    private void Awake()
    {
        gDialogueSystem = this;
    }

    public void startDialogue(DialogueSession session)
    {
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

    private SpeakerEntry activeSpeaker;

    public void setDialogueNode(int nodeIndex)
    {
        displayTextIndex = 0;
        dialogueIndex = nodeIndex;
        activeNode = activeDialogue.DialogueLines[nodeIndex];
        displayText.enabled = false;
        activeSpeaker = speakerDatabase.speakersById[activeNode.speaker];
        speakerName.text = activeSpeaker.DisplayName;
        portraitImage.sprite = activeSpeaker.DisplayImage;
        portaitDisplayText.enabled = true;
        portaitDisplayText.text = "";
        choicesCanvas.SetActive(false);
        
        if (activeNode.choicesText.Count > 0)
        {
            showChoices();
        }

        if (activeNode.hasSpecialEvent)
        {
            handleSpecialEvent(activeNode.specialEvent);
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

        setDialogueNode(activeDialogue.NodesByName[destination].nodeIndex);
    }

    private float debounce = 0.2f;
    
    public void forward()
    {
        if (isInSpecialEvent)
        {
            return;
        }
        
        debounce = 0.2f;
        if (displayTextIndex >= activeNode.displayText.Length)
        {
            if (activeNode.choicesText.Count > 0)
            {
                return;
            }
            choicesCanvas.SetActive(false);
            dialogueIndex += 1;
            displayTextIndex = 0;
            if (dialogueIndex >= activeDialogue.DialogueLines.Count || activeNode.endDialogueAfter)
            {
                hide();
                return;
            }
            
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
        
    }
}

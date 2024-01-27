using System;
using Dialogue;
using TMPro;
using UnityEditor.Profiling;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem gDialogueSystem;

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
    private float speechSpeed = 0.1f;

    private void Awake()
    {
        gDialogueSystem = this;
    }

    public void startDialogue(DialogueSession session)
    {
        if (session.NodesByName.Count != session.DialogueLines.Count)
        {
            Debug.LogWarning("Had to force a rebuild of the node map? number of nodes in map did not equal number of dialogue lines... expected " 
                             + session.DialogueLines.Count + " got " 
                             + session.NodesByName.Count );
            
            session.BuildRefmap();
        }
        Debug.Log("Session started " + session.ToString());
        
        show();
        activeDialogue = session;
        setDialogueNode(dialogueIndex);
    }

    private SpeakerEntry activeSpeaker;

    public void setDialogueNode(int nodeIndex)
    {
        dialogueIndex = nodeIndex;
        activeNode = activeDialogue.DialogueLines[nodeIndex];
        displayText.enabled = false;
        activeSpeaker = speakerDatabase.speakersById[activeNode.speaker];
        speakerName.text = activeSpeaker.DisplayName;
        portraitImage.sprite = activeSpeaker.DisplayImage;
        portaitDisplayText.enabled = true;
        portaitDisplayText.text = "";
    }

    public void forward()
    {
        if (displayTextIndex >= activeNode.displayText.Length)
        {
            dialogueIndex += 1;
            displayTextIndex = 0;
            if (dialogueIndex >= activeDialogue.DialogueLines.Count)
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
        if (activeNode != null && displayTextIndex < activeNode.displayText.Length)
        {
            timeTilNextChar -= Time.deltaTime;
            while (timeTilNextChar < 0 && displayTextIndex < activeNode.displayText.Length || (displayTextIndex < activeNode.displayText.Length && activeNode.displayText[displayTextIndex] == ' ' ))
            {
                timeTilNextChar += 1.0f / activeNode.characterRate;
                portaitDisplayText.text += activeNode.displayText[displayTextIndex];
                displayTextIndex += 1;
            }
        }
    }
}

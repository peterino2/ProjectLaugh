using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem gDialogueSystem;

    public static DialogueSystem Get()
    {
        return gDialogueSystem;
    }
    
    public TextMeshProUGUI DisplayText; 
    public TextMeshProUGUI SpeakerName; 
    public Canvas CanvasRoot;
    
    [NonSerialized]
    public DialogueSession ActiveDialogue;
    
    public SpeakerDB SpeakerDatabase;

    private DialogueNode ActiveNode;
    private int dialogueIndex = 0;

    private void Awake()
    {
        gDialogueSystem = this;
    }

    public void startDialogue(DialogueSession session)
    {
        if (session.NodesByName.Count != session.DialogueLines.Count)
        {
            Debug.LogWarning("Had to force a rebuild of the nodemap? number of nodes in map did not equal number of dialogue lines... expected " 
                             + session.DialogueLines.Count + " got " 
                             + session.NodesByName.Count );
            
            session.BuildRefmap();
        }
        Debug.Log("Session started " + session.ToString());
        
        show();
        ActiveDialogue = session;
        ActiveNode = ActiveDialogue.DialogueLines[0];
        dialogueIndex = 0;
        SpeakerName.text = ActiveNode.speaker;
        DisplayText.text = ActiveNode.displayText;
    }

    public void forward()
    {
        dialogueIndex += 1;
        if (dialogueIndex > ActiveDialogue.DialogueLines.Count)
        {
            hide();
            return;
        }
        ActiveNode = ActiveDialogue.DialogueLines[dialogueIndex];
        SpeakerName.text = ActiveNode.speaker;
        DisplayText.text = ActiveNode.displayText;
    }

    public void show()
    {
        CanvasRoot.enabled = true;
    }

    public void hide()
    {
        CanvasRoot.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

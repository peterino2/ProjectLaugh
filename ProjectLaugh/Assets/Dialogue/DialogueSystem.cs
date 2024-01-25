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
    public Canvas CanvasRoot;
    public DialogueSession ActiveDialogue;

    private void Awake()
    {
        gDialogueSystem = this;
    }

    public void startDialogue(DialogueSession session)
    {
        CanvasRoot.enabled = true;
        Debug.Log("Session started " + session.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        CanvasRoot.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

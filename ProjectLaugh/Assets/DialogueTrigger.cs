using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    public DialogueSession startDialogue;
    public bool completed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed)
        {
            return;
        }

        completed = true;
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            DialogueSystem.Get().startDialogue(startDialogue);
        }
    }
}

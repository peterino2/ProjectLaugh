using Dialogue;
using Dialogue.Gags;
using UnityEngine;

namespace DefaultNamespace
{
    public class IntBasedInteractable : Interactable
    {
        public DialogueSession PassedDialogue;
        public DialogueSession FailedDialogue;
        public int requirement = 1;
        public bool completed = false;

        public override void OnInteract(GameObject Interactor)
        {
            if (completed)
                return;
            completed = true;
            
            if (GagPlayerAttributes.Get().intelligence >= requirement)
            { 
                DialogueSystem.Get().startDialogue(PassedDialogue);
            }
            else
            {
                DialogueSystem.Get().startDialogue(FailedDialogue);
            }

        }
    }
}
using Dialogue;
using Dialogue.Gags;
using UnityEngine;

namespace DefaultNamespace
{
    public class DexBasedInteractable : Interactable
    {
        public DialogueSession PassedDialogue;
        public DialogueSession FailedDialogue;
        public int requirement = 1;

        public override void OnInteract(GameObject Interactor)
        {
            if (GagPlayerAttributes.Get().strength > requirement)
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
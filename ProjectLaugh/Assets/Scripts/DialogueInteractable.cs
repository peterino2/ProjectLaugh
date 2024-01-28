using Dialogue;
using UnityEngine;

namespace DefaultNamespace
{
    public class DialogueInteractable : Interactable
    {
        public DialogueSession SessionToStart;
        
        public override void OnInteract(GameObject Interactor)
        {
            DialogueSystem.Get().startDialogue(SessionToStart);
        }
    }
}
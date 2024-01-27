using UnityEngine;

namespace DefaultNamespace
{
    public class Interactable : MonoBehaviour
    {
        public virtual void OnInteract(GameObject Interactor)
        {
            DialogueSystem.Get().TestChoices();
        }
    }
}
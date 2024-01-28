using UnityEngine;

namespace DefaultNamespace
{
    public class Interactable : MonoBehaviour
    {
        public int type;
        public virtual void OnInteract(GameObject Interactor)
        {
            switch (type) {
                case 0:
                    print("interacting type 0");
                    break;
                case 1:
                    print("interacting type 1");
                    break;
                case 2:
                    print("interacting type 2");
                    break;
                default:
                    DialogueSystem.Get().TestChoices();
                    break;

            }
            
        }
    }
}
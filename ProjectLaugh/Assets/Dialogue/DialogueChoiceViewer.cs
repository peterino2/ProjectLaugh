using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueChoiceViewer : MonoBehaviour
    {
        public Canvas canvasRoot;
        public GameObject displayPrefab;
        public GameObject textInsertPoint;

        public DialogueNode activeNode;

        public DialogueSession TestDialogue;

        private List<GameObject> insertedText = new List<GameObject>();

        public float selectorHeightOffset = -16.2f;

        public void updateNode()
        {
            foreach (var x in insertedText)
            {
                Destroy(x);
            }

            int choiceId = 0;
            
            foreach (var x in activeNode.choicesText)
            {
                var newText = Instantiate(displayPrefab);
                newText.transform.SetParent(textInsertPoint.transform);
                var b = newText.GetComponent<DialogueButton>();
                var button = b.button;
                
                var gui = b.DisplayText;
                gui.text = x.choiceString;
                insertedText.Add(newText);
                
                var i = choiceId;
                button.onClick.AddListener(() => { buttonClicked(i); });
                choiceId += 1;
            }
        }

        public void buttonClicked(int choiceId)
        {
            Debug.Log("activating choice " + choiceId);
            DialogueSystem.Get().sendChoice(choiceId);
        }

        public void testFunction()
        {
            DialogueSystem.Get().startDialogue(TestDialogue);
        }

    }
}
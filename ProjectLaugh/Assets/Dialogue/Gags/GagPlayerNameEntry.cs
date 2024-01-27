using System;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.Gags
{
    public class GagPlayerNameEntry : MonoBehaviour
    {
        public Canvas gagCanvas;
        public Button acceptButton;
        public TextMeshProUGUI inputField;
        
        static GagPlayerNameEntry gGagPlayerNameEntry;
        public void Awake() { gGagPlayerNameEntry = this; }
        public static GagPlayerNameEntry Get() { return gGagPlayerNameEntry; }


        public void Start()
        {
            gagCanvas.gameObject.SetActive(false);
        }

        public void StartGag()
        {
            Debug.Log("Gag started");
            gagCanvas.gameObject.SetActive(true);
            DialogueSystem.Get().isInSpecialEvent = true;
            acceptButton.onClick.AddListener(OnAcceptButton);
        }

        public void CloseGag()
        {
            gagCanvas.gameObject.SetActive(false);
            DialogueSystem.Get().isInSpecialEvent = false;
        }

        public void OnAcceptButton()
        {
            GameStateManager.Get().stringValues["playername"] = inputField.text;
            CloseGag();
        }

        public void Update()
        {
            
        }
    }
}
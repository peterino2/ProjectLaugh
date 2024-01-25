using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.UI;

public class Gym_dialogue_test : MonoBehaviour
{
    public DialogueSession SessionRef;

    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(onclick);
    }

    void onclick()
    {
        DialogueSystem.Get().startDialogue(SessionRef);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

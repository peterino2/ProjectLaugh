using System;
using System.Collections;
using Dialogue;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class StoryStart : MonoBehaviour
{
    public Image BlackScreen;
    public DialogueSession gameStartDialogue;
    
    // Start is called before the first frame update
    void Start()
    {
        BlackScreen.color = Color.black;
        StartCoroutine(StartStory());
    }

    IEnumerator StartStory()
    {
        DialogueSystem.Get().inDialogue = true;
        yield return new WaitForSeconds(1.0f);
        DialogueSystem.Get().dialogueFinished += finishDialogue;
        DialogueSystem.Get().startDialogue(gameStartDialogue);
    }

    private float fadeValue = 1.0f;
    
    IEnumerator fadeBack()
    {
        while (fadeValue > 0)
        {
            BlackScreen.color = new Color(0, 0, 0, Mathf.Max(fadeValue, 0.0f));
            fadeValue -= Time.deltaTime;
            Debug.Log(BlackScreen.color.ToString());
            yield return null;
        }
    }

    void finishDialogue()
    {
        Debug.Log("Finished dialogue");
        DialogueSystem.Get().dialogueFinished -= finishDialogue;
        StartCoroutine(fadeBack());
    }

    // Update is called once per frame
    void Update()
    {
    }
}

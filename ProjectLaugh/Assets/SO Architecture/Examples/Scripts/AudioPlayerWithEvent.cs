using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerWithEvent : MonoBehaviour
{
    [SerializeField]
    private AudioEvent _playAudioEvent = default(AudioEvent);

    public void playAudio(AudioSource source) {
        _playAudioEvent.Play(source);
    }
}

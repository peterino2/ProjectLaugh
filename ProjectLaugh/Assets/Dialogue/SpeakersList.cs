using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public class SpeakerEntry
    {
        [SerializeField] public string SpeakerId;
        [SerializeField] public string DisplayName;
        [SerializeField] public Sprite DisplayImage;
        [SerializeField] public List<AudioClip> SpeechBongs;
    }
    
    [CreateAssetMenu(fileName = "SpeakerDatabase", menuName = "SpeakersDB", order = 1)]
    public class SpeakersList : ScriptableObject
    {
        [SerializeField]
        public List<SpeakerEntry> speakers;

        public Dictionary<string, SpeakerEntry> speakersById = new Dictionary<string, SpeakerEntry>();

        public void OnValidate()
        {
            foreach (var speaker in speakers)
            {
                speakersById[speaker.SpeakerId] = speaker;
            }
        }
    }
}
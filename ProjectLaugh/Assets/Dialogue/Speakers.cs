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
        [SerializeField] Texture2D DisplayImage;
    }
    
    [CreateAssetMenu(fileName = "SpeakerDatabase", menuName = "SpeakerDB", order = 0)]
    public class SpeakerDB : ScriptableObject
    {
        [SerializeField]
        public List<SpeakerEntry> speakers;

        [SerializeField]
        public Dictionary<string, SpeakerEntry> Entries;
        private void OnValidate()
        {
        }
    }
}
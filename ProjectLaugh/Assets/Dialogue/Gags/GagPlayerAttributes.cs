using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Dialogue.Gags
{
    public class GagPlayerAttributes : MonoBehaviour
    {
        public static GagPlayerAttributes gGagPlayerAttributes;

        public float strength = 3;
        public float intelligence = 3;
        public float dexterity = 3;
        public float wisdom = 3;
        public float charisma = 3;
        public float constitution = 3;

        public float resiliency = 3;
        public float conservatism = 3;
        public float dopamine = 3;
        public float javascript = 3;
        public float hotdogs = 3;
        public float rapping = 3;
        public float poise = 3;
        public float tohitarmorclasszero = 3;
        public float darksoulsinvasion = 3;
        
        public void Awake()
        {
            gGagPlayerAttributes = this;
        }

        public static GagPlayerAttributes Get()
        {
            return gGagPlayerAttributes;
        }
    }
}
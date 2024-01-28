using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Dialogue.Gags
{
    public class GagPlayerAttributes : MonoBehaviour
    {
        static GagPlayerAttributes gGagPlayerAttributes;

        public float strength = 1;
        public float intelligence = 1;
        public float dexterity = 1;

        public Canvas playerAttributesCanvasRoot;
        
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
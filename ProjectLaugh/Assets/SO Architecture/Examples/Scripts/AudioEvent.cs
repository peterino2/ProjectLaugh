using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
    public abstract void Play(AudioSource source);
}

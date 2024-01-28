using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class DamagedPitchUpdate : MonoBehaviour
{
    public FloatReference playerHealth;

    public void UpdatePitch(AudioSource source) {
        Debug.Log(playerHealth.Value);
        source.pitch = 1-( (1-playerHealth.Value)/2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;

public class VideoEnd : MonoBehaviour
{

    public VideoPlayer vid;
    public GameEvent vidEnd = default(GameEvent);

    // Start is called before the first frame update
    void Start()
    {
        vid.loopPointReached += CheckOver;
    }

    void CheckOver(VideoPlayer vp) {
        Debug.Log("Video Ended");
        vidEnd.Raise();
        
        SceneManager.LoadScene("FirstScene", LoadSceneMode.Single);
        SceneManager.LoadScene("CombatGagSubScene", LoadSceneMode.Additive);
    }
}

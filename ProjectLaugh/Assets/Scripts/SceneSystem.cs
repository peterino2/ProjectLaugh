using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private static SceneSystem system = null;

    public static SceneSystem Get()
    {
        if(!system)
        {
            system = new SceneSystem();
        }

        return system;
    }

    private Dictionary<string, SceneComponent> m_sceneComponents = null;

    private SceneSystem()
    {
        
    }

    private void Awake()
    {
        if (system && this != system)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            system = this;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterSceneObjects();
    }

    private void RegisterSceneObjects()
    {
        SceneComponent[] sceneComponents = FindObjectsOfType<SceneComponent>();

        m_sceneComponents = new Dictionary<string, SceneComponent>();

        foreach(SceneComponent component in sceneComponents)
        {
            if(component.SceneObjectName == "")
            {
                Debug.LogError("SceneSystem: empty name not allowed");
                continue;
            }

            if(!m_sceneComponents.TryAdd(component.SceneObjectName, component))
            {
                Debug.LogError("SceneSystem: Failed to add scene component, " + component.SceneObjectName + " already exists");
            }
        }

        Debug.Log("SceneManager: " + m_sceneComponents.Count + " Scene Components Loaded");
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager manager = null;

    public Dictionary<string, float> floatValues = new Dictionary<string, float>();
    public Dictionary<string, string> stringValues = new Dictionary<string, string>();

    public static GameStateManager Get()
    {
        return manager;
    }

    private GameStateManager()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        manager = this;
    }

    private HashSet<string> m_tags = new HashSet<string>();

    public bool AddTag(string tag)
    {
        return m_tags.Add(tag);
    }

    public void AddTags(string[] tags)
    {
        foreach(string tag in tags)
        {
            m_tags.Add(tag);
        }
    }

    public bool RemoveTag(string tag)
    {
        return m_tags.Remove(tag);
    }

    public void RemoveTags(string[] tags)
    {
        foreach(string tag in tags)
        {
            m_tags.Remove(tag);
        }
    }

    public bool HasTag(string tag)
    {
        return m_tags.Contains(tag);
    }

    public bool HasAnyTag(string[] tags)
    {
        foreach(string tag in tags)
        {
            if(m_tags.Contains(tag))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAllTags(string[] tags)
    {
        foreach(string tag in tags)
        {
            if(!m_tags.Contains(tag))
            {
                return false;
            }
        }

        return true;
    }
}

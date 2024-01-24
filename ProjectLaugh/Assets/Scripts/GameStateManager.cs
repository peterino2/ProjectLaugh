using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager manager = null;

    public static GameStateManager Get()
    {
        if(!manager)
        {
            manager = new GameStateManager();
        }

        return manager;
    }

    private GameStateManager()
    {
        
    }
    private void Awake()
    {
        if (manager && this != manager)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            manager = this;
            m_tags = new HashSet<string>();
        }
    }

    private HashSet<string> m_tags;

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

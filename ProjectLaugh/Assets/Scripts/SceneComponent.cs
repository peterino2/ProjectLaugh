using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneComponent : MonoBehaviour
{
    [SerializeField]
    private string m_sceneObjectName = null;

    public string SceneObjectName { get { return m_sceneObjectName; } private set { m_sceneObjectName = value; } }

    [SerializeField]
    private List<string> m_sceneTags = null;

    public bool HasTag(string tag)
    {
        return m_sceneTags.Contains(tag);
    }
}

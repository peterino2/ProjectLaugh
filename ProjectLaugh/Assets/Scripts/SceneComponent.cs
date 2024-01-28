using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneComponent : MonoBehaviour
{
    public static Vector3 INVALID_POSITION = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    [SerializeField]
    private string m_sceneObjectName = null;

    public string SceneObjectName { get { return m_sceneObjectName; } private set { m_sceneObjectName = value; } }

    [SerializeField]
    private List<string> m_sceneTags = null;

    public Vector3 SavedPosition { get; set; }

    public bool HasTag(string tag)
    {
        return m_sceneTags.Contains(tag);
    }
}

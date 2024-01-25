using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public abstract class SceneActionBase
{
    public abstract bool IsInstant();

    public virtual bool Tick() { return true; }

    public abstract bool TryParse(string[] parameters);

    public static bool TryParseFloat(string toParse, out float value)
    {
        try
        {
            value = float.Parse(toParse);
            return true;
        }
        catch (System.Exception)
        {
            LogParseError(toParse, "float");
        }
        
        value = 0;
        return false;
    }

    public static bool TryParseVector(string toParse, out Vector3 value)
    {
        value = new Vector3();
        if(toParse.StartsWith("(") && toParse.EndsWith(")"))
        {
            string[] floats = toParse.Substring(1, toParse.Length - 2).Split(',');

            float x, y, z;

            if(!TryParseFloat(floats[0], out x))
            {
                return false;
            }

            if (!TryParseFloat(floats[1], out y))
            {
                return false;
            }

            if (!TryParseFloat(floats[2], out z))
            {
                return false;
            }

            value = new Vector3(x, y, z);
        }
        else
        {
            GameObject sceneObject = SceneSystem.Get().GetSceneObject(toParse);
            if(!sceneObject)
            {
                return false;
            }

            value = sceneObject.transform.position;
        }

        return true;
    }

    private static void LogParseError(string raw, string type)
    {
        Debug.LogError("SceneAction: Unable to parse \"" + raw + "\" as \"" + type + "\"");
    }
}

public class InvalidAction : SceneActionBase
{
    public override bool IsInstant()
    {
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        Debug.LogError("SceneAction: Invalid Action Created");
        return false;
    }
}

public class MoveTo : SceneActionBase
{
    protected GameObject m_objectToMove = null;
    protected Vector3 m_destination;
    protected float m_velocity;
    
    public MoveTo()
    {
    }

    public MoveTo(GameObject objectToMove, Vector3 destination, float velocity)
    {
        m_objectToMove = objectToMove;
        m_destination = destination;
        m_velocity = velocity;
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        if(!m_objectToMove)
        {
            return true;
        }

        Vector3 direction = m_destination - m_objectToMove.transform.position;
        
        m_objectToMove.transform.position += direction.normalized * m_velocity * Time.deltaTime;
        return m_objectToMove.transform.position == m_destination;
    }

    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 3)
        {
            return false;
        }

        GameObject objectToMove = SceneSystem.Get().GetSceneObject(parameters[0].Trim());

        if(!objectToMove)
        {
            return false;
        }

        Vector3 destination;
        if(!TryParseVector(parameters[1], out destination))
        {
            return false;
        }

        float velocity;
        if(!TryParseFloat(parameters[2], out velocity))
        {
            return false;
        }

        m_objectToMove = objectToMove;
        m_destination = destination;
        m_velocity = velocity;
        return true;
    }
}

public class SceneThread
{
    public delegate void ThreadCompleted();

    public event ThreadCompleted ThreadCompletedEvent;

    private List<SceneActionBase> m_executionLines;

    private int m_currentLine = 0;

    public SceneThread(SceneActionBase action)
    {
        m_executionLines = new List<SceneActionBase>();
        m_executionLines.Add(action);
        m_currentLine = 0;
    }

    public SceneThread(List<SceneActionBase> executionLines)
    {
        m_currentLine = 0;
        m_executionLines = executionLines;
    }

    public bool Tick()
    {
        if(m_currentLine >= m_executionLines.Count)
        {
            ThreadCompletedEvent?.Invoke();
            return true;
        }

        while(m_executionLines[m_currentLine].Tick())
        {
            ++m_currentLine;
            if(!m_executionLines[m_currentLine - 1].IsInstant())
            {
                break;
            }

            if(m_currentLine >= m_executionLines.Count)
            {
                break;
            }
        }

        if(m_currentLine >= m_executionLines.Count)
        {
            ThreadCompletedEvent?.Invoke();
            return true;
        }

        return false;
    }
}

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

    private Dictionary<string, GameObject> m_sceneComponents = null;

    private List<SceneThread> m_threads;

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

    private void Start()
    {
        
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
        m_threads = new List<SceneThread>();
    }

    private void Update()
    {
        List<SceneThread> toRemove = new List<SceneThread>();
        foreach(SceneThread thread in m_threads)
        {
            if(thread.Tick())
            {
                toRemove.Add(thread);
            }
        }

        foreach(SceneThread thread in toRemove)
        {
            m_threads.Remove(thread);
        }
    }

    private void RegisterSceneObjects()
    {
        SceneComponent[] sceneComponents = FindObjectsOfType<SceneComponent>();

        m_sceneComponents = new Dictionary<string, GameObject>();

        foreach(SceneComponent component in sceneComponents)
        {
            if(component.SceneObjectName == "")
            {
                Debug.LogError("SceneSystem: empty name not allowed");
                continue;
            }

            if(!m_sceneComponents.TryAdd(component.SceneObjectName, component.gameObject))
            {
                Debug.LogError("SceneSystem: Failed to add scene component, " + component.SceneObjectName + " already exists");
            }
        }

        Debug.Log("SceneManager: " + m_sceneComponents.Count + " Scene Components Loaded");
    }

    public GameObject GetSceneObject(string sceneObjectName)
    {
        if(!m_sceneComponents.ContainsKey(sceneObjectName))
        {
            Debug.Log("SceneManager: Unable to find object with name: " + sceneObjectName);
            return null;
        }

        return m_sceneComponents[sceneObjectName];
    }

    public bool ExecuteAction(string actionToParse, SceneThread.ThreadCompleted callback = null)
    {
        SceneActionBase action;
        
        if(ParseAction(actionToParse, out action))
        {
            SceneThread thread = new SceneThread(action);

            if (callback != null)
            {
                thread.ThreadCompletedEvent += callback;
            }

            m_threads.Add(thread);  //Maybe not thread safe but whatever
            
            return true;
        }

        return false;
    }

    public bool ParseAction(string actionToParse, out SceneActionBase action)
    {
        if(actionToParse.Length < 1)
        {
            action = new InvalidAction();
            return false;
        }

        List<string> parameters = new List<string>(actionToParse.Trim().Split(';'));

        string actionType = parameters[0];

        parameters.RemoveAt(0);

        switch(actionType)
        {
            case "MoveTo":
                action = new MoveTo();
                if(!action.TryParse(parameters.ToArray()))
                {
                    action = new InvalidAction();
                    return false;
                }
                break;
            default:
                Debug.LogError("SceneSystem: Unable to parse action \"" + actionToParse + "\"");
                action = new InvalidAction();
                return false;
        }

        return true;
    }
}

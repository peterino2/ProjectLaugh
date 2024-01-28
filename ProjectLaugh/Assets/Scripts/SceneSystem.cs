using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using Dialogue;
using Dialogue.Gags;
using TMPro;
public enum Axis
{
    X = 0,
    Y = 1,
    Z = 2
}

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

    //Expected format: (x,y,z) OR scene object name as a string
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
        else if(toParse.StartsWith("$"))
        {
            GameObject sceneObject = SceneSystem.Get().GetSceneObject(toParse.Substring(1));
            if (!sceneObject)
            {
                return false;
            }

            value = sceneObject.GetComponent<SceneComponent>().SavedPosition;
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

    public static bool TryParseBool(string toParse, out bool value)
    {
        try
        {
            value = bool.Parse(toParse);
            return true;
        }
        catch (System.Exception)
        {
            LogParseError(toParse, "bool");
        }

        value = false;
        return false;
    }

    public static bool TryParseObject(string toParse, out GameObject value)
    {
        value = SceneSystem.Get().GetSceneObject(toParse);

        return value != null;
    }

    public static bool TryParseAxis(string toParse, out Axis value)
    {
        value = Axis.X;
        if(toParse.Length < 1)
        {
            return false;
        }

        switch(toParse[0])
        {
            case 'X':
                value = Axis.X;
                break;
            case 'Y':
                value = Axis.Y;
                break;
            case 'Z':
                value = Axis.Z;
                break;
            default:
                return false;
        }

        return true;
    }

    //true = positive
    public bool[] GetSigns(Vector3 vec)
    {
        bool[] signs = new bool[3];
        
        for(int i = 0; i < 3; i++)
        {
            signs[i] = vec[i] >= 0.0f;
        }

        return signs;
    }

    private static void LogParseError(string raw, string type)
    {
        Debug.LogError("SceneAction: Unable to parse \"" + raw + "\" as \"" + type + "\"");
    }
}

public class BlankAction : SceneActionBase
{
    public override bool IsInstant()
    {
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        return true;
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
        return false;
    }
}

public class DestroyObject : SceneActionBase
{
    private GameObject m_object;

    public DestroyObject() { }

    public DestroyObject(GameObject obj)
    {
        m_object = obj;
    }

    public override bool IsInstant()
    {
        return true;
    }

    public override bool Tick()
    {
        if (m_object)
        {
            GameObject.Destroy(m_object);
            m_object = null;
        }
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        TryParseObject(parameters[0], out m_object);

        return true;
    }
}

public class StorePosition : SceneActionBase
{
    public override bool IsInstant()
    {
        return true;
    }

    public override bool Tick()
    {
        return true;
    }

    //TODO error checks
    public override bool TryParse(string[] parameters)
    {
        GameObject obj;
        TryParseObject(parameters[0], out obj);

        Vector3 positionToStore = obj.transform.position;

        if(parameters.Length > 1)
        {
            TryParseVector(parameters[1], out positionToStore);
        }

        obj.GetComponent<SceneComponent>().SavedPosition = positionToStore;

        return true;
    }
}

public class TeleportTo : SceneActionBase
{
    GameObject m_object;
    Vector3 m_position;

    public override bool IsInstant()
    {
        return true;
    }

    public override bool Tick()
    {
        m_object.transform.position = m_position;
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        TryParseObject(parameters[0], out m_object);
        TryParseVector(parameters[1], out m_position);

        return true;
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
        
        if(Vector3.SqrMagnitude(m_objectToMove.transform.position - m_destination) < 0.1f)
        {
            m_objectToMove.transform.position = m_destination;
            return true;
        }
        return false;
    }

    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 3)
        {
            return false;
        }

        if(!TryParseObject(parameters[0].Trim(), out m_objectToMove))
        {
            return false;
        }

        if(!TryParseVector(parameters[1].Trim(), out m_destination))
        {
            return false;
        }

        if(!TryParseFloat(parameters[2].Trim(), out m_velocity))
        {
            return false;
        }

        return true;
    }
}

public class SpawnDamageNumber : SceneActionBase
{
    Vector3 m_position;
    string text;

    public override bool IsInstant()
    {
        return true;
    }

    public override bool Tick()
    {
        SceneSystem.Get().SpawnDamageNumber(m_position, text);
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        TryParseVector(parameters[0], out m_position);

        text = parameters[1];

        return true;
    }
}

public class BanditSpawnAndAttack : SceneActionBase
{
    private float m_timeRemaining = 0.0f;

    public BanditSpawnAndAttack()
    {
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        DialogueSystem.Get().hide();
        DialogueSystem.Get().isInSpecialEvent = true;
        m_timeRemaining -= Time.deltaTime;
        Debug.Log(m_timeRemaining);
        // play bandit attack animation
        
        if (m_timeRemaining <= 0.0f)
        {
            DialogueSystem.Get().handleSpecialEvent("BanditSpawnAndAttack");
            DialogueSystem.Get().isInSpecialEvent = false;
            DialogueSystem.Get().show();
            DialogueSystem.Get().forward(true);
        }
        return m_timeRemaining <= 0.0f;
    }
    
    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 1)
        {
            return false;
        }

        return TryParseFloat(parameters[0].Trim(), out m_timeRemaining);
    }
}

public class BanditBeatdown : SceneActionBase
{
    private float m_timeRemaining = 0.0f;
    private MonsterController m_monster;
    private float m_cooldown;
    private float m_curCooldown;

    public BanditBeatdown()
    {
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        DialogueSystem.Get().isInSpecialEvent = true;
        DialogueSystem.Get().hide();
        m_timeRemaining -= Time.deltaTime;
        m_curCooldown -= Time.deltaTime;

        if(m_curCooldown <= 0.0f)
        {
            m_curCooldown = m_cooldown;
            m_monster.attackOne();
            SceneSystem.Get().ExecuteActionSequence("SpawnDamageNumber;Player;"+ (int)Random.Range(421, 999));
        }
        
        // Start beatdown here.
        bool rv = m_timeRemaining <= 0.0f;
        if (rv)
        {
            DialogueSystem.Get().isInSpecialEvent = false;
            DialogueSystem.Get().show();
            DialogueSystem.Get().forward(true);
            DialogueSystem.Get().EndSequence();
        }

        return rv;
    }
    
    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 1)
        {
            return false;
        }

        GameObject obj;
        TryParseObject(parameters[0].Trim(), out obj);

        m_monster = obj.GetComponent<MonsterController>();

        //var monster = FindObjectOfType<MonsterController>();
        // monster.attackOne();

        //SceneSystem.Get().ExecuteActionSequence("SpawnDamageNumber;Player;428");
        TryParseFloat(parameters[1].Trim(), out m_cooldown);
        m_curCooldown = m_cooldown;

        return TryParseFloat(parameters[2].Trim(), out m_timeRemaining);
    }
}

public class BanditSuckerPunch : SceneActionBase
{
    private float m_timeRemaining = 0.0f;
    private MonsterController m_monster;
    private bool m_attacked = false;

    public BanditSuckerPunch()
    {
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        DialogueSystem.Get().isInSpecialEvent = true;
        m_timeRemaining -= Time.deltaTime;
        Debug.Log( "sucker " + m_timeRemaining.ToString());

        if(!m_attacked)
        {
            SceneSystem.Get().ExecuteActionSequence("SpawnDamageNumber;Player;761");
            m_monster.attackOne();
            m_attacked = true;
        }
        
        // start playing bandit sucker punch animation here

        bool rv = m_timeRemaining <= 0.0f;
        if (rv)
        {
            DialogueSystem.Get().isInSpecialEvent = false;
            DialogueSystem.Get().forward(true);
        }

        return rv;
    }
    
    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 1)
        {
            return false;
        }
        GameObject obj;
        TryParseObject(parameters[0].Trim(), out obj);
        m_monster = obj.GetComponent<MonsterController>();

        return TryParseFloat(parameters[1].Trim(), out m_timeRemaining);
    }
}

public class Wait : SceneActionBase
{
    private float m_timeRemaining = 0.0f;

    public Wait() { }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        DialogueSystem.Get().hide();
        DialogueSystem.Get().isInSpecialEvent = true;
        m_timeRemaining -= Time.deltaTime;

        if (m_timeRemaining <= 0.0f)
        {
            DialogueSystem.Get().isInSpecialEvent = false;
            DialogueSystem.Get().show();
            DialogueSystem.Get().forward();
        }
        return m_timeRemaining <= 0.0f;
    }

    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 1)
        {
            return false;
        }

        return TryParseFloat(parameters[0].Trim(), out m_timeRemaining);
    }
}

public class SetActive : SceneActionBase
{
    private GameObject m_object = null;
    private bool m_status = false;

    public override bool IsInstant()
    {
        return true;
    }

    public override bool Tick()
    {
        if (m_object)
        {
            m_object.SetActive(m_status);
        }
        return true;
    }

    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 2)
        {
            return false;
        }

        if(!TryParseObject(parameters[0].Trim(), out m_object))
        {
            return false;
        }

        return TryParseBool(parameters[1].Trim(), out m_status);
    }
}

public class MoveArc : SceneActionBase
{
    private GameObject m_object = null;
    private Vector3 m_velocity;
    private Vector3 m_deceleration;

    private bool[] m_initialSigns;  //true for positive (min 0), false for negative (max 0)

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        if (!m_object)
        {
            return true;
        }

        m_object.transform.position += m_velocity;

        m_velocity -= m_deceleration;

        m_velocity.x = m_initialSigns[0] ? Mathf.Min(0, m_velocity.x) : Mathf.Max(0, m_velocity.x);
        m_velocity.y = m_initialSigns[1] ? Mathf.Min(0, m_velocity.y) : Mathf.Max(0, m_velocity.y);
        m_velocity.z = m_initialSigns[2] ? Mathf.Min(0, m_velocity.z) : Mathf.Max(0, m_velocity.z);

        return m_velocity.magnitude == 0.0f;
    }

    public override bool TryParse(string[] parameters)
    {
        if(parameters.Length < 3)
        {
            return false;
        }

        if(!TryParseObject(parameters[0].Trim(), out m_object))
        {
            return false;
        }

        if(!TryParseVector(parameters[1].Trim(), out m_velocity))
        {
            m_object = null;
            return false;
        }

        if(!TryParseVector(parameters[2].Trim(), out m_deceleration))
        {
            m_object = null;
            return false;
        }

        m_initialSigns = GetSigns(m_velocity);

        return true;
    }
}

public class Bounce : SceneActionBase
{
    private GameObject m_object;
    private Vector3 m_currentVelocity;
    private Vector3 m_bounceVelocity;
    private Vector3 m_deceleration;
    private Vector3 m_initialPosition;
    private Axis m_bounceAxis;
    private float m_decayRate = 0.5f;
    private float m_maxBounces = 4.0f;
    private float m_bounces = 0;
    private float m_minDecay;
    private bool m_bounceDirectionPositive = true;
    private bool[] m_initialVelocitySign;

    public Bounce() { }

    public Bounce(GameObject obj, Vector3 vel, Vector3 decel, Axis axis, float decay, float nBounces)
    {
        m_object = obj;
        m_initialPosition = m_object.transform.position;

        m_currentVelocity = vel;
        m_deceleration = decel;
        m_bounceAxis = axis;
        m_decayRate = decay;
        m_bounces = nBounces;

        m_bounceDirectionPositive = m_deceleration[(int)m_bounceAxis] > 0.0f;
        m_initialVelocitySign = GetSigns(m_currentVelocity);
        Vector3 bounceDir = new Vector3();
        bounceDir[(int)m_bounceAxis] = 0.5f;
        m_bounceVelocity = bounceDir * m_currentVelocity[(int)m_bounceAxis];
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        m_object.transform.position += m_currentVelocity * Time.deltaTime;
        m_currentVelocity -= m_deceleration * Time.deltaTime;

        //Prevent acceleration in opposite direction for non bounce axis
        for(int i = 0; i < 3; i++)
        {
            if(i == (int)m_bounceAxis)
            {
                continue;
            }

            m_currentVelocity[i] = m_initialVelocitySign[i] ? Mathf.Max(m_currentVelocity[i], 0.0f) : Mathf.Min(m_currentVelocity[i], 0.0f);
        }

        //should bounce
        if((m_bounceDirectionPositive && (m_object.transform.position[(int)m_bounceAxis] <= (m_initialPosition[(int)m_bounceAxis] + 0.001f))) || 
               (!m_bounceDirectionPositive && (m_object.transform.position[(int)m_bounceAxis] >= (m_initialPosition[(int)m_bounceAxis] - 0.001f))))
        {
            Vector3 curPosition = m_object.transform.position;
            curPosition[(int)m_bounceAxis] = m_initialPosition[(int)m_bounceAxis];
            m_object.transform.position = curPosition;

            if(m_bounces >= m_maxBounces)
            {
                return true;
            }

            m_bounces++;

            m_currentVelocity += m_bounceVelocity;
            m_bounceVelocity *= m_decayRate;
        }

        return false;
    }

    public override bool TryParse(string[] parameters)
    {
        TryParseObject(parameters[0], out m_object);
        m_initialPosition = m_object.transform.position;

        TryParseVector(parameters[1], out m_currentVelocity);

        TryParseVector(parameters[2], out m_deceleration);
        TryParseAxis(parameters[3], out m_bounceAxis);

        m_bounceDirectionPositive = m_deceleration[(int)m_bounceAxis] > 0.0f;
        m_initialVelocitySign = GetSigns(m_currentVelocity);
        Vector3 bounceDir = new Vector3();
        bounceDir[(int)m_bounceAxis] = 0.5f;
        m_bounceVelocity = bounceDir * m_currentVelocity[(int)m_bounceAxis];

        TryParseFloat(parameters[4], out m_decayRate);
        
        TryParseFloat(parameters[5], out m_maxBounces);        

        return true;
    }
}

public class FadeIn : SceneActionBase
{
    SpriteRenderer m_renderer;
    float m_rate;

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        Color c = m_renderer.color;

        c.a += Mathf.Min(m_rate * Time.deltaTime, 1.0f);
        m_renderer.color = c;

        return c.a >= 1.0f;
    }

    public override bool TryParse(string[] parameters)
    {
        GameObject obj;
        TryParseObject(parameters[0], out obj);

        m_renderer = obj.GetComponent<SpriteRenderer>();

        TryParseFloat(parameters[1], out m_rate);

        return true;
    }
}

public class FadeOut : SceneActionBase
{
    SpriteRenderer m_renderer;
    float m_rate;

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Tick()
    {
        Color c = m_renderer.color;

        c.a -= Mathf.Max(m_rate * Time.deltaTime, 0.0f);
        m_renderer.color = c;

        return c.a <= 0.0f;
    }

    public override bool TryParse(string[] parameters)
    {
        GameObject obj;
        TryParseObject(parameters[0], out obj);

        m_renderer = obj.GetComponent<SpriteRenderer>();

        TryParseFloat(parameters[1], out m_rate);

        return true;
    }
}

public class SceneThread
{
    public delegate void ThreadCompleted();

    public event ThreadCompleted ThreadCompletedEvent;

    private List<SceneActionBase> m_executionLines;

    private int m_currentLine = 0;

    public SceneThread(SceneActionBase action, ThreadCompleted callback)
    {
        m_executionLines = new List<SceneActionBase>();
        m_executionLines.Add(action);
        m_currentLine = 0;

        if(callback != null)
        {
            ThreadCompletedEvent += callback;
        }
    }

    public SceneThread(List<SceneActionBase> executionLines, ThreadCompleted callback)
    {
        m_currentLine = 0;
        m_executionLines = executionLines;

        if (callback != null)
        {
            ThreadCompletedEvent += callback;
        }
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
    private List<SceneThread> m_threads_to_push = new List<SceneThread>();

    public GameObject DamageNumberPrefab;

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

        foreach (var t in m_threads_to_push)
        {
            m_threads.Add(t);
        }
        m_threads_to_push.Clear();
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

    public bool ExecuteActionSequence(string[] actionsToParse, SceneThread.ThreadCompleted callback = null)
    {
        if(actionsToParse.Length < 1)
        {
            return false;
        }

        List<SceneActionBase> actions = new List<SceneActionBase>();

        foreach(string actionToParse in actionsToParse)
        {
            SceneActionBase action;

            if(ParseAction(actionToParse, out action))
            {
                actions.Add(action);
            }
            else
            {
                return false;
            }
        }

        SceneThread thread = new SceneThread(actions, callback);
        m_threads_to_push.Add(thread);
        return true;
    }

    public bool ExecuteActionSequence(string actions, SceneThread.ThreadCompleted callback = null)
    {
        return ExecuteActionSequence(actions.Split('|'), callback);
    }

    public bool ExecuteAction(string actionToParse, SceneThread.ThreadCompleted callback = null)
    {
        SceneActionBase action;
        
        if(ParseAction(actionToParse, out action))
        {
            SceneThread thread = new SceneThread(action, callback);
            m_threads_to_push.Add(thread);
            
            return true;
        }

        return false;
    }

    public void SpawnDamageNumber(Vector3 position, string text)
    {
        GameObject obj = Instantiate(DamageNumberPrefab);

        obj.transform.position = position;
        obj.GetComponent<TextMesh>().text = text;

        obj.SetActive(true);

        List<SceneActionBase> actions = new List<SceneActionBase>();

        actions.Add(new Bounce(obj, new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0.75f), Axis.Y, 0.5f, 1));
        
        actions.Add(new DestroyObject(obj));

        SceneThread thread = new SceneThread(actions, null);
        m_threads_to_push.Add(thread);
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
                break;
            case "Wait":
                action = new Wait();
                break;
            case "SetActive":
                action = new SetActive();
                break;
            case "MoveArc":
                action = new MoveArc();
                break;
            case "StorePosition":
                action = new StorePosition();
                break;
            case "Bounce":
                action = new Bounce();
                break;
            case "SpawnDamageNumber":
                action = new SpawnDamageNumber();
                break;
            case "FadeIn":
                action = new FadeIn();
                break;
            case "FadeOut":
                action = new FadeOut();
                break;
            case "TeleportTo":
                action = new TeleportTo();
                break;

            case "BanditSpawnAndAttack":
                action = new BanditSpawnAndAttack();
                break;
            
            case "BanditSuckerPunch":
                action = new BanditSuckerPunch();
                break;
            
            case "StartCombat2":
                action = new BlankAction();
                DialogueSystem.Get().startDialogue(Combat2);
                break;
            
            case "BanditBeatdown":
                action = new BanditBeatdown();
                break;
            
            case "FinishEpilogue":
                action = new BlankAction();
                DialogueSystem.Get().hide();
                DialogueSystem.Get().Defade();
                SceneManager.LoadScene("Ending");
                break;
            
            case "EndSequence":
                action = new BlankAction();
                DialogueSystem.Get().EndSequence();
                break;
            
            case "PlayerSetName":
                GagPlayerNameEntry.Get().StartGag();
                action = new BlankAction();
                break;
            
            default:
                Debug.LogError("SceneSystem: Unable to parse action \"" + actionToParse + "\"");
                action = new InvalidAction();
                return false;
        }

        if (!action.TryParse(parameters.ToArray()))
        {
            action = new InvalidAction();
            return false;
        }

        return true;
    }
    
    public DialogueSession Combat2;
}

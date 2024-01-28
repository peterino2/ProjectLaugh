using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Dialogue.Gags;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GagAttributeAllocator : MonoBehaviour
{
    public static GagAttributeAllocator gGagAttributeAllocator;

    public void Awake()
    {
        gGagAttributeAllocator = this;
    }

    public static GagAttributeAllocator Get()
    {
        return gGagAttributeAllocator;
    }

    public DialogueSession highStrSession;
    public DialogueSession highDexSession;
    public DialogueSession highIntSession;
    public DialogueSession evenSession;

    public GameObject rootObject;
    public TextMeshProUGUI StrStat;
    public TextMeshProUGUI DexStat;
    public TextMeshProUGUI IntStat;
    public TextMeshProUGUI PointsAvailable;
    
    public Button StrIncrement;
    public Button StrDecrement;
    
    public Button DexIncrement;
    public Button DexDecrement;
    
    public Button IntIncrement;
    public Button IntDecrement;
    
    public Button AcceptButton;
    

    void Start()
    {
        StrStat.text = GagPlayerAttributes.Get().strength.ToString();
        DexStat.text = GagPlayerAttributes.Get().dexterity.ToString();
        IntStat.text = GagPlayerAttributes.Get().intelligence.ToString();

        StrIncrement.onClick.AddListener(IncreaseStr);
        StrDecrement.onClick.AddListener(DecreaseStr);
        
        DexIncrement.onClick.AddListener(IncreaseDex);
        DexDecrement.onClick.AddListener(DecreaseDex);
        
        IntIncrement.onClick.AddListener(IncreaseInt);
        IntDecrement.onClick.AddListener(DecreaseInt);
        
        AcceptButton.onClick.AddListener(OnAcceptButton);
        rootObject.SetActive(false);
    }

    private int pointsAvailable = 3;

    void IncreaseStr()
    {
        if (pointsAvailable > 0 && GagPlayerAttributes.Get().strength < 4)
        {
            pointsAvailable -= 1;
            GagPlayerAttributes.Get().strength += 1;
            StrStat.text = GagPlayerAttributes.Get().strength.ToString();
            updatePoints();
        }
    }
    
    void DecreaseStr()
    {
        if (pointsAvailable < 3 && GagPlayerAttributes.Get().strength > 1)
        {
            pointsAvailable += 1;
            GagPlayerAttributes.Get().strength -= 1;
            StrStat.text = GagPlayerAttributes.Get().strength.ToString();
        updatePoints();
        }
    }
    
    void IncreaseDex()
    {
        if (pointsAvailable > 0 && GagPlayerAttributes.Get().dexterity < 4)
        {
            pointsAvailable -= 1;
            GagPlayerAttributes.Get().dexterity += 1;
            DexStat.text = GagPlayerAttributes.Get().dexterity.ToString();
        updatePoints();
        }
    }
    
    void DecreaseDex()
    {
        if (pointsAvailable < 3 && GagPlayerAttributes.Get().dexterity > 1)
        {
            pointsAvailable += 1;
            GagPlayerAttributes.Get().dexterity -= 1;

            DexStat.text = GagPlayerAttributes.Get().dexterity.ToString();
        updatePoints();
        }
    }

    void updatePoints()
    {
        PointsAvailable.text = pointsAvailable.ToString();
    }
    
    void IncreaseInt()
    {
        if (pointsAvailable > 0 && GagPlayerAttributes.Get().intelligence < 4)
        {
            pointsAvailable -= 1;
            GagPlayerAttributes.Get().intelligence += 1;
            IntStat.text = GagPlayerAttributes.Get().intelligence.ToString();
        updatePoints();
        }
    }
    
    void DecreaseInt()
    {
        if (pointsAvailable < 3 && GagPlayerAttributes.Get().intelligence > 1)
        {
            pointsAvailable += 1;
            GagPlayerAttributes.Get().intelligence -= 1;
            IntStat.text = GagPlayerAttributes.Get().intelligence.ToString();
            updatePoints();
        }
    }

    public void OnAcceptButton()
    {
        if (GagPlayerAttributes.Get().intelligence >= 3)
        {
            DialogueSystem.Get().HandleAttributesAllocated(highIntSession);
        }
        else if(GagPlayerAttributes.Get().strength >= 3)
        {
            DialogueSystem.Get().HandleAttributesAllocated(highStrSession);
        }
        else if(GagPlayerAttributes.Get().dexterity >= 3)
        {
            DialogueSystem.Get().HandleAttributesAllocated(highDexSession);
        }
        else
        {
            DialogueSystem.Get().HandleAttributesAllocated(evenSession);
        }
            
        Debug.Log("Accepted");
        rootObject.SetActive(false);
    }

    public void OpenAllocator()
    {
        rootObject.SetActive(true);
    }
    

    void Update() { }
}

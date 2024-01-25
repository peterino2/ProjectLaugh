using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueChoice
    {
        [SerializeField]
        public string choiceString;
        
        [SerializeField]
        public string nodeRef;
    }
    
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField]
        public string displayText;
        
        [SerializeField]
        public string speaker;
        
        [SerializeField]
        public string NodeId = "";

        [SerializeField]
        public List<DialogueChoice> choicesText;

        public bool isMovement;
        public string movementDescription;
    }
    
    [CreateAssetMenu(fileName = "DS_", menuName = "DialogueSession", order = 0)]
    public class DialogueSession : ScriptableObject
    {
        [SerializeField]
        public List<DialogueNode> DialogueLines;

        public Dictionary<string, DialogueNode> NodesByName = new Dictionary<string, DialogueNode>();

        public void OnValidate()
        {
            NodesByName.Clear();
            foreach (var x in DialogueLines)
            {
                if (NodesByName.ContainsKey(x.NodeId) || x.NodeId == "")
                {
                    x.NodeId = Random.Range(0, Int32.MaxValue).ToString();

                    while (NodesByName.ContainsKey(x.NodeId))
                    {
                        x.NodeId = Random.Range(0, Int32.MaxValue).ToString();
                    }

                    NodesByName[x.NodeId] = x;
                    
                    Debug.Log("Assigned node in map " + x.ToString() + " to id " + x.NodeId);
                }
                else
                {
                    NodesByName[x.NodeId] = x;
                }
            }
        }
    }
}
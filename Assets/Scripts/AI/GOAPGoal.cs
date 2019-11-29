using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

public abstract class GOAPGoal 
{
    public string Key { get; protected set; }
    private List<GOAPAction[]> _paths;

    public GOAPGoal(string key)
    {
        Key = key;
        
        _paths = new List<GOAPAction[]>();
    }

    public virtual float GetGoalScore()
    {
        return 1f;
    }

    public class Goal
    {
        public const string IS_WORKING = "IS_WORKING";
        public const string IS_RESTED = "IS_RESTED";
        public const string IS_IDLING = "IS_IDLING";
    }
}

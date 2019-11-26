using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPGoal 
{
    public string Key { get; protected set; }

    public GOAPGoal(string key)
    {
        Key = key;
    }

    public class Goal
    {
        public const string IS_WORKING = "IS_WORKING";
    }
}

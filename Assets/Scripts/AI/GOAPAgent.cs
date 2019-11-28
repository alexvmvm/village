using System.Collections.Generic;
using UnityEngine;
using Village.AI;
using System.Linq;

public abstract class GOAPAgent : MonoBehaviour
{
    public Dictionary<GOAPGoal, List<GOAPAction[]>> Paths { get { return _paths; } }
    
    private List<GOAPGoal> _goals;
    private List<GOAPAction> _actions;
    private Dictionary<GOAPGoal, List<GOAPAction[]>> _paths;
    private bool _dirty;

    public virtual void Awake()
    {
        _goals = new List<GOAPGoal>();
        _actions = new List<GOAPAction>();
        _paths = new Dictionary<GOAPGoal, List<GOAPAction[]>>();
    }

    public void AddGoal(GOAPGoal goal)
    {
        _goals.Add(goal);
        _dirty = true;
    }

    public void AddAction(GOAPAction action)
    {
        _actions.Add(action);
        _dirty = true;
    }

    void CalculatePaths()
    {
        _paths.Clear();
        foreach(var goal in _goals)
        {
            var list = new List<GOAPAction[]>();
            foreach(var action in _actions.Where(a => a.Goal == goal.Key))
            {
                list.Add(GetActionPath(action));
            }
            _paths.Add(goal, list);
        }
    }

    GOAPAction[] GetActionPath(GOAPAction root)
    {
        var usable = new List<GOAPAction>(_actions);
        var list = new LinkedList<GOAPAction>();
        list.AddLast(root);
        
        while(true)
        {
            var next = FindNextAction(list.Last.Value);
            if(next != null)
            {
                list.AddLast(next);
            }
            else
            {
                break;
            }
        }

        return list.ToArray();
    }

    GOAPAction FindNextAction(GOAPAction root)
    {
        foreach(var action in _actions)
        {
            if(action.Effects.IsEqualTo(root.Preconditions))
            {
                return action;
            }
        }

        return null;
    }

    void LateUpdate()
    {
        if(_dirty)
        {
            CalculatePaths();
            _dirty = false;
        }
    }
}


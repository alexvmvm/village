using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public enum AgentState
{
    Planning,
    Picking,
    Performing,
    Completed
}

public abstract class Agent 
{
    public DateTime Created { get { return _created; } }
    protected Game _game;
    protected Thing _thing;
    private NormalPlanner _planner;
    private HashSet<GOAPAction> _available;
    private Queue<GOAPAction> _actions;
    private List<GOAPAction> _useable;
    private GOAPAction _current;
    private AgentState _state;
    protected Movement _movement;
    private DateTime _created;
    private bool _setup;

    public Agent(Game game, Thing thing)
    {
        _game = game;
        _thing = thing;        
    
        _planner = new NormalPlanner();
        _available = new HashSet<GOAPAction>();
        _actions = new Queue<GOAPAction>();
        _useable = new List<GOAPAction>();
    }

    public virtual void Setup()
    {
        _created = new DateTime();
        _movement = _thing.transform.gameObject.AddComponent<Movement>();
        _setup = true;
    }

    public void AddAction(GOAPAction action)
    {
        _available.Add(action);
    }

    public abstract Dictionary<string, bool> GetWorldState();
    public abstract Dictionary<string, bool> GetGoal();

    bool Plan(Dictionary<string, bool> world, Dictionary<string, bool> goal, Queue<GOAPAction> plan)
    {

        // Reset actions
        foreach (var action in _available)
        {
            action.Reset();
        }

        _useable.Clear();
        foreach (var action in _available)
        {
            if (action.IsPossibleToPerform())
            {
                _useable.Add(action);
            }  
        }

        // Build graph
        _planner.BuildPlan(world, _useable, goal, _actions);

        if (_actions.Count == 0)
        {
            foreach (var action in _useable)
                action.Reset();

            return false;
        }
        else
        {
            foreach (var item in _useable)
            {
                if (!_actions.Contains(item))
                {
                    item.Reset();
                }
            }

            return true;
        }
    }

    public void Update()
    {
        if(!_setup)
            return;

        switch(_state)
        {
            case AgentState.Planning:
            {
                _actions.Clear();
                if(Plan(GetWorldState(), GetGoal(), _actions))
                    _state = AgentState.Picking;
            }
            break;
            case AgentState.Picking:
            {
                if(_actions.Count() > 0)
                {
                    _current = _actions.Dequeue();
                    _state = AgentState.Performing;
                }  
                else
                    _state = AgentState.Completed;
            }
            break;
            case AgentState.Performing:
            {
                _current.Perform();
                if(_current.IsDone())
                    _state = AgentState.Picking;
            }
            break;
            case AgentState.Completed:
            {
                _state = AgentState.Planning;
            }
            break;
        }
    }

}

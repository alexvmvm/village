using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AgentState
{
    Planning,
    Picking,
    Performing,
    Completed
}

public class Agent 
{
    private Game _game;
    private Thing _thing;
    private NormalPlanner _planner;
    private HashSet<GOAPAction> _available;
    private Queue<GOAPAction> _actions;
    private List<GOAPAction> _useable;
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _worldState;
    private Dictionary<string, bool> _currentState;
    private GOAPAction _current;
    private AgentState _state;

    public Agent(Game game, Thing thing)
    {
        _game = game;
        _thing = thing;

        _planner = new NormalPlanner();
        _available = new HashSet<GOAPAction>();
        _actions = new Queue<GOAPAction>();
        _useable = new List<GOAPAction>();

    }

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

    // IEnumerator AgentPoll()
    // {
    //     while (true)
    //     {
    //         _actions.Clear();
                

    //         var planFound = false;

    //         // no plan
    //         if (_goal.Count > 0)
    //         {
    //             if (Plan(_worldState, _goal, _actions))
    //             {
    //                 planFound = true;
    //             }
    //             else
    //             {
    //                 yield return _waitAfterPlanNotFound;
    //             }
    //         }

    //         if (planFound)
    //         {
    //             while (_actions.Count() > 0)
    //             {
    //                 _current = _actions.Dequeue();

    //                 yield return StartCoroutine(_current.Perform());

    //                 // failed to peform action
    //                 if (!_current.IsDone())
    //                 {
    //                     Debug.Log("Failed plan");
    //                     break;
    //                 }
    //             }
    //         }

    //         _current = null;

    //         yield return null;
    //     }
    // }

    public void Update()
    {
        switch(_state)
        {
            case AgentState.Planning:
            {
                _actions.Clear();
                if(Plan(_worldState, _goal, _actions))
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

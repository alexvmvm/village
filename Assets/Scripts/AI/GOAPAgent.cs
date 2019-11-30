using System.Collections.Generic;
using UnityEngine;
using Village.AI;
using System.Linq;
using Helpers.Tree;
using Helpers.Graph;

public enum AgentState
{
    Planning,
    Picking,
    Performing,
    Completed,
    Paused
}

public abstract class GOAPAgent : MonoBehaviour
{
    public Dictionary<GOAPGoal, List<GOAPAction[]>> Paths { get { return _paths; } }
    public GOAPAction CurrentAction { get { return _current; } }
    public GOAPAction IdleAction { get; protected set; }
    private List<GOAPGoal> _goals;
    private List<GOAPAction> _actions;
    private Dictionary<GOAPGoal, List<GOAPAction[]>> _paths;
    private Dictionary<string, object> _state;
    private bool _dirty;
    private AgentState _agentState;
    private GOAPAction _current;
    private Queue<GOAPAction> _plan;
    private List<GOAPAction> _workingPlan;
    private Graph<GOAPAction> _graph;

    public virtual void Awake()
    {
        _goals = new List<GOAPGoal>();
        _actions = new List<GOAPAction>();
        _paths = new Dictionary<GOAPGoal, List<GOAPAction[]>>();
        _state = new Dictionary<string, object>();
        _plan = new Queue<GOAPAction>();
        _workingPlan = new List<GOAPAction>();
        _graph = new Graph<GOAPAction>();
    }

    void Start()
    {
        CalculateGraph();
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

    void CalculateGraph()
    {
        _graph.Clear();

        foreach(var action in _actions)
        {
            _graph.AddNode(action);
        }

        foreach(var action in _actions)
        {
            var effects = _actions.Where(a => action.Effects.IsEqualTo(a.Preconditions));

            foreach(var effect in effects)
            {
                _graph.AddDirectedEdge(effect, action);
            }

            var preconditions = _actions.Where(a => action.Preconditions.IsEqualTo(a.Effects));

            foreach(var precon in preconditions)
            {
                _graph.AddDirectedEdge(action, precon);
            }
        }
    }

    bool FindPlan()
    {
        _workingPlan.Clear();

        // get goals ordered by score
        foreach(var goal in _goals.OrderBy(g => g.GetGoalScore()))
        {
            var bestScore = Mathf.Infinity;

            // get all actions that fulfill this goal
            foreach(var action in _actions.Where(a => a.Goal == goal.Key))
            {
                // find a path back to the agents 
                // current state
                if(_graph.ShortestPathSearch(
                    action, 
                    a => a.IsPossibleToPerform(), 
                    a => a.Preconditions.IsSubsetOf(_state), 
                    ref _workingPlan))
                {
                    // test core
                    var score = _workingPlan.Sum(a => a.Cost);
                    if(score < bestScore)
                    {
                        bestScore = score;
                        
                        // construct plan
                        _plan.Clear();
                        foreach(var a in _workingPlan)
                            _plan.Enqueue(a);
                    }
                }

                if(bestScore < Mathf.Infinity)
                    return true;
            }
        }

        return false;
    }

    public abstract void UpdateState(Dictionary<string, object> state);

    public abstract void ActionCompleted(GOAPAction action);

    public void PauseAgent()
    {
        _agentState = AgentState.Paused;
    }

    public void UnPauseAgent()
    {
        if(_agentState == AgentState.Paused)
            _agentState = AgentState.Planning;
    }

    /*
        Update
    */

    public virtual void Update()
    {        
        switch(_agentState)
            {
                case AgentState.Planning:
                {
                    UpdateState(_state);

                    foreach(var action in _actions)
                    {
                        action.Reset();
                    }

                    if(FindPlan())
                    {
                        _agentState = AgentState.Picking;
                    }
                    else
                    {
                        
                        if(IdleAction != null)
                        {
                            IdleAction.Reset();
                            if(IdleAction.IsPossibleToPerform())
                            {
                                _current = IdleAction;
                                _agentState = AgentState.Performing;
                            }
                        }
                    }
                }
                break;
                case AgentState.Picking:
                {
                    if(_plan.Count() > 0)
                    {
                        _current = _plan.Dequeue();
                        _agentState = AgentState.Performing;
                    }  
                    else
                        _agentState = AgentState.Completed;
                }
                break;
                case AgentState.Performing:
                {
                    if(_current.IsDone())
                    {
                        ActionCompleted(_current);
                        _agentState = AgentState.Picking;
                    }
                    else if(!_current.Perform())
                    {
                        _agentState = AgentState.Picking;
                    }                        
                }
                break;
                case AgentState.Completed:
                {
                    _current = null;
                    _agentState = AgentState.Planning;
                }
                break;
                case AgentState.Paused:
                break;
            }
    }

    void LateUpdate()
    {
        if(_dirty)
        {
            CalculateGraph();
            _dirty = false;
        }
    }
    
    public virtual void OnDrawGizmos()
    {
        if(_actions == null)
            return;

        foreach(var action in _actions)
        {
            action.DrawGizmos();
        }
    }
}


using System.Collections.Generic;
using UnityEngine;
using Village.AI;
using System.Linq;

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

    public virtual void Awake()
    {
        _goals = new List<GOAPGoal>();
        _actions = new List<GOAPAction>();
        _paths = new Dictionary<GOAPGoal, List<GOAPAction[]>>();
        _state = new Dictionary<string, object>();
        _plan = new Queue<GOAPAction>();
        _workingPlan = new List<GOAPAction>();
    }

    void Start()
    {
        CalculatePaths();
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

    /*
        Calculations
    
        1. convert to tree
        2. find all leafs 
        3. find all paths from leafs
        
    */
    

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

            if(list.Count > 1000)
            {
                Debug.LogError($"Plan too long, exceded 1000. Root ${root.ToString()}");
                return null;
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

    IEnumerable<GOAPAction> FindPlan()
    {
        // loop goals in priority order
        foreach(var kv in _paths.OrderBy(kv => kv.Key.GetGoalScore()))
        {
            // loop possible paths in priority order
            foreach(var path in kv.Value.OrderBy(p => p.Sum(a => a.Cost)))
            {
                _workingPlan.Clear();
                foreach(var action in path)
                {
                    if(action.IsPossibleToPerform())
                    {
                        _workingPlan.Add(action);
                    }
                    else if(action.IsAlreadyDone(_state) && _workingPlan.Count > 0)
                    {
                        _workingPlan.Reverse();
                        return _workingPlan;
                    }
                }

                if(path.All(a => a.IsPossibleToPerform()))
                    return path.Reverse();
            }
        }
        
        return null;
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

                    var plan = FindPlan();
                    if(plan != null)
                    {
                        _plan.Clear();
                        foreach(var action in plan)
                        {
                            _plan.Enqueue(action);
                        }
                            
                        _agentState = AgentState.Picking;
                    }
                    else
                    {
                        if(IdleAction != null)
                        {
                            _current = IdleAction;
                            _current.Reset();
                            _agentState = AgentState.Performing;
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
            CalculatePaths();
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


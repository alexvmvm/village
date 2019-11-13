﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Village.AI
{
    public enum AgentState
    {
        Planning,
        Picking,
        Performing,
        Completed,
        Paused
    }

    public abstract class Agent : MonoBehaviour
    {
        public DateTime Created { get { return _created; } }
        public GOAPAction CurentAction { get { return _current; } }
        protected Game _game;
        private NormalPlanner _planner;
        private HashSet<GOAPAction> _available;
        private Queue<GOAPAction> _actions;
        private List<GOAPAction> _useable;
        protected GOAPAction _current;
        private AgentState _state;
        private DateTime _created;

        public virtual void Awake()
        {
            _game = FindObjectOfType<Game>();    
            _planner = new NormalPlanner();
            _available = new HashSet<GOAPAction>();
            _actions = new Queue<GOAPAction>();
            _useable = new List<GOAPAction>();

            _created = DateTime.Now;
        }

        public void AddAction(GOAPAction action)
        {
            _available.Add(action);
        }

        public abstract void ActionCompleted(GOAPAction action);

        public abstract Dictionary<string, object> GetWorldState();
        public abstract Dictionary<string, object> GetGoal();

        bool Plan(Dictionary<string, object> world, Dictionary<string, object> goal, Queue<GOAPAction> plan)
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

            Profiler.BeginSample("Agent_BuildPlan");

            // Build graph
            _planner.BuildPlan(world, _useable, goal, _actions);

            Profiler.EndSample();

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

        public virtual void PauseAgent()
        {
            _state = AgentState.Paused;
        }

        public virtual void UnPauseAgent()
        {
            _state = AgentState.Planning;
        }

        public virtual void Update()
        {
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
                    {
                        ActionCompleted(_current);
                        _state = AgentState.Picking;
                    }
                        
                }
                break;
                case AgentState.Completed:
                {
                    _current = null;
                    _state = AgentState.Planning;
                }
                break;
                case AgentState.Paused:
                break;
            }
        }

        public virtual void DrawGizmos()
        {
            
        }
    }

}
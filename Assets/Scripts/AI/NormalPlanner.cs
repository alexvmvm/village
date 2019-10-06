using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalPlanner
{
    private class Node 
    {
		public Node parent;
		public float runningCost;
		public Dictionary<string,object> state;
		public GOAPAction action;

		public Node(Node parent, float runningCost, Dictionary<string,object> state, GOAPAction action) 
        {
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
		}
	}

	private List<Node> _leaves;
	private List<GOAPAction> _result;

	public NormalPlanner()
	{
		_leaves = new List<Node>();
		_result = new List<GOAPAction>();
	}

    bool InState(Dictionary<string,object> test, Dictionary<string,object> state) {
		bool allMatch = true;
		foreach (KeyValuePair<string,object> t in test) 
        {
			bool match = false;
			foreach (KeyValuePair<string,object> s in state) 
            {
				if (s.Equals(t)) 
                {
					match = true;
					break;
				}
			}
			if (!match)
				allMatch = false;
		}
		return allMatch;
	}

    public void BuildPlan(Dictionary<string, object> agentState, List<GOAPAction> usable, Dictionary<string, object> goal, Queue<GOAPAction> plan)
    {
		_leaves.Clear();
		_result.Clear();
		
		var start = new Node (null, 0, agentState, null);
        var success = BuildPlan(start, _leaves, usable, goal);

		Node cheapest = null;
		foreach (var leaf in _leaves) {
			if (cheapest == null)
				cheapest = leaf;
			else {
				if (leaf.runningCost < cheapest.runningCost)
					cheapest = leaf;
			}
		}

		// get its node and work back through the parents
		var n = cheapest;
		while (n != null) {
			if (n.action != null) {
				_result.Insert(0, n.action); // insert the action in the front
			}
			n = n.parent;
		}

		// we now have this action list in correct order
		foreach (var a in _result) 
		{
			plan.Enqueue(a);
		}

    }

    private Dictionary<string,object> PopulateState(Dictionary<string,object> currentState, Dictionary<string,object> stateChange) {
	    
        var state = new Dictionary<string,object> ();
		
        // copy the KVPs over as new objects
		foreach (var s in currentState) 
        {
			state.Add(s.Key, s.Value);
		}

		foreach (var change in stateChange) 
        {
            if(state.ContainsKey(change.Key))
                state.Remove(change.Key);
            state.Add(change.Key,change.Value);
		}
		return state;
	}

    List<GOAPAction> ActionSubset(List<GOAPAction> actions, GOAPAction removeMe) {
		var subset = new List<GOAPAction> ();
		foreach (GOAPAction a in actions) 
        {
			if (!a.Equals(removeMe))
				subset.Add(a);
		}
		return subset;
	}

    bool BuildPlan(Node parent,  List<Node> leaves, List<GOAPAction> usable, Dictionary<string, object> goal)
    {
        bool foundOne = false;

        foreach (var action in usable) 
        {
			// if the parent state has the conditions for this action's preconditions, we can use it here
			if (InState(action.Preconditions, parent.state) ) {

				// apply the action's effects to the parent state
				var currentState = PopulateState (parent.state, action.Effects);
				//Debug.Log(GoapAgent.prettyPrint(currentState));
				var node = new Node(parent, parent.runningCost+action.Cost, currentState, action);

				if (goal.Any(kv => currentState.ContainsKey(kv.Key) && currentState[kv.Key].Equals(kv.Value) && parent.state.ContainsKey(kv.Key) && !parent.state[kv.Key].Equals(kv.Value))) 
				{
					// we found a solution!
					leaves.Add(node);
					foundOne = true;
				} else {
					// not at a solution yet, so test all the remaining actions and branch out the tree
					var subset = ActionSubset(usable, action);
					bool found = BuildPlan(node, leaves, subset, goal);
					if (found)
						foundOne = true;
				}
			}
		}

        return foundOne;
    }
}

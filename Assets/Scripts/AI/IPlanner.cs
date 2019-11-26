using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

public interface IPlanner
{
    void BuildPlan(Dictionary<string, object> agentState, List<GOAPAction> usable, Dictionary<string, object> goal, Queue<GOAPAction> plan);
}

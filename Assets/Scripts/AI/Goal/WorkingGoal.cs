using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingGoal : GOAPGoal
{
    public WorkingGoal() : base(GOAPGoal.Goal.IS_WORKING)
    {
    }

    public override float GetGoalScore()
    {
        return 1f;
    }
}

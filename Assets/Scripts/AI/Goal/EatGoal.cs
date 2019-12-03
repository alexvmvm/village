using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

public class EatGoal : GOAPGoal
{
    private Needs _needs;
    public EatGoal(Needs needs) : base(GOAPGoal.Goal.IS_NOT_HUNGRY)
    {
        _needs = needs;
    }

    public override float GetGoalScore()
    {
        return 10f;
    }

    public override bool IsActive()
    {
        return _needs.IsNeedCritical(Need.HUNGER);
    }
}

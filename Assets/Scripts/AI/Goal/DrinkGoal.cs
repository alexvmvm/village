using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

public class DrinkGoal : GOAPGoal
{
    private Needs _needs;
    public DrinkGoal(Needs needs) : base(GOAPGoal.Goal.IS_NOT_THIRSTY)
    {
        _needs = needs;
    }

    public override float GetGoalScore()
    {
        return 20f;
    }

    public override bool IsActive()
    {
        return _needs.IsThirsty();
    }
}

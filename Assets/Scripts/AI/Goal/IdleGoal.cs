﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGoal : GOAPGoal
{
    public IdleGoal() : base(GOAPGoal.Goal.IS_IDLING)
    {
    }
    
    public override float GetGoalScore()
    {
        return 0f;
    }
}

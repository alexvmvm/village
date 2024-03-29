﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village;
using Village.AI;

public class RestGoal : GOAPGoal
{
    private Needs _needs;
    private Game _game;

    public RestGoal(Game game, Needs needs) : base(GOAPGoal.Goal.IS_RESTED)
    {
        _needs = needs;
        _game = game;
    }

    public override float GetGoalScore()
    {
        return 30f;
    }

    public override bool IsActive()
    {
        return _game.WorldTime.TimeOfDay == TimeOfDay.Night;
    }
}

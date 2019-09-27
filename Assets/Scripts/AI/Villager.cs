using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Agent
{
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;

    public Villager(Game game, Thing thing) : base(game, thing)
    {

        SetActions(
            new Idle(game, _movement)
        ); 

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _world.Add("isIdle", false);
        _goal.Add("isIdle", true);
    }

    public override Dictionary<string, bool> GetGoal()
    {
        return _goal;
    }

    public override Dictionary<string, bool> GetWorldState()
    {
        return _world;
    }
}

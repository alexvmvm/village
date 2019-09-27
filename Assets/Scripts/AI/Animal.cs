using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Agent
{
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;

    public Animal(Game game, Thing thing) : base(game, thing)
    {
        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _world.Add("isIdle", true);
        _goal.Add("isIdle", false);

        AddAction(new Idle(_game, _movement) {
            Effects         = { { "isIdle", false } },
            Preconditions   = { { "isIdle", true } }
        }); 
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : Agent
{
    private Dictionary<string, object> _goal;
    private Dictionary<string, object> _world;

    public Director(Game game) : base(game)
    {
        _goal = new Dictionary<string, object>();
        _world = new Dictionary<string, object>();

        AddAction(new SpawnVillager(game) {
            Preconditions = { { "isWorking", false } },
            Effects = { { "isWorking", true } }
        });
    }

    public override void ActionCompleted(GOAPAction action)
    {
        
    }

    public override Dictionary<string, object> GetGoal()
    {
        _goal["isWorking"] = true;
        return _goal;
    }

    public override Dictionary<string, object> GetWorldState()
    {
        _world["isWorking"] = false;
        return _world;
    }
}

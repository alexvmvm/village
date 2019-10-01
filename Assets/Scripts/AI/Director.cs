using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : Agent
{
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;

    public Director(Game game) : base(game)
    {
        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        AddAction(new SpawnVillager(game) {
            Preconditions = { { "isWorking", false } },
            Effects = { { "isWorking", true } }
        });
    }

    public override void ActionCompleted(GOAPAction action)
    {
        
    }

    public override Dictionary<string, bool> GetGoal()
    {
        _goal["isWorking"] = true;
        return _goal;
    }

    public override Dictionary<string, bool> GetWorldState()
    {
        _world["isWorking"] = false;
        return _world;
    }
}

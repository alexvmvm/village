using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Agent
{
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;
    public Villager(Game game, Thing thing) : base(game, thing)
    {
        _thing.name = NameGenerator.GenerateCharacterName();

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _world.Add("isIdle", true);
        _world.Add("hasLogs", false);

        _goal.Add("hasLogs", true);

        AddAction(new Idle(_game, _movement) {
            Preconditions   = { { "isIdle", true } },
            Effects         = { { "isIdle", false } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Tree) {
            Preconditions   = { { "hasLogs", false } },
            Effects         = { { "hasLogs", true } }
        }); 
        
        // AddAction(new GetResource(_game, _movement, TypeOfThing.Tree));
        // AddAction(new GetResource(_game, _movement, TypeOfThing.Stone));
    
        // AddAction(new ConvertResource(_game, _movement, TypeOfThing.Tree));
        // AddAction(new ConvertResource(_game, _movement, TypeOfThing.Stone));

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

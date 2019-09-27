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
        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _world.Add("isIdle", false);
        _goal.Add("isIdle", true);
    }

    public override void Setup()
    {
        base.Setup();
        
        _thing.name = NameGenerator.GenerateCharacterName();

        AddAction(new Idle(_game, _movement)); 

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

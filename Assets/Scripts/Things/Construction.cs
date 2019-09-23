using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction 
{   
    private Game _game;
    private Thing _thing;
    private TypeOfThing _buildOn;
    private TypeOfThing _builds;
    public Construction(Game game, Thing thing, TypeOfThing buildOn, TypeOfThing builds)
    {
        _buildOn = buildOn;
        _builds = builds;
        _thing = thing;
        _game = game;
    }

    public void Construct()
    {
        var thing = _game.Create(_builds, _thing.gridPosition.x, _thing.gridPosition.y);
        _game.AddThing(thing);
    }
}

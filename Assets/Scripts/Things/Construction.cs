using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConstructionGroup
{
    Floors,
    Walls,
    Objects
}

public class Construction 
{   
    public ConstructionGroup Group { get { return _group; } }
    public TypeOfThing BuildType { get { return _builds; } }
    private Game _game;
    private Thing _thing;
    private TypeOfThing _buildOn;
    private TypeOfThing _builds;    
    private ConstructionGroup _group;

    public Construction(Game game, Thing thing, TypeOfThing buildOn, TypeOfThing builds, ConstructionGroup group)
    {
        _buildOn = buildOn;
        _builds = builds;
        _thing = thing;
        _game = game;
        _group = group;
    }

    public void Construct()
    {
        var thing = _game.Create(_builds, _thing.gridPosition.x, _thing.gridPosition.y);
        _game.AddThing(thing);
    }

    public static string GetGroupSprite(ConstructionGroup group)
    {
        switch(group)
        {
            case ConstructionGroup.Floors:
                return "colored_transparent_15";
            case ConstructionGroup.Objects:
                return "colored_transparent_15";
            case ConstructionGroup.Walls:
                return "colored_transparent_15";
        }

        return "none";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    bool ConstructAtPosition(Vector2Int position)
    {
        return _game.Things.Any(t => t.construction != null && t.gridPosition == position);
    }

    public bool IsPlaceableAt(int x, int y)
    {
        var current = _game.GetThingOnGrid(x, y);
        if(current == null)
            return false;
        if(ConstructAtPosition(current.gridPosition))
            return false;
        return current.floor && current.buildOn;
    }

    public void Construct()
    {
        var thing = _game.Create(_builds, _thing.gridPosition.x, _thing.gridPosition.y);
        _game.AddThing(thing);
        _thing.Destroy();
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

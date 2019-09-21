using System;
using UnityEngine;

public class TileRuleDefinition : ITileRule
{
    private Sprites _start;

    private Position[] _positions = new Position[]
    {
        // ends
        Position.None,
        Position.Top,
        Position.Bottom,
        Position.Left,
        Position.Right,

        // corners
        Position.Top | Position.Left,
        Position.Top | Position.Right,
        Position.Bottom | Position.Right,
        Position.Bottom | Position.Left,

        // horizontal edgges
        Position.Bottom | Position.Right | Position.Top,
        Position.Bottom | Position.Left | Position.Top,

        // vertical edges
        Position.Left | Position.Right | Position.Top,
        Position.Left | Position.Right | Position.Bottom,

        // surrounded
        Position.Bottom | Position.Top | Position.Right | Position.Left,

        // horizontal
        Position.Top | Position.Bottom,

        // vertical
        Position.Left | Position.Right,
    };

    public TileRuleDefinition(Sprites start)
    {
        _start = start;
    }

    public Sprites GetSprite(Position position)
    {
        var start = (int)_start;

        for(var i = 1; i < _positions.Length; i++) 
        {
            if(position == _positions[i])
                return (Sprites)(start + i);
        }
        
        return (Sprites)(_start + 1);
    }
}

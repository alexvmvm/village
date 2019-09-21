using System;
using UnityEngine;

public class TileRuleDefinition : ITileRule
{
    private string[] _sprites;

    public TileRuleDefinition(
        string bottomEnd,
        string topEnd,
        string rightEnd,
        string leftEnd,
        string bottomRightCorner,
        string bottomLeftCorner,
        string topLeftCorner,
        string topRightCorner,
        string leftEdge,
        string rightEdge,
        string bottomEdge,
        string topEdge,
        string surrounded,
        string vertical,
        string horizontal
    )
    {
        _sprites = new string[]
        {
            bottomEnd, topEnd, rightEnd, leftEnd,
            bottomRightCorner, bottomLeftCorner, topLeftCorner, topRightCorner, 
            leftEdge, rightEdge, bottomEdge, topEdge,
            surrounded, vertical, horizontal
        };
    }

    private Position[] _positions = new Position[]
    {
        // ends
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

   

    public string GetSprite(Position position)
    {

        for(var i = 0; i < _positions.Length; i++) 
        {
            if(position == _positions[i])
                return _sprites[i];
        }
        
        return _sprites[0];
    }
}

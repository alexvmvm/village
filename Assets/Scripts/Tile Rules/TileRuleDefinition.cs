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

    private Positions[] _positions = new Positions[]
    {
        // ends
        Positions.Top,
        Positions.Bottom,
        Positions.Left,
        Positions.Right,

        // corners
        Positions.Top | Positions.Left,
        Positions.Top | Positions.Right,
        Positions.Bottom | Positions.Right,
        Positions.Bottom | Positions.Left,

        // horizontal edgges
        Positions.Bottom | Positions.Right | Positions.Top,
        Positions.Bottom | Positions.Left | Positions.Top,

        // vertical edges
        Positions.Left | Positions.Right | Positions.Top,
        Positions.Left | Positions.Right | Positions.Bottom,

        // surrounded
        Positions.Bottom | Positions.Top | Positions.Right | Positions.Left,

        // horizontal
        Positions.Top | Positions.Bottom,

        // vertical
        Positions.Left | Positions.Right,
    };

   

    public string GetSprite(Positions position)
    {

        for(var i = 0; i < _positions.Length; i++) 
        {
            if(position == _positions[i])
                return _sprites[i];
        }
        
        return _sprites[0];
    }
}

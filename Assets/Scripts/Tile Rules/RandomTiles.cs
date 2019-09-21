using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTiles : ITileRule
{
    private Sprites[] _sprites;

    public RandomTiles(params Sprites[] sprites)
    {
        _sprites = sprites;
    }

    public Sprites GetSprite(Position position)
    {
        return _sprites[Random.Range(0, _sprites.Length)];
    }
}

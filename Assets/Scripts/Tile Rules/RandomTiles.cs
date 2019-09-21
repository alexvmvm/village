using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTiles : ITileRule
{
    private string[] _sprites;

    public RandomTiles(params string[] sprites)
    {
        _sprites = sprites;
    }

    public string GetSprite(Position position)
    {
        return _sprites[Random.Range(0, _sprites.Length)];
    }
}

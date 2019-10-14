using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTile : ITileRule
{
    private Thing _thing;

    public CropTile(Thing thing)
    {
        _thing = thing;
    }

    public string GetSprite(Position position)
    {
        return _thing.crop.GetSprite();
    }
}

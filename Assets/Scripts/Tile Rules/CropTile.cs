using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTile : ITileRule
{
    private Thing _thing;
    private Crop _crop;

    public CropTile(Thing thing)
    {
        _thing = thing;
        _crop = thing.GetTrait<Crop>();
    }

    public string GetSprite(Position position)
    {
        return _crop.GetSprite();
    }
}

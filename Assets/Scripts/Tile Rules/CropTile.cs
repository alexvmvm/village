using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Village.Things
{
    public class CropTile : ITileRule
    {
        private Thing _thing;
        private Crop _crop;

        public CropTile(Thing thing, Crop crop)
        {
            _thing = thing;
            _crop = crop;
        }

        public string GetSprite(Position position)
        {
            return _crop.GetSprite();
        }
    }
}

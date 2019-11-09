using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Village.Things
{
    public class CropTile : ITileRule
    {
        private Crop _crop;

        public CropTile(Crop crop)
        {
            _crop = crop;
        }

        public string GetSprite(Positions position)
        {
            return _crop.GetSprite();
        }
    }
}

using System.Collections.Generic;
using System;
using UnityEngine;

namespace Village.Saving
{
    [Serializable]
    public class GameSave
    {
        public Vector2Int Size;
        public ThingSave[] Things;

        public GameSave()
        {

        }
    }
}
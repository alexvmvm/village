using UnityEngine;
using System;

namespace Village.Saving
{
    [Serializable]
    public class ThingSave 
    {
        public string id;
        public TypeOfThing type;
        public Vector3 position;
        public string ownedBy;
        public int hitpoints;
        public TypeOfThing builds;

        public ThingSave() {}
    }
}

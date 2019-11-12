using System.Collections;
using System.Collections.Generic;
using SwordGC.AI.Goap;
using UnityEngine;

namespace Village.AI.V2
{
    public class SpawnVillagers : GoapGoal
    {
        public SpawnVillagers() : base(GoapGoal.Goals.SPAWN_VILLAGERS)
        {
        }
    }
}

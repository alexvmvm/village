using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Village.AI 
{

    public class SpawnVillager : GOAPAction
    {

        public SpawnVillager(Game game) : base(game)
        {
        }

        int VillagerCount()
        {
            return _game.QueryThings().Count(t => t.Config.TypeOfThing == TypeOfThing.Villager);
        }

        public override bool IsDone()
        {
            return VillagerCount() > 0;
        }

        public override bool IsPossibleToPerform()
        {
            return VillagerCount() == 0 && Time.timeSinceLevelLoad > 5;
        }

        public override bool Perform()
        {
            _game.CreateAndAddThing(TypeOfThing.Villager, 
                    Mathf.FloorToInt(_game.Size.x / 2), 
                    UnityEngine.Random.Range(0, _game.Size.y));

            return true;
        }

        public override void Reset()
        {
            
        }
    }

}

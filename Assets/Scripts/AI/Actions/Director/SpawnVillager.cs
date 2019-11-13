using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Village.AI 
{

    public class SpawnVillager : GOAPAction
    {

        public SpawnVillager(Agent agent, Game game) : base(agent, game)
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
            var position = new Vector2Int(Mathf.FloorToInt(_game.Size.x / 2), 
                    UnityEngine.Random.Range(0, _game.Size.y));

            _game.CreateAtPosition(TypeOfThing.Villager, position);

            return true;
        }

        public override void Reset()
        {
            
        }
    }

}

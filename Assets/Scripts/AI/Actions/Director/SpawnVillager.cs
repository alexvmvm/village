using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Village.AI 
{

    public class SpawnVillager : GOAPAction
    {

        public SpawnVillager(GOAPAgent agent, Game game) : base(agent, game)
        {
            Goal = GOAPGoal.Goal.IS_WORKING;

            Preconditions.Add(GOAPAction.Effect.IS_WORKING, false);
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
            return VillagerCount() == 0 && _game.TimeSinceLoaded > 5;
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SwordGC.AI.Goap;
using UnityEngine;

namespace Village.AI.V2
{
    public class SpawnVillager : GoapAction
    {
        private Game _game;

        public SpawnVillager(GoapAgent agent, Game game) : base(agent)
        {
            _game = game;
            goal = GoapGoal.Goals.SPAWN_VILLAGERS;
        }

        int GetVillagerCount()
        {
            return _game.QueryThings().Count(t => t.Config.TypeOfThing == TypeOfThing.Villager);
        }

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            target = agent.gameObject;
            return GetVillagerCount() == 0 && Time.timeSinceLevelLoad > 5;
        }

        public override void Perform()
        {
            _game.CreateAndAddThing(TypeOfThing.Villager, 
                Mathf.FloorToInt(_game.Size.x / 2), 
                UnityEngine.Random.Range(0, _game.Size.y));
            
            agent.RemoveAction(this);
        }

        public override GoapAction Clone()
        {
            return new SpawnVillager(agent, _game);
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public class LeaveVillage : MoveGOAPAction
    {
        private Thing _thing;
        private VillageManager _villageManager;
        private Villager _villager;

        public LeaveVillage(Agent agent, Game game, Movement movement, Thing thing, Villager villager) : base(agent, game, movement)
        {
            _thing = thing;
            _villager = villager;
            _villageManager = MonoBehaviour.FindObjectOfType<VillageManager>();
        }

        public override void BeforeStartMoving()
        {
            if (_villageManager != null)
                _villageManager.TriggerEvent(VillagerEvent.VillagerLeft, _villager);
        }

        public override bool Filter(Thing thing)
        {
            return thing.Config.TypeOfThing == TypeOfThing.Path;
        }

        public override bool PerformAtTarget()
        {
            _game.Remove(_thing);
            return true;
        }
    }

}

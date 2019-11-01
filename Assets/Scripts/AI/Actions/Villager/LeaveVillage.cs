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

        public LeaveVillage(Game game, Movement movement, Thing thing, Villager villager) : base(game, movement)
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

        public override IEnumerable<Thing> GetThings()
        {
            return _game.QueryThings()
                .Where(t => t.type == TypeOfThing.Path)
                .OrderBy(t => t.transform.position.y);
        }

        public override bool PerformAtTarget()
        {
            _game.Destroy(_thing);
            return true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;
using Village.Things.Config;

namespace Village.AI
{

    public class Construct : MoveGOAPAction
    {
        private Movement _movement;
        private bool _started;
        private TypeOfThing _resource;
        private Thing _thing;
        private Inventory _inventory;
        private InventorySlot _resourceSlot;

        public Construct(Agent agent, Game game, Movement movement, TypeOfThing resource, Thing thing) : base(agent, game, movement)
        {
            _movement = movement;
            _resource = resource;
            _thing = thing;
            _inventory = _thing.Inventory;

            _resourceSlot = Assets.GetThingConfig(resource).InventorySlot;

            if(resource != TypeOfThing.None)
                Preconditions.Add(GOAPAction.Effect.HAS_THING + _resourceSlot, resource);

            Effects.Add(GOAPAction.Effect.IS_WORKING, true);
        }

        public override bool Filter(Thing thing)
        {
            return thing != null && thing.Config.TypeOfThing == TypeOfThing.Blueprint && thing.Requires == _resource;
        }

        public override bool PerformAtTarget()
        {
            if(_target == null)
                return true;

            if (_inventory.IsHoldingThing(_resourceSlot))
            {
                var resource = _inventory.GetHoldingThing(_resourceSlot);
                resource.Hitpoints -= 1;

                if (resource.Hitpoints == 0)
                {
                    _game.Remove(_inventory.Drop(_resourceSlot));
                }
            }

            _target.Construct();

            return true;
        }

        public override void Reset()
        {
            base.Reset();

            _started = false;
        }

        public override string ToString()
        {
            return $"ConstructBlueprint(required {_resource.ToString()})";
        }
    }

}

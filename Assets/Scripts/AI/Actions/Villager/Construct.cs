using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public class Construct : MoveGOAPAction
    {
        private Movement _movement;
        private bool _started;
        private bool _isDone;
        private TypeOfThing _resource;
        private Thing _thing;
        private Inventory _inventory;

        public Construct(Agent agent, Game game, Movement movement, TypeOfThing resource, Thing thing) : base(agent, game, movement)
        {
            _movement = movement;
            _resource = resource;
            _thing = thing;
            _inventory = _thing.Inventory;
        }

        public override bool Filter(Thing thing)
        {
            return thing.Requires == _resource;
        }

        public override TypeOfThing GetThingType()
        {
            return TypeOfThing.Blueprint;
        }

        public override bool IsDone()
        {
            return _isDone;
        }


        public override bool PerformAtTarget()
        {
            
            if(_target == null)
                return false;

            if (_inventory.IsHoldingSomething() && !_inventory.IsHoldingTool())
            {
                var resource = _inventory.Holding;
                resource.Hitpoints -= 1;

                if (resource.Hitpoints == 0)
                {
                    _inventory.Drop();
                }
            }

            _target.Construct();

            return true;
        }

        public override void Reset()
        {
            base.Reset();

            _started = false;
            _isDone = false;
        }
    }

}

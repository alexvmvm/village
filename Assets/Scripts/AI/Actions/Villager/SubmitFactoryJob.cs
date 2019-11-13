using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public class SubmitFactoryJob : MoveGOAPAction
    {

        private Thing _thing;
        private Movement _movement;
        private Inventory _inventory;
        private TypeOfThing _factoryType;
        private TypeOfThing _output;
        private bool _requiresAgentToMake;
        private bool _submittedJob;

        public SubmitFactoryJob(Game game, Thing thing, Movement movement, TypeOfThing factoryType, TypeOfThing output, bool requiresAgentToMake) : base(game, movement)
        {
            _thing = thing;
            _movement = movement;
            _factoryType = factoryType;
            _inventory = _thing.Inventory;
            _output = output;
            _requiresAgentToMake = requiresAgentToMake;
        }

        public override bool Filter(Thing thing)
        {
            return thing.Factory.IsProducing() && thing.Factory.IsQueuedForProduction(_output);
        }

        public override TypeOfThing GetThingType()
        {
            return TypeOfThing.ForagedWall;
        }

        public override bool PerformAtTarget()
        {
            if (_inventory.IsHoldingSomething())
            {
                _game.Destroy(_inventory.Holding);
                _inventory.Drop();
            }

            if (!_submittedJob)
            {
                _target.Factory.Craft(_output);
                _submittedJob = true;
            }

            if (_requiresAgentToMake && _target.Factory.IsProducing())
                return false;


            return true;
        }

        public override void Reset()
        {
            base.Reset();

            _submittedJob = false;
        }
    }

}

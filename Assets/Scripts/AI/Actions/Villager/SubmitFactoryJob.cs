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
            _inventory = _thing.GetTrait<Inventory>();
            _output = output;
            _requiresAgentToMake = requiresAgentToMake;
        }

        public override IEnumerable<Thing> GetThings()
        {
            return _game.QueryThings()
                .Where(t => t.type == _factoryType && !t.GetTrait<Factory>().IsProducing() && t.GetTrait<Factory>().IsQueuedForProduction(_output))
                .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
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
                _target.GetTrait<Factory>().Craft(_output);
                _submittedJob = true;
            }

            if (_requiresAgentToMake && _target.GetTrait<Factory>().IsProducing())
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

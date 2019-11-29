using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;
using Village.Things.Config;

namespace Village.AI
{

    public class SubmitFactoryJob : MoveGOAPAction
    {

        private Thing _thing;
        private Movement _movement;
        private Inventory _inventory;
        private TypeOfThing _factoryType;
        private TypeOfThing _input;
        private TypeOfThing _output;
        private bool _requiresAgentToMake;
        private bool _submittedJob;
        private InventorySlot _inputSlot;

        public SubmitFactoryJob(GOAPAgent agent, Game game, Thing thing, Movement movement, TypeOfThing factoryType, TypeOfThing input, TypeOfThing output, bool requiresAgentToMake) : base(agent, game, movement)
        {
            _thing = thing;
            _movement = movement;
            _factoryType = factoryType;
            _inventory = _thing.Inventory;
            _output = output;
            _input = input;
            _requiresAgentToMake = requiresAgentToMake;

            var inputConfig = Assets.GetThingConfig(input);
            _inputSlot = inputConfig.InventorySlot;

            var outputConfig = Assets.GetThingConfig(output);

            // Preconditions.Add(GOAPAction.Effect.HAS_THING + inputConfig.InventorySlot, _input);
            // Effects.Add(GOAPAction.Effect.HAS_THING + outputConfig.InventorySlot, _output);
            // Effects.Add(GOAPAction.Effect.IS_WORKING, true);
        }

        public override bool Filter(Thing thing)
        {
            return 
                thing != null && 
                thing.Config.TypeOfThing == _factoryType &&
                !thing.Factory.IsProducing() && 
                thing.Factory.IsQueuedForProduction(_output);
        }

        public override bool PerformAtTarget()
        {
            if (_inventory.IsHoldingThing(_inputSlot))
            {
                _game.Remove(_inventory.Drop(_inputSlot));
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

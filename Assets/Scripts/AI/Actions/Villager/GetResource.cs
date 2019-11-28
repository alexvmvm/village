using UnityEngine;
using Village.Things;
using Village.Things.Config;

namespace Village.AI
{
    public class GetThing : MoveGOAPAction
    {
        private Thing _thing;
        protected Movement _movement;
        protected TypeOfThing _resource;
        private Inventory _inventory;
        protected Villager _villager;

        public GetThing(GOAPAgent agent, Game game, Thing thing, Movement movement, TypeOfThing resource, Villager villager) : base(agent, game, movement)
        {
            _thing = thing;
            _movement = movement;
            _resource = resource;
            _inventory = _thing.Inventory;
            _villager = villager;

            var resourceConfig = Assets.GetThingConfig(_resource);    
            var producesConfig = Assets.GetThingConfig(resourceConfig.Produces);

            if(resourceConfig.RequiredToProduce != TypeOfThing.None)
            {
                var requiresConfig = Assets.GetThingConfig(resourceConfig.RequiredToProduce);
                Preconditions.Add(GOAPAction.Effect.HAS_THING + requiresConfig.InventorySlot, requiresConfig.TypeOfThing);
            }

            Preconditions.Add(GOAPAction.Effect.IS_HOLDING_THING + producesConfig.InventorySlot, false);

            //Effects.Add(GOAPAction.Effect.IS_HOLDING_THING + producesConfig.InventorySlot, true);
            Effects.Add(GOAPAction.Effect.HAS_THING + producesConfig.InventorySlot, producesConfig.TypeOfThing);

            // costs more if not a straight resource
            // pickup
            Cost = resourceConfig.Produces == resourceConfig.TypeOfThing ? 1 : 2;
        }

        public override bool Filter(Thing thing)
        {
            return thing != null && thing.Config.TypeOfThing == _resource;
        }

        public override bool PerformAtTarget()
        {
            if (_target.Config.FixedToFloor)
            {
                var resource = _game.CreateAtPosition(_target.Config.Produces, Vector2Int.zero);
                resource.Hitpoints = Mathf.Min(10, _target.Hitpoints);
                resource.ownedBy = _villager.Fullname;
                _inventory.Hold(resource);

                // damage existing resource
                _target.Hitpoints -= 10;
                if (_target.Hitpoints <= 0 && _target.transform != null)
                    _game.CreateAtPosition(TypeOfThing.MudFloor, _target.Position);
            }
            else
            {
                _target.ownedBy = _villager.Fullname;
                _inventory.Hold(_target);
            }

            return true;
        }

        public override void Reset()
        {
            base.Reset();

            _target = null;
        }

        public override string ToString() 
        {
            return $"{GetType().ToString()}({_resource})";
        }

    }

    

}

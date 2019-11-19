using UnityEngine;
using Village.Things;

namespace Village.AI
{
    public class GetThing : MoveGOAPAction
    {
        private Thing _thing;
        protected Movement _movement;
        protected TypeOfThing _resource;
        private Inventory _inventory;
        protected Villager _villager;

        public GetThing(Agent agent, Game game, Thing thing, Movement movement, TypeOfThing resource, Villager villager) : base(agent, game, movement)
        {
            _thing = thing;
            _movement = movement;
            _resource = resource;
            _inventory = _thing.Inventory;
            _villager = villager;

            var config = Assets.GetThingConfig(_resource);    
            if(config.RequiredToProduce != TypeOfThing.None)
                Preconditions.Add(GOAPAction.Effect.HAS_THING, config.RequiredToProduce);
            else
                Preconditions.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, false);

            Effects.Add(GOAPAction.Effect.HAS_THING, config.Produces);
            Effects.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, true);
            Effects.Add(GOAPAction.Effect.HAS_EDIBLE_THING, config.Edible);

            // costs more if not a straight resource
            // pickup
            Cost = config.Produces == config.TypeOfThing ? 1 : 2;
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
                _inventory.HoldThing(resource);

                // damage existing resource
                _target.Hitpoints -= 10;
                if (_target.Hitpoints <= 0 && _target.transform != null)
                    _game.CreateAtPosition(TypeOfThing.MudFloor, _target.Position);
            }
            else
            {
                _target.ownedBy = _villager.Fullname;
                _inventory.HoldThing(_target);
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

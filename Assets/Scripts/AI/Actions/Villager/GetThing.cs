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

        public GetThing(GOAPAgent agent, Game game, Thing thing, Movement movement, TypeOfThing resource) : base(agent, game, movement)
        {
            _thing = thing;
            _movement = movement;
            _resource = resource;
            _inventory = _thing.Inventory;

            Preconditions.Add(Effect.HAS_THING, TypeOfThing.None);
            Effects.Add(Effect.HAS_THING, resource);
        }

        public override bool Filter(Thing thing)
        {
            return thing != null && thing.Config.TypeOfThing == _resource;
        }

        public override bool PerformAtTarget()
        {
            if (_target.Config.FixedToFloor)
            {
                var resource = _game.CreateAtPosition(_resource, Vector2Int.zero);
                resource.Hitpoints = Mathf.Min(10, _target.Hitpoints);
                //resource.ownedBy = _villager.Fullname;
                _inventory.Hold(resource);

                // damage existing resource
                _target.Hitpoints -= 10;
                if (_target.Hitpoints <= 0 && _target.transform != null)
                    _game.CreateAtPosition(TypeOfThing.MudFloor, _target.Position);
            }
            else
            {
                //_target.ownedBy = _villager.Fullname;
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

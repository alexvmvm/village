using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{
    public class GetResource : MoveGOAPAction
    {
        private Thing _thing;
        protected Movement _movement;
        protected TypeOfThing _type;
        private Inventory _inventory;
        protected Villager _villager;

        public GetResource(Game game, Thing thing, Movement movement, TypeOfThing type, Villager villager) : base(game, movement)
        {
            _thing = thing;
            _movement = movement;
            _type = type;
            _inventory = _thing.Inventory;
            _villager = villager;
        }

        public override IEnumerable<Thing> GetThings()
        {
            return _game.QueryThings()
                .Where(t => t.Config.TypeOfThing == _type && (string.IsNullOrEmpty(t.ownedBy) || t.ownedBy == _villager.Fullname))
                .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
        }

        public override bool PerformAtTarget()
        {
            if (_target.Config.FixedToFloor)
            {
                var resource = _game.CreateAndAddThing(_target.Config.Produces, 0, 0);
                resource.Hitpoints = Mathf.Min(10, _target.Hitpoints);
                resource.ownedBy = _villager.Fullname;
                _inventory.HoldThing(resource);

                // damage existing resource
                _target.Hitpoints -= 10;
                if (_target.Hitpoints <= 0 && _target.transform != null)
                    _game.CreateAndAddThing(TypeOfThing.MudFloor, _target.Position.x, _target.Position.y);
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
    }

    

}

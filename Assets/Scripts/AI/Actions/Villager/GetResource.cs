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
        private Movement _movement;
        private TypeOfThing _type;
        private Inventory _inventory;
        private Villager _villager;

        public GetResource(Game game, Thing thing, Movement movement, TypeOfThing type, Villager villager) : base(game, movement)
        {
            _thing = thing;
            _movement = movement;
            _type = type;
            _inventory = _thing.GetTrait<Inventory>();
            _villager = villager;
        }

        public override IEnumerable<Thing> GetThings()
        {
            return _game.QueryThings()
                .Where(t => t.type == _type && (string.IsNullOrEmpty(t.ownedBy) || t.ownedBy == _villager.Fullname))
                .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
        }

        public override bool PerformAtTarget()
        {

            if (_target.fixedToGrid)
            {
     
                var resource = _game.CreateAndAddThing(_target.produces, 0, 0);
                resource.hitpoints = Mathf.Min(10, _target.hitpoints);
                resource.ownedBy = _villager.Fullname;
                _inventory.HoldThing(resource);

                // damage existing resource
                _target.hitpoints -= 10;
                if (_target.hitpoints <= 0)
                    _game.CreateAndAddThing(TypeOfThing.MudFloor, _target.gridPosition.x, _target.gridPosition.y);
            }
            else
            {
                _target.ownedBy = _villager.Fullname;
                _inventory.HoldThing(_target);
            }

            return true;
        }
    }

}

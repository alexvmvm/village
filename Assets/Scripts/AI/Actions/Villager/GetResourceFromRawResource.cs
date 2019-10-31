using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public class GetResourceFromRawResource : MoveGOAPAction
    {
        private Movement _movement;
        private TypeOfThing _type;
        private Inventory _inventory;

        public GetResourceFromRawResource(Game game, Movement movement, TypeOfThing type, Inventory inventory) : base(game, movement)
        {
            _movement = movement;
            _type = type;
            _inventory = inventory;
        }

        TypeOfThing RawResourceToProcessed(TypeOfThing rawResource)
        {
            switch (rawResource)
            {
                case TypeOfThing.Tree:
                    return TypeOfThing.Wood;
                case TypeOfThing.Rock:
                    return TypeOfThing.Stone;
                case TypeOfThing.Mushroom:
                    return TypeOfThing.Mushroom;
                default:
                    throw new System.Exception(string.Format("Unknown raw resource conversion {0}", rawResource));
            }
        }

        public override IEnumerable<Thing> GetThings()
        {
            return _game.Things
                .Where(t => t.type == _type)
                .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
        }

        public override bool PerformAtTarget()
        {
            // get resource to hold
            var processedResourceType = RawResourceToProcessed(_type);
            var resource = _game.CreateAndAddThing(processedResourceType, 0, 0);
            resource.hitpoints = 10;
            _inventory.HoldThing(resource);

            // damage existing resource
            _target.hitpoints -= 10;
            if (_target.hitpoints <= 0)
                _game.CreateAndAddThing(TypeOfThing.Grass, _target.gridPosition.x, _target.gridPosition.y);

            return true;
        }

        public override string ToString()
        {
            return $"Getting {RawResourceToProcessed(_type).ToString()}";
        }
    }

}

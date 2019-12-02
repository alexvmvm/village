using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{
    public class GetClayFromGround : MoveGOAPAction
    {
        private Inventory _inventory;

        public GetClayFromGround(GOAPAgent agent, Game game, Movement movement) : base(agent, game, movement)
        {            
            Effects.Add(Effect.HAS_THING, TypeOfThing.Clay);

            _inventory = agent.GetComponent<Inventory>();
        }

        public override bool Filter(Thing thing)
        {
            return thing.Config.TypeOfThing == TypeOfThing.ClayFloor;
        }

        public override bool PerformAtTarget()
        {
            if(_target == null)
                return false;
            
            _target.Hitpoints -= 10;
            if (_target.Hitpoints <= 0 && _target.transform != null)
                _game.CreateAtPosition(TypeOfThing.MudFloor, _target.Position);

            var resource = _game.CreateAtPosition(TypeOfThing.Wood, Vector2Int.zero);
            _inventory.Hold(resource);

            return true;
        }
    }
}

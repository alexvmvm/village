using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public class GetThing : GoapAction
    {
        private Game _game;
        private Thing _thing;
        private TypeOfThing _type;
        private TypeOfThing _produces;
        private Thing _target;
        private Inventory _inventory;

        public GetThing(GoapAgent agent, Thing thing, Game game, TypeOfThing type, TypeOfThing produces) : base(agent)
        {
            preconditions.Add(Effects.EMPTY_INVENTORY, true);
            effects.Add($"{Effects.HAS_THING}_{produces}", true);
            requiredRange = 1.2f;
            removeWhenTargetless = true;
            
            _game = game;
            _thing = thing;
            _type = type;
            _produces = produces;
            _inventory = thing.Inventory;
        }   

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            _target = _game.IsPathPossibleToThing(_thing.Position, _type, (thing) => true);
            if(_target == null)
                return false;
            target = _target.gameObject;
            return true;
        }

        public override void Perform() 
        {
            if (_target.Config.FixedToFloor)
            {
                var resource = _game.CreateAndAddThing(_target.Config.Produces, 0, 0);
                resource.Hitpoints = Mathf.Min(10, _target.Hitpoints);
                //resource.ownedBy = _villager.Fullname;
                _inventory.HoldThing(resource);

                // damage existing resource
                _target.Hitpoints -= 10;
                if (_target.Hitpoints <= 0 && _target.transform != null)
                    _game.CreateAndAddThing(TypeOfThing.MudFloor, _target.Position.x, _target.Position.y);
            }
            else
            {
                //_target.ownedBy = _villager.Fullname;
                _inventory.HoldThing(_target);
            }
        }

        public override GoapAction Clone()
        {
            return new GetThing(agent, _thing, _game, _type, _produces);
        }
    }
}

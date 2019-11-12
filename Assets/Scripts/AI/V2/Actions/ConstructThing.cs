using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public class ConstructThing : GoapAction
    {
        private Game _game;
        private Thing _thing;
        private TypeOfThing _resource;
        private Thing _target;
        private Inventory _inventory;

        public ConstructThing(GoapAgent agent, Thing thing, Game game, TypeOfThing resource) : base(agent)
        {
            preconditions.Add($"{Effects.HAS_THING}_{resource}", true);

            goal = GoapGoal.Goals.CONSTRUCT;
            requiredRange = 1.2f;
            removeWhenTargetless = true;

            _game = game;
            _thing = thing;
            _resource = resource;
            _inventory = thing.Inventory;
        }   

        public override void Perform() 
        {
            if(_target != null)
            {
                _target.Construct();

                if (_inventory.IsHoldingSomething() && !_inventory.IsHoldingTool())
                {
                    var resource = _inventory.Holding;
                    resource.Hitpoints -= 1;

                    if (resource.Hitpoints == 0)
                    {
                        _inventory.Drop();
                        _game.Destroy(resource);
                    }
                }
            }
        }

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            _target = _game.IsPathPossibleToThing(_thing.Position, TypeOfThing.Blueprint, (thing) => true);
            if(_target == null)
                return false;
            target = _target.gameObject;
            return true;
        }

    

        public override GoapAction Clone()
        {
            return new ConstructThing(agent, _thing, _game, _resource);
        }
    }
}

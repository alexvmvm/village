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
        private Thing _target;

        public GetThing(GoapAgent agent, Thing thing, Game game, TypeOfThing type) : base(agent)
        {
            preconditions.Add(Effects.EMPTY_INVENTORY, true);
            effects.Add($"{Effects.HAS_THING}_{type}", true);
            
            _game = game;
            _thing = thing;
            _type = type;
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
            _thing.Inventory.Drop();
            _thing.Inventory.HoldThing(_target);
        }

        public override GoapAction Clone()
        {
            return new GetThing(agent, _thing, _game, _type);
        }
    }
}

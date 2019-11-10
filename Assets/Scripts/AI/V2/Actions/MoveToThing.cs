using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public abstract class MoveToThing : GoapAction
    {
        protected Thing _thing;
        protected Game _game;
        protected Thing _target;

        public MoveToThing(GoapAgent agent, Thing thing, Game game) : base(agent)
        {
            //preconditions.Add(Effects.EMPTY_INVENTORY, false);
            //effects.Add($"{Effects.HAS_THING}_{GetTargetType()}", true);

            cost = 10;
            requiredRange = 1f;
            _game = game;
            _thing = thing;
        }

        public abstract bool FilterThings(Thing thing);
        public abstract TypeOfThing GetTargetType();

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            //data.SetData(Effects.EMPTY_INVENTORY, _thing.Inventory.IsHoldingSomething());

            _target = _game.IsPathPossibleToThing(_thing.Position, GetTargetType(), FilterThings);
            if(_target == null)
                return false;
            target = _target.gameObject;
            return true;
        }
    }
}

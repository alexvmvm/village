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

        public ConstructThing(GoapAgent agent, Thing thing, Game game, TypeOfThing resource) : base(agent)
        {
            preconditions.Add($"{Effects.HAS_THING}_{resource}", true);
            goal = GoapGoal.Goals.CONSTRUCT;

            _game = game;
            _thing = thing;
            _resource = resource;
        }   

        public override void Perform() 
        {
            _target.Construct();
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

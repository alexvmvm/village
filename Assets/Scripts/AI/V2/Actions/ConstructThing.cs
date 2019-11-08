using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;

namespace Village.AI.V2
{
    public class ConstructThing : GoapAction
    {
        private TypeOfThing _type;

        public ConstructThing(GoapAgent agent, TypeOfThing type) : base(agent)
        {
            goal = GoapGoal.Goals.CONSTRUCT;
            preconditions.Add($"{Effects.HAS_THING}_{type}", true);
            cost = 10;
            requiredRange = 1f;
            _type = type;
        }

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            return true;
        }

        public override void Perform() 
        {

        }

        public override GoapAction Clone() 
        {
            return new ConstructThing(agent, _type);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;

namespace Village.AI.V2
{
    public class GetThing : GoapAction
    {
        private TypeOfThing _type;

        public GetThing(GoapAgent agent, TypeOfThing type) : base(agent)
        {
            preconditions.Add(Effects.EMPTY_INVENTORY, true);
            effects.Add($"{Effects.HAS_THING}_{type}", true);
            cost = 10;
            requiredRange = 1f;
            _type = type;
        }

        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            data.SetData(Effects.EMPTY_INVENTORY, true);

            return true;
        }

        public override void Perform() 
        {

        }

        public override GoapAction Clone()
        {
            return new GetThing(agent, _type);
        }
    }
}

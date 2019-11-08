using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;

namespace Village.AI.V2
{
    public class DropThing : GoapAction
    {
        public DropThing(GoapAgent agent) : base(agent)
        {
            preconditions.Add(Effects.EMPTY_INVENTORY, false);
            effects.Add(Effects.EMPTY_INVENTORY, true);
            cost = 0;
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
            return new DropThing(agent);
        }
    }
}

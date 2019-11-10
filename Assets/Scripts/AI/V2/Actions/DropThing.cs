using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public class DropThing : GoapAction
    {
        private Inventory _inventory;

        public DropThing(GoapAgent agent, Inventory inventory) : base(agent)
        {
            _inventory = inventory;
            
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
            _inventory.Drop();
        }

        public override GoapAction Clone()
        {
            return new DropThing(agent, _inventory);
        }
    }
}

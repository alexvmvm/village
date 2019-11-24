using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Village.Things;
using Village.Things.Config;

namespace Village.AI 
{
    public class Drop : GOAPAction
    {
        private Thing _thing;
        private Inventory _inventory;

        public Drop(Agent agent, Game game, Thing thing, InventorySlot slot) : base(agent, game)
        {
            _thing = thing;    
            _inventory = _thing.Inventory;
            
            Preconditions.Add(GOAPAction.Effect.IS_HOLDING_THING + slot, true);
            Effects.Add(GOAPAction.Effect.IS_HOLDING_THING + slot, false);
        }

        public override bool IsDone()
        {
            return !_inventory.IsHoldingThing(InventorySlot.Hands);
        }

        public override bool IsPossibleToPerform()
        {
            return _inventory != null;
        }

        public override bool Perform()
        {
            _inventory.Drop(InventorySlot.Hands);
            return true;
        }

        public override void Reset()
        {
            
        }
    }
}

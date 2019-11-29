using System;
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
        private Inventory _inventory;

        public Drop(GOAPAgent agent, Game game) : base(agent, game)
        {
            _inventory = agent.GetComponent<Inventory>();

            Effects.Add(Effect.HAS_THING, TypeOfThing.None);
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

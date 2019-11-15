﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Village.Things;

namespace Village.AI 
{
    public class Drop : GOAPAction
    {
        private Thing _thing;
        private Inventory _inventory;

        public Drop(Agent agent, Game game, Thing thing) : base(agent, game)
        {
            _thing = thing;    
            _inventory = _thing.Inventory;
            
            Preconditions.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, true);
            Effects.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, false);
        }

        public override bool IsDone()
        {
            return !_inventory.IsHoldingSomething();
        }

        public override bool IsPossibleToPerform()
        {
            return _inventory != null;
        }

        public override bool Perform()
        {
            _inventory.Drop();
            return true;
        }

        public override void Reset()
        {
            
        }
    }
}

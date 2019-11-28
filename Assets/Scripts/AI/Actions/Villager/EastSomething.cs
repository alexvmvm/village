using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;
using Village.Things.Config;

namespace Village.AI
{

    public class EastSomething : GOAPAction
    {
        private Inventory _inventory;

        public EastSomething(GOAPAgent agent, Game game, Thing thing) : base(agent, game)
        {
            _inventory = thing.Inventory;

            Preconditions.Add(GOAPAction.Effect.IS_HUNGRY, true);
            Preconditions.Add(GOAPAction.Effect.HAS_EDIBLE_THING, true);
            Effects.Add(GOAPAction.Effect.IS_HUNGRY, false);
        }

        public override bool IsDone()
        {
            return !_inventory.IsHoldingThing(InventorySlot.Hands);
        }

        public override bool IsPossibleToPerform()
        {
            return true;
        }

        public override bool Perform()
        {
            _game.Remove(_inventory.Drop(InventorySlot.Hands));
            return true;
        }

        public override void Reset()
        {

        }

        public override string ToString()
        {
            if (_inventory.GetHoldingThing(InventorySlot.Hands) == null)
                return base.ToString();
            return $"Eating {_inventory.GetHoldingThing(InventorySlot.Hands).Config.TypeOfThing.ToString()}";
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{

    public class EastSomething : GOAPAction
    {
        private Inventory _inventory;

        public EastSomething(Agent agent, Game game, Thing thing) : base(agent, game)
        {
            _inventory = thing.Inventory;

            Preconditions.Add(GOAPAction.Effect.IS_HUNGRY, true);
            Preconditions.Add(GOAPAction.Effect.HAS_EDIBLE_THING, true);
            Effects.Add(GOAPAction.Effect.IS_HUNGRY, false);
        }

        public override bool IsDone()
        {
            return !_inventory.IsHoldingSomething();
        }

        public override bool IsPossibleToPerform()
        {
            return true;
        }

        public override bool Perform()
        {
            _game.Remove(_inventory.Holding);
            _inventory.Drop();

            return true;
        }

        public override void Reset()
        {

        }

        public override string ToString()
        {
            if (_inventory.Holding == null)
                return base.ToString();
            return $"Eating {_inventory.Holding.Config.TypeOfThing.ToString()}";
        }
    }

}

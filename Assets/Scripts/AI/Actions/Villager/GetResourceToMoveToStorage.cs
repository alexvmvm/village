using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{
    public class GetThingToMoveToStorage : GetThing
    {
        public GetThingToMoveToStorage(Agent agent, Game game, Thing thing, Movement movement, TypeOfThing resource, Villager villager) : base(agent, game, thing, movement, resource, villager)
        {
            Effects.Add(GOAPAction.Effect.HAS_THING_FOR_STORAGE, true);
        }

        public override bool Filter(Thing thing)
        {
            return base.Filter(thing) && thing.Config.Storeable && !thing.IsInStorage();
        }

        public override void Reset()
        {
            base.Reset();
            _target = null;
        }

    }

    

}

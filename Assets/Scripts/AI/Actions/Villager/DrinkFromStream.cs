using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{
    public class DrinkFromStream : MoveGOAPAction
    {
        private Movement _movement;
        private bool _started;
        private bool _isDone;

        public DrinkFromStream(GOAPAgent agent, Game game, Movement movement) : base(agent, game, movement)
        {
            _movement = movement;

            Preconditions.Add(GOAPAction.Effect.IS_THIRSTY, true);
            Effects.Add(GOAPAction.Effect.IS_THIRSTY, false);
        }

        public override bool Filter(Thing thing)
        {
            return thing.Config.TypeOfThing == TypeOfThing.Stream;
        }

        public override bool PerformAtTarget()
        {
            return true;
        }

        public override string ToString()
        {
            return "Drinking from Stream";
        }
    }

}

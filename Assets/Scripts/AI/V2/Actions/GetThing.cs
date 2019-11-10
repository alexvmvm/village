using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public class GetThing : MoveToThing
    {
        private TypeOfThing _type;

        public GetThing(GoapAgent agent, Thing thing, Game game, TypeOfThing type) : base(agent, thing, game)
        {
            //preconditions.Add(Effects.EMPTY_INVENTORY, true);
            preconditions.Add($"{Effects.HAS_THING}_{GetTargetType()}", true);
            // effects.Add($"{Effects.HAS_THING}_{GetTargetType()}", true);

            _type = type;
        }   
        
        public override void Perform() 
        {
            //agent.dataSet.SetData($"{Effects.HAS_THING}_{_type}", true);
            
            Debug.Log("PERFORM");
        }

        public override bool FilterThings(Thing thing)
        {
            return true;
        }

        public override TypeOfThing GetTargetType()
        {
            return _type;
        }

        public override GoapAction Clone()
        {
            return new GetThing(agent, _thing, _game, _type);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordGC.AI.Goap;
using Village.Things;

namespace Village.AI.V2
{
    public class ConstructThing : MoveToThing
    {
        private TypeOfThing _resource;

        public ConstructThing(GoapAgent agent, Thing thing, Game game, TypeOfThing resource) : base(agent, thing, game)
        {
            _resource = resource;

            goal = GoapGoal.Goals.CONSTRUCT;
            preconditions.Add($"{Effects.HAS_THING}_{resource}", true);
        }
        
        public override void Perform() 
        {
            _target.Construct();
            Debug.Log("PERFORM");
        }

        public override bool FilterThings(Thing thing)
        {
            return thing.Config.Construction.Requires == _resource;
        }

        public override TypeOfThing GetTargetType()
        {
            return TypeOfThing.Blueprint;
        }

        public override GoapAction Clone()
        {
            return new ConstructThing(agent, _thing, _game, _resource);
        }
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using SwordGC.AI.Goap;

// namespace Village.AI.V2
// {
//     public class ConstructThing : MoveToThing
//     {
//         private TypeOfThing _type;

//         public ConstructThing(GoapAgent agent, TypeOfThing type) : base(agent)
//         {
//             goal = GoapGoal.Goals.CONSTRUCT;
//             preconditions.Add($"{Effects.HAS_THING}_{type}", true);
//             cost = 10;
//             requiredRange = 1f;
//             _type = type;
//         }

//         protected override bool CheckProceduralPreconditions(DataSet data)
//         {
            

//             return true;
//         }

//         public override void Perform() 
//         {

//         }

//         public override GoapAction Clone() 
//         {
//             return new ConstructThing(agent, _type);
//         }
//     }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using Village.Things;

// namespace Village.AI
// {
//     public class GetResourceToMoveToStorage : MoveGOAPAction
//     {
//         private Thing _thing;
//         protected Movement _movement;

//         public GetResourceToMoveToStorage(Agent agent, Game game, Thing thing, Movement movement) : base(agent, game, movement)
//         {
//             _thing = thing;
//             _movement = movement;

//             Preconditions.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, false);

//             Effects.Add(GOAPAction.Effect.HAS_THING_FOR_STORAGE, true);
//             Effects.Add(GOAPAction.Effect.HAS_FULL_INVENTORY, true);
//         }

        
//         public override TypeOfThing GetThingType()
//         {
//             return _resource;
//         }

//         public override bool Filter(Thing thing)
//         {
//             return thing.Config.Storeable && !thing.IsInStorage();
//         }

//         public override bool PerformAtTarget()
//         {
//             if (_target.Config.FixedToFloor)
//             {
//                 var resource = _game.CreateAtPosition(_target.Config.Produces, Vector2Int.zero);
//                 resource.Hitpoints = Mathf.Min(10, _target.Hitpoints);
//                 resource.ownedBy = _villager.Fullname;
//                 _inventory.HoldThing(resource);

//                 // damage existing resource
//                 _target.Hitpoints -= 10;
//                 if (_target.Hitpoints <= 0 && _target.transform != null)
//                     _game.CreateAtPosition(TypeOfThing.MudFloor, _target.Position);
//             }
//             else
//             {
//                 _target.ownedBy = _villager.Fullname;
//                 _inventory.HoldThing(_target);
//             }

//             return true;
//         }

//         public override void Reset()
//         {
//             base.Reset();

//             _target = null;
//         }

//     }

    

// }

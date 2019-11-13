using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;
using Village.AI;

namespace Village.AI
{
    // public class GetResourceToMoveToStorage : GetResource
    // {
    //     public GetResourceToMoveToStorage(Game game, Thing thing, Movement movement, TypeOfThing type, Villager villager) : base(game, thing, movement, type, villager)
    //     {
    //     }

    //     public override IEnumerable<Thing> GetThings()
    //     {
    //         return _game.QueryThings()
    //             .Where(t => 
    //                 t.Config.TypeOfThing == _type && 
    //                 (string.IsNullOrEmpty(t.ownedBy) || t.ownedBy == _villager.Fullname) &&
    //                 !t.IsInStorage())
    //             .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    //     }
    // }



}

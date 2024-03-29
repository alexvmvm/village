﻿// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SleepAtHome : GOAPAction
// {
//     private Thing _thing;
//     private Movement _movement;
//     private Villager _villager;
//     private bool _started;
//     private Vector3 _target;
//     private Thing _bed;
//     public SleepAtHome(Game game, Thing thing, Movement movement, Villager villager) : base(game)
//     {
//         _thing = thing;
//         _villager = villager;
//         _movement = movement;
//     }

//     public override bool IsDone()
//     {
//         return _game.WorldTime.GetTimeOfDay() == TimeOfDay.Day;
//     }

//     public override bool IsPossibleToPerform()
//     {
//         return _game.WorldTime.GetTimeOfDay() == TimeOfDay.Night;
//     }

//     public override bool Perform()
//     {
//         if(!_started)
//         {

//             _bed = _villager.FamilyChest.GetFreeBedInHouse();
//             if(_bed != null)
//                 _target = _bed.transform.position;
//             else
//                 _target = _villager.FamilyChest.GetRandomPositionInHouse();


//             _movement.CancelCurrentPath();
//             _movement.MoveTo(_target);
//             _started = true;
//         }

//         if(_movement.FailedToFollowPath)
//             return false;

//         if(_movement.ReachedEndOfPath)
//         {
//             switch(_game.WorldTime.GetTimeOfDay())
//             {
//                 case TimeOfDay.Night:
//                     _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
//                 break;
//                 default:
//                     _thing.transform.rotation = Quaternion.Euler(0, 0, 0);
//                 break;
//             }
//         }
        
//         return true;
//     }

//     public override void Reset()
//     {
//         _started = false;
//     }

//     public override string ToString()
//     {
//         return "Sleeping At Home";
//     }
// }

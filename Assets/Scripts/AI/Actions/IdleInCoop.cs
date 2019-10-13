// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class IdleInCoop : GOAPAction
// {
//     private Movement _movement;
//     private Vector3 _target = new Vector3(10, 10, 0);
//     private bool _started;
//     private float _timeToWait;
//     private float _timeWaited;
//     private Animal _animal;

//     public IdleInCoop(Game game, Movement movement, Animal animal) : base(game)
//     {
//         _movement = movement;
//         _animal = animal;
//     }

//     public override bool IsDone()
//     {
//         return _timeWaited > _timeToWait;
//     }

//     public override bool IsPossibleToPerform()
//     {
//         if(_animal.Coop == null)
//             return false;

//         _target = _animal.Coop.coop.GetRandomPositionInCoop();
      

//         if(!_game.IsOnGrid((int)_target.x, (int)_target.y))
//             return false;

//         if(!_movement.IsPathPossible(_target))
//             return false;

//         _timeToWait = Random.Range(0, 4);

//         return true;
//     }

//     public override bool Perform()
//     {
//         if(!_started)
//         {
//             _movement.MoveTo(_target);
//             _started = true;
//         }

//         if(_movement.FailedToFollowPath)
//             return false;

//         if(_movement.ReachedEndOfPath)
//             _timeWaited += Time.deltaTime;
        
//         return true;
//     }

//     public override void Reset()
//     {
//        _started = false;
//        _timeToWait = 0f;
//        _timeWaited = 0f;
//     }
// }

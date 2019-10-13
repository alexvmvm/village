// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;

// public class FamilyChest 
// {
//     private Game _game;
//     private Thing _thing;
//     private List<Thing> _beds;
//     private HashSet<Thing> _seen;
//     private Queue<Thing> _queue;
//     private Color _color;
    
//     public FamilyChest(Game game, Thing thing)
//     {
//         _game = game;
//         _thing = thing;
//         _seen = new HashSet<Thing>();
//         _queue = new Queue<Thing>();
//         _color = ExtensionMethods.RandomColor();

//         _beds = new List<Thing>();
//     }

//     public Vector3 GetRandomPositionInHouse()
//     {
//         return _seen.ToList().Shuffle().FirstOrDefault().gridPosition.ToVector3();
//     }

//     public Thing GetFreeBedInHouse()
//     {
//         return _beds.FirstOrDefault();
//     }

//     public void Update()
//     {
//         _beds.Clear();

//         _seen.Clear();
//         _queue.Clear();

//         _queue.Enqueue(_game.GetThingOnGrid(_thing.gridPosition));

//         while(_queue.Count > 0)
//         {
//             var current = _queue.Dequeue();
            
//             if(_seen.Contains(current))
//                 continue;

//             _seen.Add(current);

//             var neighbours = current.GetNeighboursOnGrid();

//             foreach(var neighbour in neighbours)
//             {
//                 if(neighbour == null)
//                     continue;

//                 switch(neighbour.type)
//                 {
//                     case TypeOfThing.Door:
//                         _queue.Enqueue(neighbour);
//                     break;
//                     case TypeOfThing.Bed:
//                         if(!_beds.Contains(neighbour))
//                             _beds.Add(neighbour);
//                         _queue.Enqueue(neighbour);
//                     break;
//                     default:
//                         if(neighbour.playerBuiltFloor)
//                             _queue.Enqueue(neighbour);
//                     break;
//                 }
//             }
            
//         }
//     }

//     public void DrawGizmos()
//     {
//         Gizmos.color = _color;
//         foreach(var thing in _seen)
//         {
//             Gizmos.DrawWireSphere(thing.gridPosition.ToVector3(), 0.2f);
//         }
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AtmosphereSimultation
// {
//     private Game _main;
//     private HashSet<Thing> _seen;
//     private Queue<Thing> _queue;
//     private List<Thing> _all;
//     private bool _touchesSpace;

//     private Vector2Int[] _neighbours = new Vector2Int[]
//     {
//         Vector2Int.up,
//         Vector2Int.down,
//         Vector2Int.right,
//         Vector2Int.left
//     };

//     public AtmosphereSimultation(Game main)
//     {
//         _main = main;
//         _seen = new HashSet<Thing>();
//         _queue = new Queue<Thing>(); 
//         _all = new List<Thing>();
//     }

//     bool BordersSpace(Thing thing)
//     {
//         foreach(var n in _neighbours)
//         {
//             var neighbour = _main.GetThingOnGrid(thing.gridPosition + n);
//             if(neighbour == null || (!neighbour.wall && !neighbour.floor && neighbour.gurder))
//                 return true;
//         }
//         return false;
//     }

//     void RunSimulationFrom(Thing thing)
//     {
//         if(!thing.floor || _seen.Contains(thing))
//             return;

//         _queue.Clear();
//         _queue.Enqueue(thing);

//         _all.Clear();
//         _all.Add(thing);

//         _touchesSpace = false;

//         while(_queue.Count > 0)
//         {
//             var current = _queue.Dequeue();

//             if(_seen.Contains(current))
//                 continue;
            
//             _seen.Add(current);
//             _all.Add(current);

//             if(BordersSpace(current))
//                 _touchesSpace = true;

//             foreach(var direction in _neighbours)
//             {
//                 var neighbour = _main.GetThingOnGrid(current.gridPosition + direction);
//                 if(neighbour != null && neighbour.floor)
//                     _queue.Enqueue(neighbour);
                    
//             }
//         }

//         // foreach(var seenThing in _all)
//         // {
//         //     seenThing.pressurized = !_touchesSpace;
//         // }
//     }

//     public void Update()
//     {
//         _seen.Clear();

//         for(var i = 0; i < _main.Things.Count; i++) 
//         {
//             var thing = _main.Things[i];
//             if(!thing.floor || _seen.Contains(thing))
//                 continue;
//             RunSimulationFrom(thing);
//         }
//     }
// }

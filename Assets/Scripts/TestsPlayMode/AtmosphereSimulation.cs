// using System.Collections;
// using System.Linq;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// namespace Tests
// {
//     public class AtmosphereSimulation
//     {
//         private Game _game;
//         private GameObject _camera;
        
//         [SetUp]
//         public void Setup()
//         {
//             var obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
//             _game = obj.GetComponent<Game>();

//             _camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Camera"));

//             for(var x = 0; x < _game.MapSize.x; x++)
//             {
//                 for(var y = 0; y < _game.MapSize.y; y++)
//                 {
//                     var thing = _game.Create(x == 0 || y == 0 || x == _game.MapSize.x - 1 || y == _game.MapSize.y - 1 ? 
//                         TypeOfThing.Wall : TypeOfThing.Floor, x, y);
//                     _game.AddThing(thing);
//                 }
//             }
//         }

//         [TearDown]
//         public void Teardown()
//         {
//             Object.Destroy(_game.gameObject);
//             Object.Destroy(_camera.gameObject);
//         }

//         [UnityTest]
//         public IEnumerator AllFloorsShouldBePressurizedIfSurroundedByWall()
//         {
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => t.pressurized));
//         }

        
//         [UnityTest]
//         public IEnumerator AllFloorsShouldNotBePressurizedIfFloorBordersSpace()
//         {
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => t.pressurized));
//             var wall = _game.GetThingOnGrid(0, 1);
//             _game.RemoveThing(wall);
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => !t.pressurized));
//         }

//         [UnityTest]
//         public IEnumerator OnlyRegionWithHoleShouldDePressurize()
//         {
//             for(var y = 0; y < _game.MapSize.y; y++)
//             {
//                 var thing = _game.Create(TypeOfThing.Wall, 5, y);
//                 _game.AddThing(thing);
//             }

//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => t.pressurized));
//             var wall = _game.GetThingOnGrid(0, 1);
//             _game.RemoveThing(wall);
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor && t.gridPosition.x < 5).All(t => !t.pressurized));
//             Assert.IsTrue(_game.Things.Where(t => t.floor && t.gridPosition.x > 5).All(t => t.pressurized));
//         }

//         [UnityTest]
//         public IEnumerator AllFloorsShouldRePressurizeIfWallFilled()
//         {
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => t.pressurized));
//             var wall = _game.GetThingOnGrid(0, 1);
//             _game.RemoveThing(wall);
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => !t.pressurized));
//             var wall2 = _game.Create(TypeOfThing.Wall, 0, 1);
//             _game.AddThing(wall2);
//             yield return null;
//             Assert.IsTrue(_game.Things.Where(t => t.floor).All(t => t.pressurized));
//         }
//     }

// }

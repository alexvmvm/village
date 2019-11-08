// using System.Linq;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using System;
// using Pathfinding;

// namespace Tests
// {
//     public class AgentTests
//     {
//         private GameObject _game;
//         private GameObject _camera;
        
//         [SetUp]
//         public void Setup()
//         {
//             _game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
//             _camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Camera"));
//         }

//         [TearDown]
//         public void Teardown()
//         {
//             UnityEngine.Object.DestroyImmediate(_game.gameObject);
//             UnityEngine.Object.DestroyImmediate(_camera.gameObject);
//         }

//         [UnityTest]
//         public IEnumerator ShouldAddChickensToGame()
//         {
//             var game = _game.GetComponent<Game>();

//             foreach(var chicken in game.Things.Where(t => t.Config.TypeOfThing == TypeOfThing.Hen).ToArray())
//             {
//                 chicken.Destroy();
//                 game.RemoveThing(chicken);
//             }

//             var totalChickens = 100;
//             for(var i = 0; i < totalChickens; i++)
//             {
//                 var x = UnityEngine.Random.Range(0, game._size.x);
//                 var y = UnityEngine.Random.Range(0, game._size.y);

//                 var thing = game.Create(TypeOfThing.Hen, x, y);
//                 game.AddThing(thing);
//             }       

            
//             yield return null;

//             var count = 0;
//             foreach(var chicken in game.Things.Where(t => t.Config.TypeOfThing == TypeOfThing.Hen).ToArray())
//             {
//                 Assert.IsNotNull(chicken);
//                 Assert.IsNotNull(chicken.GetTrait<Agent>());
//                 Assert.IsNotNull(chicken.transform);
//                 Assert.IsNotNull(chicken.transform.gameObject.GetComponent<Movement>());
//                 Assert.IsNotNull(chicken.transform.gameObject.GetComponent<Seeker>());
//                 Assert.IsNotNull(chicken.transform.gameObject.GetComponent<AIPath>());
//                 count++;
//             }

//             Assert.AreEqual(totalChickens, count);
//         }

//                 [UnityTest]
//         public IEnumerator ShouldAddVillagesToGame()
//         {
//             var game = _game.GetComponent<Game>();

//             foreach(var villager in game.Things.Where(t => t.Config.TypeOfThing == TypeOfThing.Villager).ToArray())
//             {
//                 villager.Destroy();
//                 game.RemoveThing(villager);
//             }

//             var totalVillagers = 100;
//             for(var i = 0; i < totalVillagers; i++)
//             {
//                 var x = UnityEngine.Random.Range(0, game._size.x);
//                 var y = UnityEngine.Random.Range(0, game._size.y);

//                 var thing = game.Create(TypeOfThing.Villager, x, y);
//                 game.AddThing(thing);
//             }       

//             yield return null;

//             var count = 0;
//             foreach(var villager in game.Things.Where(t => t.Config.TypeOfThing == TypeOfThing.Villager).ToArray())
//             {
//                 Assert.IsNotNull(villager);
//                 Assert.IsNotNull(villager.GetTrait<Agent>());
//                 Assert.IsNotNull(villager.transform);
//                 Assert.IsNotNull(villager.transform.gameObject.GetComponent<Movement>());
//                 Assert.IsNotNull(villager.transform.gameObject.GetComponent<Seeker>());
//                 Assert.IsNotNull(villager.transform.gameObject.GetComponent<AIPath>());
//                 count++;
//             }

//             Assert.AreEqual(totalVillagers, count);
//         }
//     }
// }

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using Pathfinding;

namespace Tests
{
    public class VillagerTests
    {
        private GameObject _game;
        private GameObject _camera;
        
        [SetUp]
        public void Setup()
        {
            _game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
            _camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Camera"));
        }

        [TearDown]
        public void Teardown()
        {
            UnityEngine.Object.DestroyImmediate(_game.gameObject);
            UnityEngine.Object.DestroyImmediate(_camera.gameObject);
        }


        [UnityTest]
        public IEnumerator ShouldNotTryToGetResourceWherePathIsNotPossible()
        {
            var game = _game.GetComponent<Game>();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.CreateAndAddThing(TypeOfThing.Grass, x, y);
                }
                yield return null;
            }

            game.CreateAndAddThing(TypeOfThing.Villager, 10, 10);

            yield return new WaitForSeconds(10f);

            // foreach(var villager in game.Things.Where(t => t.type == TypeOfThing.Villager).ToArray())
            // {
            //     villager.Destroy();
            //     game.RemoveThing(villager);
            // }

            // var totalVillagers = 100;
            // for(var i = 0; i < totalVillagers; i++)
            // {
            //     var x = UnityEngine.Random.Range(0, game.MapSize.x);
            //     var y = UnityEngine.Random.Range(0, game.MapSize.y);

            //     var thing = game.Create(TypeOfThing.Villager, x, y);
            //     game.AddThing(thing);

            //     yield return null;
            // }       

            // var count = 0;
            // foreach(var villager in game.Things.Where(t => t.type == TypeOfThing.Villager).ToArray())
            // {
            //     Assert.IsNotNull(villager);
            //     Assert.IsNotNull(villager.agent);
            //     Assert.IsNotNull(villager.transform);
            //     Assert.IsNotNull(villager.transform.gameObject.GetComponent<Movement>());
            //     Assert.IsNotNull(villager.transform.gameObject.GetComponent<Seeker>());
            //     Assert.IsNotNull(villager.transform.gameObject.GetComponent<AIPath>());
            //     count++;
            // }

            // Assert.AreEqual(totalVillagers, count);
        }
    }
}

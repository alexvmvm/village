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

            game.MoveToDay();
            
            foreach(var thing in game.Things.Where(t => t.type == TypeOfThing.Chick || t.type == TypeOfThing.Chicken).ToArray())
                thing.Destroy();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.CreateAndAddThing(TypeOfThing.Grass, x, y);
                }
            }

            var villager = game.CreateAndAddThing(TypeOfThing.Villager, 10, 10);
            game.CreateAndAddThing(TypeOfThing.WoodFloorBlueprint, 11, 10);
            game.CreateAndAddThing(TypeOfThing.Tree, 10, 25);

            for(var x = 0; x <= 20; x++)
            {
                for(var y = 0; y <= 20; y++)
                {
                    if(x == 0 || y == 0 || x == 20 || y == 20)
                    {
                        game.CreateAndAddThing(TypeOfThing.StoneWall, x, y);
                    }
                }
            }

            var timeToWait = 5f;
            
            yield return new WaitForSeconds(10f);

            while(timeToWait > 0)
            {
                timeToWait -= Time.deltaTime;
                if(villager.agent.CurentAction != null && !(villager.agent.CurentAction is Idle))
                {
                    Assert.Fail("Villager attempted to get something that was not possible to reach");
                }

                yield return null;
            }
        }
    }
}

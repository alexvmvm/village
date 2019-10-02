using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Tests
{
    public class GameTests
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
        public IEnumerator ShouldAddThingToGrid()
        {
            var game = _game.GetComponent<Game>();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    var thing = game.Create(TypeOfThing.Grass, x, y);
                    game.AddThing(thing);
                }
            }

            yield return null;

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    Assert.IsNotNull(game.GetThingOnGrid(x, y));
                }
            }            
        }

        
        [UnityTest]
        public IEnumerator ShouldRemoveThingsFromGrid()
        {
            var game = _game.GetComponent<Game>();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    var thing = game.Create(TypeOfThing.Grass, x, y);
                    game.AddThing(thing);
                }
            }

            yield return null;

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.RemoveThing(game.GetThingOnGrid(x, y));
                }
            }

            yield return null;

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    Assert.IsNull(game.GetThingOnGrid(x, y));
                }
            }            
        }

        [UnityTest]
        public IEnumerator ShouldBuildThings()
        {
            var game = _game.GetComponent<Game>();

            var things = new List<Thing>();
            foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
            {
                things.Add(game.Create(thingType));
            }

            var placed = new List<Thing>();
            for(var i = 0; i < 50; i++)
            {
                foreach(var thing in things.Where(t => t.construction != null))
                {
                    var x = UnityEngine.Random.Range(0, game.MapSize.x);
                    var y = UnityEngine.Random.Range(0, game.MapSize.y);
                    
                    if(placed.Any(t => t.gridPosition == new Vector2Int(x, y)))
                        continue;

                    var blueprint = game.Create(thing.type, x, y);
                    game.AddThing(blueprint);
                    placed.Add(blueprint);
                }     
            }
            
            yield return null;

            foreach(var thing in placed)
            {
                thing.construction.Construct();
            }

            yield return null;
            
            foreach(var thing in placed)
            {
                var buildType = thing.construction.BuildType;
                var actualType = game.GetThingOnGrid(thing.gridPosition.x, thing.gridPosition.y).type;
                if(buildType != actualType)
                {
                    Debug.Log(string.Format("{0} {1}", buildType, actualType));
                }
                Assert.IsTrue(buildType == actualType);
            }

        }
    }
}

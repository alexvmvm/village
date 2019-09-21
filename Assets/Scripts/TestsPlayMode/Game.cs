using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            Object.Destroy(_game.gameObject);
            Object.Destroy(_camera.gameObject);
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
                yield return null;
            }

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
                yield return null;
            }

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.RemoveThing(game.GetThingOnGrid(x, y));
                }
                yield return null;
            }

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    Assert.IsNull(game.GetThingOnGrid(x, y));
                }
            }            
        }
    }
}

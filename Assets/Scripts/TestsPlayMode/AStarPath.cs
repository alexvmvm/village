using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Tests
{
    public class AStarPathTests
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
        public IEnumerator ShouldUpdatePathIfBlocking()
        {
            var game = _game.GetComponent<Game>();
            var aStartPath = _game.GetComponentInChildren<AstarPath>();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.CreateAndAddThing(TypeOfThing.Grass, x, y);
                }
            }

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    if(x % 2 == 0 && y % 2 == 0)
                    {
                        game.CreateAndAddThing(TypeOfThing.WoodWall, x, y);
                    }
                }
                yield return null;
            }

            var tag = game.TagFromString("blocking");
            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    var graphNode = aStartPath.GetNearest(new Vector3(x, y));
                    if(x % 2 == 0 && y % 2 == 0)
                    {
                        Assert.IsTrue((graphNode.node.Tag & tag) == tag);
                    }
                    else
                    {
                        Assert.IsFalse((graphNode.node.Tag & tag) == tag);
                    }
                }
            }
        }

                [UnityTest]
        public IEnumerator ShouldRemovePathIfDestroyed()
        {
            var game = _game.GetComponent<Game>();
            var aStartPath = _game.GetComponentInChildren<AstarPath>();

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    game.CreateAndAddThing(TypeOfThing.Grass, x, y);
                }
            }

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    if(x % 2 == 0 && y % 2 == 0)
                    {
                        game.CreateAndAddThing(TypeOfThing.WoodWall, x, y);
                    }
                }
                yield return null;
            }

            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    if(x % 2 == 0 && y % 2 == 0)
                    {
                        var thing = game.GetThingOnGrid(x, y);
                        thing.Destroy();
                    }
                }
                yield return null;
            }

            var tag = game.TagFromString("blocking");
            for(var x = 0; x < game.MapSize.x; x++)
            {
                for(var y = 0; y < game.MapSize.y; y++)
                {
                    var graphNode = aStartPath.GetNearest(new Vector3(x, y));
                    Assert.IsFalse((graphNode.node.Tag & tag) == tag);
                }
            }
        }
    }
}

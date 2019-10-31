using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Village;

namespace Tests
{
    public class Main
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ShouldAddThingToGrid()
        {
            var obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Main"));
            var game = obj.GetComponent<Game>();

            var thing = game.Create(TypeOfThing.Grass, 10, 10);
            game.AddThing(thing);

            Assert.IsNotNull(game.GetThingOnGrid(10, 10));

            Object.Destroy(obj);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MainWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}

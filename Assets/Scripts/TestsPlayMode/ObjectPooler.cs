using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Tests
{
    public class ObjectPoolerTests
    {
        private GameObject _obj;
        private ObjectPooler _objectPooler;
        private GameObject _child;
        private GameObject _camera;
        
        [SetUp]
        public void Setup()
        {
            _obj = new GameObject();
            _objectPooler = _obj.AddComponent<ObjectPooler>();

            _child = new GameObject();
            _child.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("cursor");

            _camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Camera"));
        }

        [TearDown]
        public void Teardown()
        {
            UnityEngine.Object.DestroyImmediate(_obj);
            UnityEngine.Object.DestroyImmediate(_child);
            UnityEngine.Object.DestroyImmediate(_camera);
        }

        [UnityTest]
        public IEnumerator ShouldCreateObjectsAndReturnNullIfExceedsPooledAmount()
        {
            var pooledAmount = 1000;

            _objectPooler.WillGrow = false;
            _objectPooler.PooledAmount = pooledAmount;
            _objectPooler.GameObject = _child;

            Assert.AreEqual(_obj.transform.childCount, 0);

            for(var i = 0; i < pooledAmount; i++)
            {
                var obj = _objectPooler.GetPooledObject();
                obj.transform.position = new Vector3(
                    UnityEngine.Random.Range(-100, 100),
                    UnityEngine.Random.Range(-100, 100)
                );
                
                // should be activated
                Assert.IsTrue(obj.activeSelf);

                if(i % 10 == 0)
                    yield return null;
            }
            

            for(var i = 0; i < 10; i++)
            {
                // should be null grow is false
                Assert.IsNull(_objectPooler.GetPooledObject());
            }
                        
        }

        [UnityTest]
        public IEnumerator ShouldCreateObjectsPastedPooledAmountIfGrowTrue()
        {
            var pooledAmount = 1000;

            _objectPooler.WillGrow = true;
            _objectPooler.PooledAmount = pooledAmount;
            _objectPooler.GameObject = _child;

            Assert.AreEqual(_obj.transform.childCount, 0);

            for(var i = 0; i < 2 * pooledAmount; i++)
            {
                var obj = _objectPooler.GetPooledObject();
                obj.transform.position = new Vector3(
                    UnityEngine.Random.Range(-100, 100),
                    UnityEngine.Random.Range(-100, 100)
                );
                
                // should be activated
                Assert.IsTrue(obj.activeSelf);

                if(i % 10 == 0)
                    yield return null;
            }
            
                        
        }
        
        [UnityTest]
        public IEnumerator ShouldRemoveAddedComponents()
        {
            var pooledAmount = 1000;

            _objectPooler.WillGrow = true;
            _objectPooler.PooledAmount = pooledAmount;
            _objectPooler.GameObject = _child;

            Assert.AreEqual(_obj.transform.childCount, 0);

            for(var i = 0; i < pooledAmount; i++)
            {
                var obj = _objectPooler.GetPooledObject();
                obj.transform.position = new Vector3(
                    UnityEngine.Random.Range(-100, 100),
                    UnityEngine.Random.Range(-100, 100)
                );
                
                // should be activated
                Assert.IsTrue(obj.activeSelf);

                obj.AddComponent<BoxCollider2D>();

                if(i % 10 == 0)
                    yield return null;
            }

            for(var i = 0; i < pooledAmount; i++)
            {
                var obj = _objectPooler.GetPooledObject();
                obj.transform.position = new Vector3(
                    UnityEngine.Random.Range(-100, 100),
                    UnityEngine.Random.Range(-100, 100)
                );
                
                // should be activated
                Assert.IsTrue(obj.activeSelf);

                // should not have components from previous object
                Assert.IsNull(obj.GetComponent<BoxCollider2D>());

                if(i % 10 == 0)
                    yield return null;
            }
            
                        
        }
    }
}

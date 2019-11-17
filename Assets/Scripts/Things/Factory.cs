using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things.Config;

namespace Village.Things 
{
    public class Factory : MonoBehaviour
    {
        public TypeOfThing[] Produces { get { return _produces; } }
        private Game _game;
        private Thing _thing;
        private Queue<TypeOfThing> _jobs;
        private float _timer;
        private float _timeToProduce;
        private Dictionary<TypeOfThing, int> _queued;
        private TypeOfThing[] _produces;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _thing = GetComponent<Thing>();
            _jobs = new Queue<TypeOfThing>();
            _timeToProduce = 60f;
            _queued = new Dictionary<TypeOfThing, int>();
        }

        public void Setup(FactoryConfig config)
        {
            _produces = config.Produces;
            foreach(var type in _produces)
            {
                _queued[type] = 0;
            }
        }

        public void AddThingToProduce(TypeOfThing thingToProduce)
        {
            _queued[thingToProduce] += 1;
        }

        public void RemoveThingToProduce(TypeOfThing thingToProduce)
        {
            _queued[thingToProduce] -= 1;
            _queued[thingToProduce] = Mathf.Max(_queued[thingToProduce], 0);
        }

        public void Craft(TypeOfThing thing)
        {
            RemoveThingToProduce(thing);
            _jobs.Enqueue(thing);
            _timer = _timeToProduce;
        }

        public bool IsQueuedForProduction(TypeOfThing type)
        {
            return _queued[type] > 0;
        }

        public int GetQueuedCount(TypeOfThing thing)
        {
            return _queued[thing];
        }


        public bool IsProducing()
        {
            return _jobs.Count > 0;
        }

        public TypeOfThing CurrentlyProducing()
        {
            return IsProducing() ? _jobs.Peek() : TypeOfThing.None;
        }
        
        public void Update()
        {
            if(_jobs.Count == 0)
                return;
            
            if(_timer > 0)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                var position = _game.FindNearestLoosePosition(transform.position.ToVector2IntFloor());

                if(position.HasValue)
                {
                    var item = _jobs.Dequeue();
                    var thing = _game.CreateAtPosition(item, position.Value);
                    _timer = _timeToProduce;
                }

            }
        }

        public void DrawGizmos()
        {
            #if UNITY_EDITOR
                
            var style = new GUIStyle();
            style.fontSize = 10;
            style.normal.textColor = Color.white;

            var label = $"timer: {_timer}";

            // current actions
            var position = _thing.transform.position + Vector3.up;
            UnityEditor.Handles.Label(position, label, style);
                    
    #endif
        }
    }
}
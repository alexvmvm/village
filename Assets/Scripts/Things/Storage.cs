using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Village.Things
{

    public class Storage : MonoBehaviour
    {
        public Dictionary<TypeOfThing, bool> Allowed { get { return _allowed; } }
        public Thing[] Stored { get { return transform.GetComponentsInChildrenExcludingParent<Thing>(); }}
        private Dictionary<TypeOfThing, bool> _allowed;
        private Game _game;
        private int _max;
        private int _currentStoredCount { get { return Stored.Sum(t => t.Hitpoints); } }
        
        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _allowed = new Dictionary<TypeOfThing, bool>();
            _max = 100;
        }
        
        public Thing Add(Thing thing)
        {
            var total = _currentStoredCount + thing.Hitpoints;
            var difference = total - _max;
            
            Thing remainder = null;
            if(difference > 0)
            {
                remainder = _game.CreateAtPosition(thing.Config.TypeOfThing, Vector2Int.zero);
                remainder.Hitpoints = difference;
                thing.Hitpoints = _max - _currentStoredCount;
            }

            // any existing items of same type
            if(Stored.Any(t => t.Config.TypeOfThing == thing.Config.TypeOfThing))
            {
                var totalInStorage = Stored.Where(t => t.Config.TypeOfThing == thing.Config.TypeOfThing).Sum(t => t.Hitpoints);
                var toRemove = Stored.Where(t => t.Config.TypeOfThing == thing.Config.TypeOfThing).ToList();
                
                foreach(var remove in toRemove)
                {
                    _game.Remove(remove);
                }

                thing.Hitpoints += totalInStorage;
            }

            thing.transform.SetParent(transform);
            thing.transform.localPosition = Vector3.zero;

            return remainder;
        }

        public void Allow(TypeOfThing type)
        {
            _allowed[type] = true;
        }

        public void Disallow(TypeOfThing type)
        {
            _allowed[type] = false;
        }

        public bool IsAllowing(TypeOfThing type)
        {
            return _allowed.ContainsKey(type) && _allowed[type];
        }

        public bool IsFull()
        {
            return _currentStoredCount >= _max;
        }

        public void Update()
        {

        }

        public void DrawGizmos()
        {

        }

    }

}

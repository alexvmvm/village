using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Village.Things
{

    public class Storage : ITrait
    {
        public Dictionary<TypeOfThing, bool> Allowed { get { return _allowed; } }
        private Dictionary<TypeOfThing, bool> _allowed;
        private Game _game;
        private Thing _thing;
        private List<Thing> _stored;
        private int _max;
        private int _currentStoredCount { get { return _stored.Sum(t => t.Hitpoints); } }
        
        public Storage(Game game, Thing thing)
        {
            _game = game;
            _allowed = new Dictionary<TypeOfThing, bool>();
            _thing = thing;
            _stored = new List<Thing>();
            _max = 100;
        }

        public void Setup()
        {

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

            if(_stored.Any(t => t.Config.TypeOfThing == thing.Config.TypeOfThing))
            {
                var totalInStorage = _stored.Where(t => t.Config.TypeOfThing == thing.Config.TypeOfThing).Sum(t => t.Hitpoints);
                var toRemove = _stored.Where(t => t.Config.TypeOfThing == thing.Config.TypeOfThing).ToList();
                
                foreach(var remove in toRemove)
                {
                    _stored.Remove(remove);
                    _game.Remove(remove);
                }

                thing.Hitpoints += totalInStorage;
            }

            _stored.Add(thing);
            thing.transform.SetParent(_thing.transform);
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

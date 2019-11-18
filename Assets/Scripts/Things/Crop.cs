using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things.Config;

namespace Village.Things
{

    public class Crop : MonoBehaviour
    {
        private Thing _thing;
        private Game _game;
        private string[] _sprites;
        private float _age;
        private float _delayBeforeProducing;
        private float _timeToGrow;
        private TypeOfThing _produces;
        private int _index;

        void Awake() 
        {
            _game = FindObjectOfType<Game>();
            _thing = GetComponent<Thing>();
            _delayBeforeProducing = 10f;
        }

        public void Setup(CropConfig config)
        {
            _sprites = config.Sprites;
            _timeToGrow = config.TimeToGrow;
            //_thing.Config.TileRuleConfig = new CropTile(this);
            _produces = config.Produces;
        }

        public string GetSprite()
        {
            return _sprites[_index];
        }

        public int GetSpriteIndex()
        {
            return Mathf.FloorToInt(Mathf.Min(_age / _timeToGrow, 1) * (_sprites.Length - 1));
        }

        void Update()
        {
            if(_sprites == null)
                return;

            _age += Time.deltaTime;
            var index = GetSpriteIndex();
            if (index != _index)
            {
                _index = index;
                _thing.SetSprite();
            }

            if(index == _sprites.Length - 1)
                _delayBeforeProducing -= Time.deltaTime;

            if(_delayBeforeProducing < 0f)
            {
                _game.CreateAtPosition(_produces, transform.position.ToVector2IntFloor());
                _game.Remove(_thing);
            }
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR

            var style = new GUIStyle();
            style.fontSize = 10;
            style.normal.textColor = Color.white;

            var label = $"age: {_age}/{_timeToGrow}\nindex: {_index}";

            // current actions
            var position = _thing.transform.position + Vector3.up;
            UnityEditor.Handles.Label(position, label, style);

#endif
        }
    }

}

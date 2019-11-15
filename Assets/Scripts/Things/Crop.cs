using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Village.Things
{

    public class Crop : MonoBehaviour
    {
        private Thing _thing;
        private Game _game;
        private string[] _sprites;
        private float _age;
        private float _timeToGrow;
        private int _index;

        void Awake() 
        {
            _game = FindObjectOfType<Game>();
            _thing = GetComponent<Thing>();
        }

        public void Setup(Thing.CropConfig config)
        {
            _sprites = config.Sprites;
            _timeToGrow = config.TimeToGrow;
            _thing.Config.TileRule = new CropTile(this);
        }

        public string GetSprite()
        {
            return _sprites[_index];
        }

        void Update()
        {
            if(_sprites == null)
                return;

            _age += Time.deltaTime;
            var index = Mathf.FloorToInt(Mathf.Min(_age / _timeToGrow, 1) * (_sprites.Length - 1));
            if (index != _index)
            {
                _index = index;
                _thing.SetSprite();
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

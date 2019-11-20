using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Village.Things
{

    public class Fire : MonoBehaviour
    {
        private Game _game;
        private GameObject _light;
        private Factory _factory;
        private RadialSource _radialSource;
        private float _radius;
        private float _halfRadius;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _factory = GetComponent<Factory>();
        }

        void Start()
        {
            _light = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Fire Light"));
            _light.transform.SetParent(transform);
            _light.transform.localPosition = Vector3.zero;

            _radialSource = GetComponentInChildren<RadialSource>();
            _radius = _radialSource.Radius;
            _halfRadius = _radius / 2;
        }

        public void Update()
        {
            if (_factory != null)
            {
                _light.SetActive(_factory.IsProducing());
            }
            else
            {
                _light.SetActive(true);

                // get light curve for ligths, peaking in day
                var progressInRadians = _game.WorldTime.NormalizedTimeOfDay * Mathf.PI * 2 + Mathf.PI/2;
                var progressInRadiansNormalized = (Mathf.Sin(progressInRadians) + 1) / 2;

                // apply light curve to half radius
                _radialSource.Radius = _halfRadius + _halfRadius * progressInRadiansNormalized;
            }
        }
    }

}

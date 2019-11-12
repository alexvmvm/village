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
        }

        public void Update()
        {
            if (_factory != null)
            {
                _light.SetActive(_factory.IsProducing());
            }
            else
            {
                _light.SetActive(_game.WorldTime.TimeOfDay == TimeOfDay.Night);
            }
        }
    }

}

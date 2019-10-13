using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire 
{   
    private Thing _thing;
    private Game _game;
    private GameObject _light;
    private bool _lightWithFactory;

    public Fire(Game game, Thing thing, bool lightWithFactory = false)
    {
        _thing = thing;
        _game = game;
        _lightWithFactory = lightWithFactory;
    }

    public void Setup()
    {
        _light = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Fire Light"));
        _light.transform.SetParent(_thing.transform);
        _light.transform.localPosition = Vector3.zero;
    }

    public void Update()
    {
        if(_lightWithFactory)
        {
            _light.SetActive(_thing.factory.IsProducing());
        }
        else
        {
            _light.SetActive(_game.WorldTime.TimeOfDay == TimeOfDay.Night);
        }
    }
}

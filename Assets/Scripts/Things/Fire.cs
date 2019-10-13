using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire 
{   
    private Thing _thing;
    private GameObject _light;

    public Fire(Game game, Thing thing)
    {
        _thing = thing;
    }

    public void Setup()
    {
        _light = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Fire Light"));
        _light.transform.SetParent(_thing.transform);
        _light.transform.localPosition = Vector3.zero;
    }

    public void Update()
    {

    }
}

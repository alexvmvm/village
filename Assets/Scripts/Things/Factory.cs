using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Factory 
{
    private Game _game;
    private Thing _thing;
    private Queue<TypeOfThing> _jobs;
    private float _timer;
    private float _timeToProduce;
    private int _requested;

    public Factory(Game game, Thing thing)
    {
        _game = game;
        _thing = thing;
        _jobs = new Queue<TypeOfThing>();
        _timeToProduce = 60f;
    }
    
    public bool IsRequested()
    {
        return _requested > 0;
    }

    public bool IsProducing()
    {
        return _jobs.Count > 0;
    }

    public void RequestJob()
    {
        _requested++;
    }

    public void QueueThingToProduce(TypeOfThing thing)
    {
        _jobs.Enqueue(thing);
        _requested -= 1;
        _timer = _timeToProduce;
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
            var item = _jobs.Dequeue();
            _game.CreateAndAddThing(item, _thing.gridPosition.x, _thing.gridPosition.y - 1);
            _timer = _timeToProduce;
        }
    }

    public void DrawGizmos()
    {
        #if UNITY_EDITOR
               
        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        var label = $"jobs: {_requested}\ntimer: {_timer}";

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, label, style);
                  
#endif
    }
}

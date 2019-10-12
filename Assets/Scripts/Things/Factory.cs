using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Factory 
{
    public TypeOfThing[] Produces { get { return _produces; } }
    private Game _game;
    private Thing _thing;
    private Queue<TypeOfThing> _jobs;
    private float _timer;
    private float _timeToProduce;
    private TypeOfThing[] _produces;
    private Dictionary<TypeOfThing, int> _queued;
    private Dictionary<TypeOfThing, int> _hopper;
    private Dictionary<TypeOfThing, TypeOfThing[]> _map;

    public Factory(Game game, Thing thing, TypeOfThing[] produces)
    {
        _game = game;
        _thing = thing;
        _jobs = new Queue<TypeOfThing>();
        _timeToProduce = 60f;
        _produces = produces;
        _queued = new Dictionary<TypeOfThing, int>();
        _hopper = new Dictionary<TypeOfThing, int>();
        _map = new Dictionary<TypeOfThing, TypeOfThing[]>();
    
        foreach(var type in produces)
        {
            _queued[type] = 0;
        }
    }

    public void Setup()
    {
        foreach(var type in _produces)
        {
            _map[type] = _game.GetThingNotInScene(type).requiredToCraft;
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

    public void AddToHopper(TypeOfThing hopper)
    {
        if(!_hopper.ContainsKey(hopper))
            _hopper[hopper] = 0;
        _hopper[hopper] += 1;
    }
    
    public bool RequiresTypeOfThing(TypeOfThing type)
    {
        // all queud that need this thing
        var totalRequired = _queued
            .Where(kv => kv.Value > 0 && _map[kv.Key].Contains(type))
            .Sum(kv => kv.Value);

        return totalRequired > 0 && (!_hopper.ContainsKey(type) || _hopper[type] < totalRequired);
    }

    public bool IsPossibleToCraftSomething()
    {
        foreach(var kv in _queued)
        {
            if(_queued[kv.Key] == 0)
                continue;

            var found = 0;
            foreach(var required in _map[kv.Key])
            {
                if(!RequiresTypeOfThing(required))
                    found++;
            }
            if(found == _map.Count())
                return true;
        }
        return false;
    }

    public void Craft()
    {
        foreach(var kv in _queued.ToArray())
        {
            if(_queued[kv.Key] == 0)
                continue;

            var found = 0;
            foreach(var required in _map[kv.Key])
            {
                if(!RequiresTypeOfThing(required))
                    found++;
            }

            if(found == _map.Count())
            {
                _queued[kv.Key] -= 1;
                QueueThingToProduce(kv.Key);
            }
        }
    }

    public int GetQueuedCount(TypeOfThing thing)
    {
        return _queued[thing];
    }


    public bool IsProducing()
    {
        return _jobs.Count > 0;
    }


    void QueueThingToProduce(TypeOfThing thing)
    {
        _jobs.Enqueue(thing);
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

        var label = $"timer: {_timer}";

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, label, style);
                  
#endif
    }
}

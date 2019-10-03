using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Coop 
{
    private Game _game;
    private Thing _thing;
    private List<Thing> _beds;
    private HashSet<Thing> _seen;
    private Queue<Thing> _queue;
    private Color _color;
    private Thing _cockeral;
    private Thing _chicken;
    
    public Coop(Game game, Thing thing)
    {
        _game = game;
        _thing = thing;
        _seen = new HashSet<Thing>();
        _queue = new Queue<Thing>();
        _color = ExtensionMethods.RandomColor();

        _beds = new List<Thing>();
    }

    public bool IsSetup()
    {
        return HasChicken() && HasCockeral();
    }

    public bool HasChicken()
    {
        return _chicken != null;
    }

    public bool HasCockeral()
    {
        return _cockeral != null;
    }

    public void AddCockeral(Thing thing)
    {
        _cockeral = thing;
    }

    public void AddChicken(Thing thing)
    {
        _chicken = thing;
    }

    public void Update()
    {
        _beds.Clear();

        _seen.Clear();
        _queue.Clear();

        _queue.Enqueue(_game.GetThingOnGrid(_thing.gridPosition));

        while(_queue.Count > 0 && _seen.Count() < 20)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current))
                continue;

            _seen.Add(current);

            var neighbours = current.GetNeighboursOnGrid();

            foreach(var neighbour in neighbours)
            {
                if(neighbour == null)
                    continue;

                if(neighbour.floor)
                    _queue.Enqueue(neighbour);
                
            }
            
        }
    }

    public void DrawGizmos()
    {
        Gizmos.color = _color;
        foreach(var thing in _seen)
        {
            Gizmos.DrawWireSphere(thing.gridPosition.ToVector3(), 0.2f);
        }
    }
}

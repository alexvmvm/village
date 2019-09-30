using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyChest 
{
    private Game _game;
    private Thing _thing;
    
    private HashSet<Thing> _seen;
    private Queue<Thing> _queue;
    private Color _color;
    
    public FamilyChest(Game game, Thing thing)
    {
        _game = game;
        _thing = thing;
        _seen = new HashSet<Thing>();
        _queue = new Queue<Thing>();
        _color = ExtensionMethods.RandomColor();
    }

    public void Update()
    {
        _seen.Clear();
        _queue.Clear();

        _queue.Enqueue(_game.GetThingOnGrid(_thing.gridPosition));

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current))
                continue;

            _seen.Add(current);

            var neighbours = current.GetNeighboursOnGrid();

            foreach(var neighbour in neighbours)
            {
                if(neighbour.playerBuiltFloor || neighbour.type == TypeOfThing.Door)
                {
                    _queue.Enqueue(neighbour);
                }
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

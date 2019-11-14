using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village;
using Village.AI;
using Village.Things;

public class FillStorage : MoveGOAPAction
{
    private Movement _movement;
    private Village.AI.Villager _villager;
    private bool _started;
    private bool _isDone;
    private Inventory _inventory;
    private TypeOfThing _type;

    public FillStorage(Agent agent, Game game, Movement movement, Villager villager, Inventory inventory, TypeOfThing type) : base(agent, game, movement)
    {
        _movement = movement;
        _villager = villager;
        _inventory = inventory;
        _type = type;
    }

    public override bool Filter(Thing thing)
    {
        return 
            thing != null && 
            thing.Config.TypeOfThing == TypeOfThing.Storage && 
            thing.Storage.IsFull() && 
            thing.Storage.IsAllowing(_type);
    }
    
    public override bool PerformAtTarget()
    {
        var item = _inventory.Drop();
        var remainder = _target.Storage.Add(item);

        if(remainder != null)
        {
            _target.Storage.Add(remainder);
        }

        return true;
    }

    public override string ToString()
    {
        return "Drinking from Stream";
    }
}

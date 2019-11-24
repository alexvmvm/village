using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village;
using Village.AI;
using Village.Things;
using Village.Things.Config;

public class FillStorage : MoveGOAPAction
{
    private Movement _movement;
    private bool _started;
    private bool _isDone;
    private Inventory _inventory;
    private TypeOfThing _type;

    public FillStorage(Agent agent, Game game, Movement movement, Inventory inventory, TypeOfThing type) : base(agent, game, movement)
    {
        _movement = movement;
        _inventory = inventory;
        _type = type;

        var thingConfig = Assets.GetThingConfig(type);
        
        Preconditions.Add(GOAPAction.Effect.HAS_THING + thingConfig.InventorySlot, type);
        Preconditions.Add(GOAPAction.Effect.HAS_THING_FOR_STORAGE, true);
        Effects.Add(GOAPAction.Effect.HAS_THING_FOR_STORAGE, false);
        Effects.Add(GOAPAction.Effect.IS_WORKING, true);
    }

    public override bool Filter(Thing thing)
    {
        return 
            thing != null && 
            thing.Config.TypeOfThing == TypeOfThing.Storage && 
            !thing.Storage.IsFull() && 
            thing.Storage.IsAllowing(_type);
    }

    public override bool PerformAtTarget()
    {
        var item = _inventory.Drop(InventorySlot.Hands);
        var remainder = _target.Storage.Add(item);

        if(remainder != null)
        {
            _target.Storage.Add(remainder);
        }

        return true;
    }
}

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

    public FillStorage(Game game, Movement movement, Villager villager, Inventory inventory, TypeOfThing type) : base(game, movement)
    {
        _movement = movement;
        _villager = villager;
        _inventory = inventory;
        _type = type;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.QueryThings()
            .Where(
                t => t.Config.TypeOfThing == TypeOfThing.Storage && 
                !t.Storage.IsFull() && 
                t.Storage.IsAllowing(_type))
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        var item = _inventory.Drop();
        var remainder = _target.Storage.Add(item);

        if(remainder != null)
        {
            _target.GetTrait<Storage>().Add(remainder);
        }

        return true;
    }

    public override string ToString()
    {
        return "Drinking from Stream";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FillFactoryHopper : MoveGOAPAction
{
    private Movement _movement;
    private Inventory _inventory;
    private TypeOfThing _resource;

    public FillFactoryHopper(Game game, Movement movement, TypeOfThing resource, Inventory inventory) : base(game, movement)
    {
        _movement = movement;
        _inventory = inventory;
        _resource = resource;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t =>  t.factory != null && t.factory.RequiresTypeOfThing(_resource))
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        
        if(_inventory.IsHoldingSomething())
        {
            _inventory.Holding.Destroy();
            _inventory.Drop();
        }

        _target.factory.AddToHopper(_resource);

        return true;
    }
}

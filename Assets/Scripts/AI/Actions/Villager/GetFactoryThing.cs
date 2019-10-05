using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetFactoryThing : MoveGOAPAction
{
    private Movement _movement;
    private Inventory _inventory;
    private TypeOfThing _factoryType;
    private TypeOfThing _output;

    public GetFactoryThing(Game game, Movement movement, TypeOfThing factoryType, TypeOfThing output, Inventory inventory) : base(game, movement)
    {
        _movement = movement;
        _factoryType = factoryType;
        _inventory = inventory;
        _output = output;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t => t.type == _factoryType)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        
        if(_inventory.IsHoldingSomething())
        {
            _inventory.Holding.Destroy();
            _inventory.Drop();
        }

        var resource = _game.CreateAndAddThing(_output, 0, 0);
        resource.hitpoints = Mathf.Min(10, _target.hitpoints);
        _inventory.HoldThing(resource);

        return true;
    }
}

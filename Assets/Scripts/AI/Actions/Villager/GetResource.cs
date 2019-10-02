using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetResource : MoveGOAPAction
{
    private Movement _movement;
    private TypeOfThing _type;
    private Inventory _inventory;

    public GetResource(Game game, Movement movement, TypeOfThing type, Inventory inventory) : base(game, movement)
    {
        _movement = movement;
        _type = type;
        _inventory = inventory;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t => t.type == _type)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        _inventory.HoldThing(_target);
        return true;
    }
}

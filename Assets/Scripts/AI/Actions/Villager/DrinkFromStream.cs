using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrinkFromStream : MoveGOAPAction
{
    private Movement _movement;
    private bool _started;
    private bool _isDone;

    public DrinkFromStream(Game game, Movement movement) : base(game, movement)
    {
        _movement = movement;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t => t.type == TypeOfThing.Stream)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        return true;
    }

    public override string ToString()
    {
        return "Drinking from Stream";
    }
}

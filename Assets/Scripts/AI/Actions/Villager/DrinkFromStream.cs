using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrinkFromStream : GOAPAction
{
    private Movement _movement;
    private Thing _target;
    private bool _started;
    private bool _isDone;

    public DrinkFromStream(Game game, Movement movement) : base(game)
    {
        _movement = movement;
    }

    public override bool IsDone()
    {
        return _isDone;
    }

    public override bool IsPossibleToPerform()
    {
        var targets = _game.Things
            .Where(t => t.type == TypeOfThing.Stream)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));

        foreach(var target in targets)
        {
            if(_movement.IsPathPossible(target.transform.position))
            {
                _target = target;

                // set action cost based on distance
                Cost = Vector2.Distance(_target.transform.position, _movement.transform.position) + 10;

                return true;
            }
        }

        return false;
    }

    public override bool Perform()
    {
        if(!_started)
        {
            _movement.CancelCurrentPath();
            _movement.MoveTo(_target.transform.position);
            _started = true;
        }

        if(_movement.FailedToFollowPath)
            return false;

        if(_movement.ReachedEndOfPath)
        {
            _isDone = true;
        }
        
        
        return true;
    }

    public override void Reset()
    {
       _started = false;
       _isDone = false;
    }
}

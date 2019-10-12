using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MoveGOAPAction : GOAPAction
{
    private Movement _movement;
    protected Thing _target;
    private bool _started;
    private bool _isDone;

    public MoveGOAPAction(Game game, Movement movement) : base(game)
    {
        _movement = movement;
    }
    public virtual void BeforeStartMoving()
    {

    }

    public abstract IEnumerable<Thing> GetThings();

    public abstract bool PerformAtTarget();

    public override bool IsDone()
    {
        return _isDone;
    }

    public override bool IsPossibleToPerform()
    {
        var targets = GetThings();

        foreach(var target in targets)
        {
            if(_movement.IsPathPossible(target.transform.position))
            {
                _target = target;

                // set action cost based on distance
                Cost = Vector2.Distance(_target.transform.position, _movement.transform.position);

                return true;
            }
        }

        return false;
    }

    public override bool Perform()
    {
        if(!_started)
        {
            BeforeStartMoving();
            
            _movement.CancelCurrentPath();
            _movement.MoveTo(_target.transform.position);
            _started = true;
        }

        if(_movement.FailedToFollowPath)
            return false;

        if(_movement.ReachedEndOfPath)
        {
            if(!PerformAtTarget())
                return true;
 
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

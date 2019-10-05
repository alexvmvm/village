using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Construct : GOAPAction
{
    private Movement _movement;
    private Thing _target;
    private bool _started;
    private bool _isDone;
    private TypeOfThing _type;
    private Thing _thing;

    public Construct(Game game, Movement movement, TypeOfThing type, Thing thing) : base(game)
    {
        _movement = movement;
        _type = type;   
        _thing = thing;
    }

    public override bool IsDone()
    {
        return _isDone;
    }

    public override bool IsPossibleToPerform()
    {

        _target = _game.Things
            .Where(t => t.construction != null && t.construction.Requires == _type)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position))
            .FirstOrDefault();

        return _target != null && _movement.IsPathPossible(_target.transform.position);
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
            _target.construction.Construct();

            if(_thing.inventory.IsHoldingSomething())
            {
                var resource = _thing.inventory.Holding;
                resource.hitpoints -= 1; 
                
                if(resource.hitpoints == 0)
                {
                    _thing.inventory.Drop();
                    resource.Destroy();
                }
            }
            _isDone = true;
        }
        
        
        return true;
    }

    public override void Reset()
    {
       _started = false;
       _isDone = false;
    }

    public override string ToString() 
    {
        if(_target == null || _target.construction == null)
            return base.ToString();

        return $"Building {_target.construction.BuildType.ToString()}";
    }
}

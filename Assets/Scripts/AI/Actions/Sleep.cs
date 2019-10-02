using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : GOAPAction
{
    private Game _game;
    private Thing _thing;
    private Movement _movement;
    private bool _started;
    private Vector3 _target;
    public Sleep(Game game, Thing thing, Movement movement) : base(game)
    {
        _game = game;
        _thing = thing;
        _movement = movement;
    }

    public override bool IsDone()
    {
        return _game.WorldTime.GetTimeOfDay() == TimeOfDay.Day;
    }

    public override bool IsPossibleToPerform()
    {
        return _game.WorldTime.GetTimeOfDay() == TimeOfDay.Night;
    }

    public override bool Perform()
    {
        if(!_started)
        {
            _target = _thing.transform.position;
            _movement.CancelCurrentPath();
            _movement.MoveTo(_target);
            _started = true;
        }

        if(_movement.FailedToFollowPath)
            return false;

        if(_movement.ReachedEndOfPath)
        {
            switch(_game.WorldTime.GetTimeOfDay())
            {
                case TimeOfDay.Night:
                    _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
                default:
                    _thing.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
        }
        
        return true;
    }

    public override void Reset()
    {
        _started = false;
    }
}

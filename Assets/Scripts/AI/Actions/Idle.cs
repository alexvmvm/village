using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : GOAPAction
{
    private Movement _movement;
    private Vector3 _target = new Vector3(10, 10, 0);
    private bool _started;
    private float _timeToWait;
    private float _timeWaited;

    public Idle(Game game, Movement movement) : base(game)
    {
        _movement = movement;

        _preconditions.Add("isIdle", false);
        _effects.Add("isIdle", true);
    }

    public override bool IsDone()
    {
        return _timeWaited > _timeToWait;
    }

    public override bool IsPossibleToPerform()
    {

        var offset = new Vector3(
            Random.Range(-5, 5), 
            Random.Range(-5, 5));
        
        _target = _movement.transform.position + offset;

        if(!_game.IsOnGrid((int)_target.x, (int)_target.y))
            return false;

        _timeToWait = Random.Range(0, 10);

        return true;
    }

    public override bool Perform()
    {
        if(!_started)
        {
            _movement.MoveTo(_target);
            _started = true;
        }

        if(_movement.FailedToFollowPath)
            return false;

        if(_movement.ReachedEndOfPath)
            _timeWaited += Time.deltaTime;
        
        return true;
    }

    public override void Reset()
    {
       _started = false;
       _timeToWait = 0f;
       _timeWaited = 0f;
    }
}

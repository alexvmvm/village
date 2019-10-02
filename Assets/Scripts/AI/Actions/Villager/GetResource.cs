﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetResource : GOAPAction
{
    private Movement _movement;
    private Thing _target;
    private bool _started;
    private bool _isDone;
    private TypeOfThing _type;
    private Inventory _inventory;

    public GetResource(Game game, Movement movement, TypeOfThing type, Inventory inventory) : base(game)
    {
        _movement = movement;
        _type = type;
        _inventory = inventory;
    }

    public override bool IsDone()
    {
        return _isDone;
    }

    public override bool IsPossibleToPerform()
    {

        _target = _game.Things
            .Where(t => t.type == _type)
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position))
            .FirstOrDefault();
        
        if(_target == null)
            return false;

        // set action cost based on distance
        Cost = Vector2.Distance(_target.transform.position, _movement.transform.position);

        return _movement.IsPathPossible(_target.transform.position);
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
            _inventory.HoldThing(_target);
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
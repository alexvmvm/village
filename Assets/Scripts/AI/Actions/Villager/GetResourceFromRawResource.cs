using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetResourceFromRawResource : GOAPAction
{
    private Movement _movement;
    private Thing _target;
    private bool _started;
    private bool _isDone;
    private TypeOfThing _type;
    private Inventory _inventory;

    public GetResourceFromRawResource(Game game, Movement movement, TypeOfThing type, Inventory inventory) : base(game)
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
            .Where(t => t.type == _type && _movement.IsPathPossible(t.transform.position))
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position))
            .FirstOrDefault();
        
        if(_target == null)
            return false;

        // set action cost based on distance
        Cost = Vector2.Distance(_target.transform.position, _movement.transform.position) + 10;

        return true;
    }

    TypeOfThing RawResourceToProcessed(TypeOfThing rawResource)
    {
        switch(rawResource)
        {
            case TypeOfThing.Tree:
                return TypeOfThing.Wood;
            case TypeOfThing.Rock:
                return TypeOfThing.Stone;
            default:
                throw new System.Exception(string.Format("Unknown raw resource conversion {0}", rawResource));
        }
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
            // get resource to hold
            var processedResourceType = RawResourceToProcessed(_type);
            var resource = _game.CreateAndAddThing(processedResourceType, 0, 0);
            resource.hitpoints = 10;
            _inventory.HoldThing(resource);

            // damage existing resource
            _target.hitpoints -= 10;
            if(_target.hitpoints <= 0)
                _game.CreateAndAddThing(TypeOfThing.Grass, _target.gridPosition.x, _target.gridPosition.y);
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

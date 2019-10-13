using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubmitFactoryJob : MoveGOAPAction
{
    private Movement _movement;
    private Inventory _inventory;
    private TypeOfThing _factoryType;
    private TypeOfThing _output;
    private bool _requiresAgentToMake;
    private bool _submittedJob;

    public SubmitFactoryJob(Game game, Movement movement, TypeOfThing factoryType, TypeOfThing output, Inventory inventory, bool requiresAgentToMake) : base(game, movement)
    {
        _movement = movement;
        _factoryType = factoryType;
        _inventory = inventory;
        _output = output;
        _requiresAgentToMake = requiresAgentToMake;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t => t.type == _factoryType && !t.factory.IsProducing() && t.factory.IsQueuedForProduction(_output))
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        if(_inventory.IsHoldingSomething())
        {
            _inventory.Holding.Destroy();
            _inventory.Drop();
        }

        if(!_submittedJob)
        {
            _target.factory.Craft(_output);
            _submittedJob = true;
        }

        if(_requiresAgentToMake && _target.factory.IsProducing())
            return false;


        return true;
    }

    public override void Reset()
    {
        base.Reset();

        _submittedJob = false;
    }
}

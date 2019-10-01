using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaveVillage : GOAPAction
{
    private Movement _movement;
    private Thing _thing;
    private bool _started;
    private bool _isDone;
    private VillageManager _villageManager;
    private Villager _villager;

    public LeaveVillage(Game game, Movement movement, Thing thing, Villager villager) : base(game)
    {
        _movement = movement;
        _thing = thing;
        _villager = villager;
        _villageManager = MonoBehaviour.FindObjectOfType<VillageManager>();
    }

    public override bool IsDone()
    {
        return _isDone;
    }

    public override bool IsPossibleToPerform()
    {
        return true;
    }

    public override bool Perform()
    {
        if(!_started)
        {
            if(_villageManager != null)
                _villageManager.TriggerEvent(VillagerEvent.VillagerLeft, _villager);

            _movement.CancelCurrentPath();
            _movement.MoveTo(_game.GetVillageExit());
            _started = true;
        }

        if(_movement.FailedToFollowPath)
            return false;

        if(_movement.ReachedEndOfPath)
        {
            _thing.Destroy();
            _isDone = true;
        }
        
        return true;
    }

    public override void Reset()
    {

    }
}

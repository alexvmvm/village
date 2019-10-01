using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestResidence : GOAPAction
{
    private bool _isDone;
    private Thing _thing;
    private Villager _villager;
    private VillageManager _villageManager;

    public RequestResidence(Game game, Thing thing, Villager villager) : base(game)
    {
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
        return !_isDone;
    }

    public override bool Perform()
    {
        if(_villageManager != null)
            _villageManager.TriggerEvent(VillagerEvent.VillagerArrived, _villager);
        _isDone = true;
        return true;
    }

    public override void Reset()
    {
        
    }
}

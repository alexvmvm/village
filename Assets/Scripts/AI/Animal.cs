using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : ThingAgent
{
    public Thing Coop;
    private Dictionary<string, object> _goal;
    private Dictionary<string, object> _world;
    private Movement _movement;

    public Animal(Game game, Thing thing) : base(game, thing)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _goal = new Dictionary<string, object>();
        _world = new Dictionary<string, object>();

        AddAction(new IdleInCoop(_game, _movement, this) {
            Effects         = { { "isIdle", false } },
            Preconditions   = { { "isIdle", true },     { "isInCoop", true } }
        });

        AddAction(new Idle(_game, _movement) {
            Effects         = { { "isIdle", false } },
            Preconditions   = { { "isIdle", true } },
            Cost = 10
        }); 
    }

    public override void PauseAgent()
    {
        base.PauseAgent();

        _movement.CancelCurrentPath();
        _movement.SetStopped(true);
    }

    public override void UnPauseAgent()
    {
        base.UnPauseAgent();

        _movement.SetStopped(false);
    }

    public override void ActionCompleted(GOAPAction action)
    {
        
    }

    public override Dictionary<string, object> GetGoal()
    {
        _goal["isIdle"] = false;
        return _goal;
    }

    public override Dictionary<string, object> GetWorldState()
    {
        _world["isIdle"] = true;
        _world["isInCoop"]  = Coop != null;
        return _world;
    }

    public override void Update()
    {
        base.Update();

        SetLabel($"{_thing.type.ToString()}\n{(CurentAction == null ? "" : CurentAction.ToString())}");
    }
}

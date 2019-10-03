using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Agent
{
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;
    private Movement _movement;
    private Thing _thing;

    public Animal(Game game, Thing thing) : base(game)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _world.Add("isIdle", true);
        _goal.Add("isIdle", false);

        AddAction(new Idle(_game, _movement) {
            Effects         = { { "isIdle", false } },
            Preconditions   = { { "isIdle", true } }
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

    public override Dictionary<string, bool> GetGoal()
    {
        return _goal;
    }

    public override Dictionary<string, bool> GetWorldState()
    {
        return _world;
    }
}

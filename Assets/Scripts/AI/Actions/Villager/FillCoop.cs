using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FillCoop : MoveGOAPAction
{
    private Movement _movement;
    private Villager _villager;
    private bool _started;
    private bool _isDone;
    private Inventory _inventory;

    public FillCoop(Game game, Movement movement, Villager villager, Inventory inventory) : base(game, movement)
    {
        _movement = movement;
        _villager = villager;
        _inventory = inventory;
    }

    public override IEnumerable<Thing> GetThings()
    {
        return _game.Things
            .Where(t => t.type == TypeOfThing.ChickenCoop && t.belongsToFamily == _villager.Lastname && !t.coop.IsSetup())
            .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position));
    }

    public override bool PerformAtTarget()
    {
        var chicken = _inventory.Holding;
        _target.coop.AddChicken(chicken);
        chicken.agent.UnPauseAgent();
        _inventory.Drop();
        return true;
    }

    public override string ToString()
    {
        return "Drinking from Stream";
    }
}

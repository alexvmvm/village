using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : GOAPAction
{
    private Thing _thing;

    public Drop(Game game, Thing thing) : base(game)
    {
        _thing = thing;    
    }

    public override bool IsDone()
    {
        return !_thing.inventory.IsHoldingSomething();
    }

    public override bool IsPossibleToPerform()
    {
        return _thing.inventory != null;
    }

    public override bool Perform()
    {
        _thing.inventory.Drop();
        return true;
    }

    public override void Reset()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village;
using Village.AI;

public abstract class SimpleGOAPAction : GOAPAction
{
    public SimpleGOAPAction(GOAPAgent agent, Game game) : base(agent, game)
    {
    }

    public override bool IsDone()
    {
        return true;
    }

    public override bool IsPossibleToPerform()
    {
        return true;
    }

    public override bool Perform()
    {
        return true;
    }

    public override void Reset()
    {
        
    }
}

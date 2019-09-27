using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public struct ActionState
{
    public string action;
    public bool state;
}

public abstract class GOAPAction
{
    public Dictionary<string, bool> Preconditions;
    public Dictionary<string, bool> Effects;

    public float Cost = 1f;

    protected Game _game;

    public GOAPAction(Game game)
    {
        _game = game;

        Preconditions = new Dictionary<string, bool>();
        Effects = new Dictionary<string, bool>();
    }

    abstract public bool Perform();
    abstract public bool IsDone();
    abstract public bool IsPossibleToPerform();

    abstract public void Reset();
}



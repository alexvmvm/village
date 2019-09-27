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
    public Dictionary<string, bool> Preconditions { get { return _preconditions; } }
    public Dictionary<string, bool> Effects { get { return _effects; } }

    public float Cost = 1f;

    protected readonly Dictionary<string, bool> _preconditions;
    protected readonly Dictionary<string, bool> _effects;

    protected Game _game;

    public GOAPAction(Game game)
    {
        _preconditions = new Dictionary<string, bool>();
        _effects = new Dictionary<string, bool>();
        _game = game;
    }

    abstract public bool Perform();
    abstract public bool IsDone();
    abstract public bool IsPossibleToPerform();

    abstract public void Reset();
}



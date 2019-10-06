using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeedEventOp
{
    Increment,
    Decrement,
    Set
}

public struct NeedEvent
{
    public string Description;
    public string Need;
    public float Value;   
    public NeedEventOp NeedEventOp;
    public float WorldTime;
}

public class Needs
{
    private Game _game;
    private List<NeedEvent> _events;
    public Needs(Game game)
    {
        _game = game;
        _events = new List<NeedEvent>();
    }
    
}

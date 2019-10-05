using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Factory 
{
    private Game _game;
    private Thing _thing;
    private TypeOfThing _input;
    private TypeOfThing _output;
    private int _jobs;

    public Factory(Game game, Thing thing, TypeOfThing input, TypeOfThing output)
    {
        _game = game;
        _thing = thing;
        _input = input;
        _output = output;
    }

    public bool HasJob()
    {
        return _jobs > 0;
    }

    public Thing CompleteJob()
    {
        _jobs = Mathf.Max(0, _jobs -= 1);
        return _game.CreateAndAddThing(_output, _thing.gridPosition.x, _thing.gridPosition.y);
    }
}

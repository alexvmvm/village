using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Agent
{
    private bool _requestedResidence;
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;
    public Villager(Game game, Thing thing) : base(game, thing)
    {
        _thing.name = NameGenerator.GenerateCharacterName();

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();




        AddAction(new RequestResidence(_game, _thing) {
            Preconditions   = { { "hasRequestedResidence", false } },
            Effects         = { { "hasRequestedResidence", true } },
        });

        AddAction(new Idle(_game, _movement) {
            Preconditions   = { { "isIdle", true } },
            Effects         = { { "isIdle", false } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Tree) {
            Preconditions   = { { "hasWood", false } },
            Effects         = { { "hasWood", true } }
        }); 

         AddAction(new GetResource(_game, _movement, TypeOfThing.Stone) {
            Preconditions   = { { "hasStone", false } },
            Effects         = { { "hasStone", true } }
        }); 

        AddAction(new Construct(_game, _movement, TypeOfThing.Wood) {
            Preconditions   = { { "hasWood", true } },
            Effects         = { { "isWorking", true }, }
        });

        AddAction(new Construct(_game, _movement, TypeOfThing.Stone) {
            Preconditions   = { { "hasStone", true } },
            Effects         = { { "isWorking", true }, }
        });


        AddAction(new Construct(_game, _movement, TypeOfThing.Stone));
        
        // AddAction(new GetResource(_game, _movement, TypeOfThing.Tree));
        // AddAction(new GetResource(_game, _movement, TypeOfThing.Stone));
    
        // AddAction(new ConvertResource(_game, _movement, TypeOfThing.Tree));
        // AddAction(new ConvertResource(_game, _movement, TypeOfThing.Stone));

    }

    public override void ActionCompleted(GOAPAction action)
    {
        if(action is RequestResidence)
            _requestedResidence = true;
    }

    public override Dictionary<string, bool> GetGoal()
    {
        _goal.Clear();
        if(!_requestedResidence)
        {
            _goal["hasRequestedResidence"] = true;
        }
        else
        {
            _goal["isWorking"] = true;
        }

        return _goal;
    }

    public override Dictionary<string, bool> GetWorldState()
    {
        _world["hasRequestedResidence"] = _requestedResidence;
        _world["isIdle"] =  true;
        _world["hasWood"] = false;
        _world["hasStone"] =  false;
        _world["isWorking"] = false;
        
        return _world;
    }
}

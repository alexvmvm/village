using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Agent
{
    public string Firstname { get { return _firstname; } }
    public string Lastname { get { return _lastname; } }
    public string Fullname { get { return Firstname + " " + Lastname; } }
    public FamilyChest FamilyChest { get { return _familyChestThing != null ? _familyChestThing.familyChest : null; } }
    private string _firstname;
    private string _lastname;
    private bool _requestedResidence;
    private Dictionary<string, bool> _goal;
    private Dictionary<string, bool> _world;
    private Thing _familyChestThing;
    private Thing _thing;
    private Movement _movement;
    private float _idleTime;
    private bool _leaveVillage;
    private Clock _clock;

    public Villager(Game game, Thing thing) : base(game)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _clock = MonoBehaviour.FindObjectOfType<Clock>();

        _firstname = NameGenerator.GenerateFirstName();
        _lastname = NameGenerator.GenerateLastName();

        _thing.name = string.Format("{0} {1}", _firstname, _lastname);

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        /*
            Misc
        */

        AddAction(new RequestResidence(_game, _thing, this) {
            Preconditions   = { { "hasRequestedResidence", false } },
            Effects         = { { "hasRequestedResidence", true } },
        });

        AddAction(new LeaveVillage(_game, _movement, _thing, this) {
            Preconditions   = { { "hasLeftVillage", false } },
            Effects         = { { "hasLeftVillage", true } },
        });

        AddAction(new Drop(_game, _thing) {
            Preconditions   = { { "hasFullInventory", true } },
            Effects         = { { "hasFullInventory", false } },
        });

        AddAction(new Sleep(_game, _thing, _movement, this) {
            Preconditions   = { { "isSleeping", false }, { "hasFullInventory", false } },
            Effects         = { { "isSleeping", true } },
        });

        /*
            Idle
        */

        AddAction(new Idle(_game, _movement) {
            Preconditions   = { { "isWorking", false }, { "hasFullInventory", false } },
            Effects         = { { "isWorking", true } },
            Cost = 10
        }); 

        /*
            Resources
        */

        AddAction(new GetResource(_game, _movement, TypeOfThing.Tree, thing.inventory) {
            Preconditions   = { { "hasWood", false },   { "hasFullInventory", false } },
            Effects         = { { "hasWood", true },    { "hasFullInventory", true } }
        }); 

         AddAction(new GetResource(_game, _movement, TypeOfThing.Rock, thing.inventory) {
            Preconditions   = { { "hasStone", false },  { "hasFullInventory", false } },
            Effects         = { { "hasStone", true },   { "hasFullInventory", true } }
        }); 

        /*
            Construction
        */

        AddAction(new Construct(_game, _movement, TypeOfThing.Wood, _thing) {
            Preconditions   = { { "hasWood", true } },
            Effects         = { { "isWorking", true }, }
        });

        AddAction(new Construct(_game, _movement, TypeOfThing.Stone, _thing) {
            Preconditions   = { { "hasStone", true } },
            Effects         = { { "isWorking", true }, }
        });

    }

    public override void ActionCompleted(GOAPAction action)
    {
        if(action is RequestResidence)
            _requestedResidence = true;

        if(!(action is Idle))
            _idleTime = 0f;
    }

    bool ShouldLeaveVillage()
    {
        return _idleTime > _clock.SecondsInADay && FamilyChest == null;
    }

    public override Dictionary<string, bool> GetGoal()
    {
        _goal.Clear();
        if(!_requestedResidence)
        {
            _goal["hasRequestedResidence"] = true;
        }
        else if(ShouldLeaveVillage())
        {
            _goal["hasLeftVillage"] = true;
        }
        else if(_clock.TimeOfDay == TimeOfDay.Night)
        {
            _goal["isSleeping"] = true;
        }
        else
        {
            _goal["isWorking"] = true;
        }

        return _goal;
    }

    public override Dictionary<string, bool> GetWorldState()
    {
        _familyChestThing = _game.FindChestForFamily(Lastname);

        _world["hasRequestedResidence"] = _requestedResidence;
        _world["isIdle"] =  true;
        
        /*
            Resources
        */
        
        _world["hasWood"] = _thing.inventory.IsHolding(TypeOfThing.Wood);
        _world["hasStone"] =  _thing.inventory.IsHolding(TypeOfThing.Stone);
        _world["hasFullInventory"] = _thing.inventory.IsHoldingSomething();

        _world["isWorking"] = false;
        _world["isSleeping"] = false;
        _world["hasHome"] =  FamilyChest != null;
        _world["hasLeftVillage"] = false;
        
        return _world;
    }

    public override void Update()
    {
        base.Update();

        _idleTime += Time.deltaTime;
    }

    public override void DrawGizmos()
    {
#if UNITY_EDITOR
        
        var text = "";

        text += string.Format("idleTime: {0}", _idleTime);

        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, text, style);
#endif   
    }
}

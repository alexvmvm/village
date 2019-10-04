using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : ThingAgent
{
    public string Firstname { get { return _firstname; } }
    public string Lastname { get { return _lastname; } }
    public string Fullname { get { return Firstname + " " + Lastname; } }
    public FamilyChest FamilyChest { get { return _familyChestThing != null ? _familyChestThing.familyChest : null; } }
    private string _firstname;
    private string _lastname;
    private bool _requestedResidence;
    private Dictionary<string, object> _goal;
    private Dictionary<string, object> _world;
    private VillageManager _villagerManager;
    private Thing _familyChestThing;
    private Movement _movement;
    private float _idleTime;
    private bool _leaveVillage;
    private int _nightsSleptInHome;

    /* 
        Survival
    */

    private bool _thirsty;
    private bool _hungry;

    public Villager(Game game, Thing thing) : base(game, thing)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _firstname = NameGenerator.GenerateFirstName();
        _lastname = NameGenerator.GenerateLastName();

        _thing.name = string.Format("{0} {1}", _firstname, _lastname);

        _goal = new Dictionary<string, object>();
        _world = new Dictionary<string, object>();

        _villagerManager = MonoBehaviour.FindObjectOfType<VillageManager>();



        /*
            Misc
        */

        AddAction(new RequestResidence(_game, _thing) {
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

        AddAction(new SleepAtHome(_game, _thing, _movement, this) {
            Preconditions   = { { "isSleeping", false }, { "hasFullInventory", false }, { "hasHome", true } },
            Effects         = { { "isSleeping", true } },
        });

        AddAction(new Sleep(_game, _thing, _movement) {
            Preconditions   = { { "isSleeping", false }, { "hasFullInventory", false } },
            Effects         = { { "isSleeping", true } },
        });

        /*
            Idle
        */

        AddAction(new Idle(_game, _movement) {
            Preconditions   = { { "isWorking", false }, { "hasFullInventory", false } },
            Effects         = { { "isWorking", true } },
            Cost = 99999 // last resort
        }); 

        /*
            Farming
        */
        AddAction(new FillCoop(_game, _movement, this, thing.inventory) {
            Preconditions   = { { "hasChicken", true } },
            Effects         = { { "isWorking", true }, { "hasChicken", false }  }
        });

        /*
            Survival
        */

        AddAction(new DrinkFromStream(_game, _movement) {
            Preconditions   = { { "isThirsty", true } },
            Effects         = { { "isThirsty", false }, { "needsFullfilled", true } }
        }); 

        AddAction(new EastSomething(_game, thing.inventory) {
            Preconditions   = { { "isHungry", true },   { "hasEdibleThing", true } },
            Effects         = { { "isHungry", false },  { "needsFullfilled", true } }
        }); 

        /*
            Resources
        */

        AddAction(new GetResource(_game, _movement, TypeOfThing.Rock, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Stone },   { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Mushroom, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Mushroom },  { "hasFullInventory", true }, { "hasEdibleThing", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Tree, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false }, { "hasThing", TypeOfThing.Axe } },
            Effects         = { { "hasThing", TypeOfThing.Wood },  { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.FallenWood, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Wood },    { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Wood, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Wood },    { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Clay, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Clay },    { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Stone, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Stone },   { "hasFullInventory", true } }
        });

        AddAction(new GetResource(_game, _movement, TypeOfThing.Hen, thing.inventory) {
            Preconditions   = { { "hasFullInventory", false } },
            Effects         = { { "hasThing", TypeOfThing.Hen },   { "hasFullInventory", true } }
        });
         

        /*
            Construction
        */

        AddAction(new Construct(_game, _movement, TypeOfThing.Wood, _thing) {
            Preconditions   = { { "hasThing", TypeOfThing.Wood } },
            Effects         = { { "isWorking", true }, }
        });

        AddAction(new Construct(_game, _movement, TypeOfThing.Stone, _thing) {
            Preconditions   = { { "hasThing", TypeOfThing.Stone } },
            Effects         = { { "isWorking", true }, }
        });

        AddAction(new Construct(_game, _movement, TypeOfThing.Clay, _thing) {
            Preconditions   = { { "hasThing", TypeOfThing.Clay } },
            Effects         = { { "isWorking", true }, }
        });

    }

    public override void ActionCompleted(GOAPAction action)
    {
        if(!(action is Idle) && !(action is Sleep))
        {
            _idleTime = 0f;
        }
        
        if(action is SleepAtHome)
        {
            if(_nightsSleptInHome == 0 && _villagerManager != null)
                _villagerManager.TriggerEvent(VillagerEvent.VillagerFirstNightAtHome, this);

            _nightsSleptInHome += 1;
        }

        // thirsty after sleeping
        if(action is Sleep || action is SleepAtHome)
        {
            _thirsty = true;
            _hungry = true;
        }
        
        if(action is RequestResidence)
        {
            if(_villagerManager != null)
                _villagerManager.TriggerEvent(VillagerEvent.VillagerArrived, this);
            _requestedResidence = true;
        }

        if(action is DrinkFromStream)
            _thirsty = false;
        
        if(action is EastSomething)
            _hungry = false;

    }

    bool ShouldLeaveVillage()
    {
        return _idleTime > _game.WorldTime.SecondsInADay && FamilyChest == null;
    }

    public override Dictionary<string, object> GetGoal()
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
        else if(_game.WorldTime.GetTimeOfDay() == TimeOfDay.Night)
        {
            _goal["isSleeping"] = true;
        }
        else if(_hungry)
        {
            _goal["needsFullfilled"] = true;
        }
        else
        {
            _goal["isWorking"] = true;
        }

        return _goal;
    }

    public override Dictionary<string, object> GetWorldState()
    {
        _familyChestThing = _game.FindChestForFamily(Lastname);

        _world["hasRequestedResidence"] = _requestedResidence;
        _world["isIdle"] =  true;
        
        /*
            Resources
        */
        
        _world["hasThing"] = _thing.inventory.IsHoldingSomething() ?
            _thing.inventory.Holding.type : TypeOfThing.None;

        _world["hasEdibleThing"] = 
            _thing.inventory.IsHoldingSomething() && 
            _thing.inventory.IsHoldingSomethingToEat();

        _world["hasFullInventory"] = _thing.inventory.IsHoldingSomething();

        /*
            Survival
        */

        _world["isThirsty"] = _thirsty;
        _world["isHungry"] = _hungry;

        _world["isWorking"] = false;
        _world["isSleeping"] = false;
        _world["hasHome"] =  FamilyChest != null;
        _world["hasLeftVillage"] = false;
        _world["needsFullfilled"] = false;
        
        return _world;
    }

    public override void Update()
    {
        base.Update();

        _idleTime += Time.deltaTime;
        
        SetLabel($"{Fullname}\n{(CurentAction == null ? "" : CurentAction.ToString())}");
    }

    public override void DrawGizmos()
    {
#if UNITY_EDITOR
        
        var text = "";

        if(_current != null)
        {
            text += string.Format("current action: {0}\n", _current.ToString());
        }

        text += string.Format("idleTime: {0}\n", _idleTime);

        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, text, style);
#endif   
    }
}

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
    private VillageManager _villagerManager;
    private Thing _familyChestThing;
    private Thing _thing;
    private Movement _movement;
    private float _idleTime;
    private bool _leaveVillage;
    private int _nightsSleptInHome;
    private TextMesh _textMesh;

    /* 
        Survival
    */

    private bool _thirsty;
    private bool _hungry;

    public Villager(Game game, Thing thing) : base(game)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _firstname = NameGenerator.GenerateFirstName();
        _lastname = NameGenerator.GenerateLastName();

        _thing.name = string.Format("{0} {1}", _firstname, _lastname);

        _goal = new Dictionary<string, bool>();
        _world = new Dictionary<string, bool>();

        _villagerManager = MonoBehaviour.FindObjectOfType<VillageManager>();

        /*
            Name Label 
        */
        var labelObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Label"));    
        labelObj.transform.SetParent(_thing.transform);
        labelObj.transform.localPosition = new Vector3(0, -0.8f, 0);

        _textMesh = labelObj.GetComponentInChildren<TextMesh>();
        _textMesh.text = Fullname;
        labelObj.GetComponentInChildren<MeshRenderer>().sortingOrder = (int)SortingOrders.Labels;
      

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
            Survival
        */

        AddAction(new DrinkFromStream(_game, _movement) {
            Preconditions   = { { "isThirsty", true } },
            Effects         = { { "isThirsty", false }, { "needsFullfilled", true } }
        }); 

        AddAction(new EastSomething(_game, thing.inventory) {
            Preconditions   = { { "isHungry", true },   { "hasSomethingToEat", true } },
            Effects         = { { "isHungry", false },  { "needsFullfilled", true } }
        }); 

        /*
            Resources
        */

        AddAction(new GetResourceFromRawResource(_game, _movement, TypeOfThing.Tree, thing.inventory) {
            Preconditions   = { { "hasWood", false },   { "hasFullInventory", false } },
            Effects         = { { "hasWood", true },    { "hasFullInventory", true } }
        }); 

        AddAction(new GetResourceFromRawResource(_game, _movement, TypeOfThing.Rock, thing.inventory) {
            Preconditions   = { { "hasStone", false },  { "hasFullInventory", false } },
            Effects         = { { "hasStone", true },   { "hasFullInventory", true } }
        }); 

        AddAction(new GetResourceFromRawResource(_game, _movement, TypeOfThing.Mushroom, thing.inventory) {
            Preconditions   = { { "hasMushroom", false },  { "hasFullInventory", false },   { "hasSomethingToEat", false } },
            Effects         = { { "hasMushroom", true },   { "hasFullInventory", true },    { "hasSomethingToEat", true } }
        }); 


        AddAction(new GetResource(_game, _movement, TypeOfThing.Wood, thing.inventory) {
            Preconditions   = { { "hasWood", false },   { "hasFullInventory", false } },
            Effects         = { { "hasWood", true },    { "hasFullInventory", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Stone, thing.inventory) {
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
        else if(_game.WorldTime.GetTimeOfDay() == TimeOfDay.Night)
        {
            _goal["isSleeping"] = true;
        }
        else if(_thirsty || _hungry)
        {
            _goal["needsFullfilled"] = true;
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
        _world["hasMushroom"] = _thing.inventory.IsHolding(TypeOfThing.Mushroom);

        _world["hasSomethingToEat"] = _thing.inventory.IsHoldingSomethingToEat();
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
        
        _textMesh.text = $"{Fullname}\n{(CurentAction == null ? "" : CurentAction.ToString())}";
        _textMesh.transform.rotation = Quaternion.identity;
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

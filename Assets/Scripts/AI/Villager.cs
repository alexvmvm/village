﻿using System;
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
    private Needs _needs;

    /* 
        Survival
    */

    private float _thirst;
    private float _hunger;
    private float _warmth;
    private float _rest;

    public Villager(Game game, Thing thing) : base(game, thing)
    {
        _thing = thing;
        _movement = _thing.transform.gameObject.AddComponent<Movement>();

        _needs = new Needs(game);

        _firstname = NameGenerator.GenerateFirstName();
        _lastname = NameGenerator.GenerateLastName();

        _thing.name = string.Format("{0} {1}", _firstname, _lastname);

        _goal = new Dictionary<string, object>();
        _world = new Dictionary<string, object>();

        _villagerManager = MonoBehaviour.FindObjectOfType<VillageManager>();



        /*
            Misc
        */

        // AddAction(new RequestResidence(_game, _thing) {
        //     Preconditions   = { { "hasRequestedResidence", false } },
        //     Effects         = { { "hasRequestedResidence", true } },
        // });

        // AddAction(new LeaveVillage(_game, _movement, _thing, this) {
        //     Preconditions   = { { "hasLeftVillage", false } },
        //     Effects         = { { "hasLeftVillage", true } },
        // });

        AddAction(new Drop(_game, _thing) {
            Preconditions   = { { "hasAnything", true } },
            Effects         = { { "hasAnything", false } },
        });

        // AddAction(new SleepAtHome(_game, _thing, _movement, this) {
        //     Preconditions   = { { "isSleeping", false }, { "hasAnything", false }, { "hasHome", true } },
        //     Effects         = { { "isSleeping", true } },
        // });

        AddAction(new Sleep(_game, _thing, _movement, this, _needs) {
            Preconditions   = { { "isRested", false },    { "hasAnything", false } },
            Effects         = { { "isRested", true } },
        });

        /*
            Idle
        */

        AddAction(new Idle(_game, _movement) {
            Preconditions   = { { "isWorking", false }, { "hasAnything", false } },
            Effects         = { { "isWorking", true } },
            Cost = 99999 // last resort
        }); 

        /*
            Farming
        */
        // AddAction(new FillCoop(_game, _movement, this, thing.inventory) {
        //     Preconditions   = { { "hasChicken", true } },
        //     Effects         = { { "isWorking", true }, { "hasChicken", false } }
        // });

        /*
            Survival
        */

        AddAction(new DrinkFromStream(_game, _movement) {
            Preconditions   = { { "isThirsty", true } },
            Effects         = { { "isThirsty", false } }
        }); 

        AddAction(new EastSomething(_game, thing.inventory) {
            Preconditions   = { { "isHungry", true },   { "hasEdibleThing", true } },
            Effects         = { { "isHungry", false } }
        }); 

        /*
            Resources
        */

        AddAction(new GetResource(_game, _movement, TypeOfThing.Rock, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Stone },   { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Mushroom, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Mushroom },  { "hasAnything", true }, { "hasEdibleThing", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Tree, thing.inventory) {
            Preconditions   = { { "hasThing", TypeOfThing.Axe } },
            Effects         = { { "hasThing", TypeOfThing.Wood },  { "hasAnything", true }, }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.FallenWood, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Wood }, { "hasWood", true },   { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Wood, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Wood },    { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Clay, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Clay },    { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Ore, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Ore },    { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Iron, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Iron },  { "hasIron", true },   { "hasAnything", true } }
        }); 

        AddAction(new GetResource(_game, _movement, TypeOfThing.Stone, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Stone },   { "hasAnything", true } }
        });

        AddAction(new GetResource(_game, _movement, TypeOfThing.Axe, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Axe },   { "hasAnything", true } }
        });
        

        /*
            Factories
        */

        // AddAction(new FillFactoryHopper(_game, _movement, TypeOfThing.Ore, thing.inventory) {
        //     Preconditions   = { { "hasThing", TypeOfThing.Ore } },
        //     Effects         = { { "hasAnything", false }, { "isWorking", true } }
        // });

        // AddAction(new FillFactoryHopper(_game, _movement, TypeOfThing.Iron, thing.inventory) {
        //     Preconditions   = { { "hasThing", TypeOfThing.Iron } },
        //     Effects         = { { "hasAnything", false }, { "isWorking", true } },
        // });

        // AddAction(new FillFactoryHopper(_game, _movement, TypeOfThing.Wood, thing.inventory) {
        //     Preconditions   = { { "hasThing", TypeOfThing.Wood } },
        //     Effects         = { { "hasAnything", false }, { "isWorking", true } },
        // });
        
        AddAction(new SubmitFactoryJob(_game, _movement, TypeOfThing.ClayForge, TypeOfThing.Iron, thing.inventory) {
            Preconditions   = { { "hasWood", true }, { "hasIron", true } },
            Effects         = { { "hasThig", TypeOfThing.Iron }, { "isWorking", true } },
        });

        AddAction(new SubmitFactoryJob(_game, _movement, TypeOfThing.Workbench, TypeOfThing.Axe, thing.inventory) {
            Preconditions   = { { "hasAnything", false } },
            Effects         = { { "hasThing", TypeOfThing.Axe }, { "isWorking", true } },
        });

        /*
            Construction
        */
        foreach(var type in new TypeOfThing[] { 
            TypeOfThing.Wood, 
            TypeOfThing.Stone, 
            TypeOfThing.Clay 
            })
        {
            AddAction(new Construct(_game, _movement, type, _thing) {
                Preconditions   = { { "hasThing", type } },
                Effects         = { { "isWorking", true }, }
            });
        }
      

        // construct something that requries no resources
        AddAction(new Construct(_game, _movement, TypeOfThing.None, _thing) {
            Effects         = { { "isWorking", true }, }
        });
    }

    public override Dictionary<string, object> GetGoal()
    {
        _goal["isWorking"] = true;
        _goal["isHungry"] = false;
        _goal["isRested"] = true;
        _goal["isWarm"] = true;
        _goal["isThirsty"] = false;

        return _goal;
    }

    public override Dictionary<string, object> GetWorldState()
    {
        /*
            Resources
        */
        
        _world["hasThing"] = _thing.inventory.IsHoldingSomething() ?
            _thing.inventory.Holding.type : TypeOfThing.None;

        _world["hasEdibleThing"] = 
            _thing.inventory.IsHoldingSomething() && 
            _thing.inventory.IsHoldingSomethingToEat();

        _world["hasAnything"] = _thing.inventory.IsHoldingSomething();

        /*
            Survival
        */

        _world["isThirsty"] = _thirst < 0f;
        _world["isHungry"] = _hunger < 0f;
        _world["isRested"] = _rest > 0f;
        _world["isWarm"] = !_needs.IsCold();
        _world["isWorking"] = false;

        
        return _world;
    }

    public override void ActionCompleted(GOAPAction action)
    {

        if(action.Effects.ContainsKey("isRested") && (bool)action.Effects["isRested"])
            _rest = 0f;        
           

        // thirsty after sleeping
        if(action is Sleep)
        {
            _thirst = -1f;
            _hunger = -1f;
        }
        
        if(action is DrinkFromStream)
            _thirst = 0f;
        
        if(action is EastSomething)
            _hunger = 0f;

    }

    public override void Update()
    {
        base.Update();

        if(_needs.IsDead())
        {
            PauseAgent();
            _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        
        if(_game.WorldTime.GetTimeOfDay() == TimeOfDay.Night && _rest >= 0f)
        {
            _rest = -1f;
        }

        var label = Fullname + "\n";

        if(_needs.IsDead())
        {
            label += $"DEAD\n";
            label += _needs.GetReasonsForDeath();
        }
        else if(CurentAction != null)
            label += $"{CurentAction.ToString()}\n";

        label += string.Format("hunger: {0}\n", _hunger);
        label += string.Format("thirst: {0}\n", _thirst);
        label += string.Format("warmth: {0}\n", _needs.Warmth);
        label += string.Format("rest: {0}\n", _rest);

        SetLabel(label);
    }

    public override void DrawGizmos()
    {
#if UNITY_EDITOR
        
        var text = "";

        if(_current != null)
        {
            text += string.Format("current action: {0}\n", _current.ToString());
        }

        text += string.Format("hunger: {0}\n", _hunger);
        text += string.Format("thirst: {0}\n", _thing);
        text += string.Format("warmth: {0}\n", _needs.Warmth);
        text += string.Format("rest: {0}\n", _rest);

        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        // current actions
        var position = _thing.transform.position + Vector3.up;
        UnityEditor.Handles.Label(position, text, style);
#endif   
    }
}

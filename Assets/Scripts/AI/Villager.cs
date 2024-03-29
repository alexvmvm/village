﻿using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{
    public class Villager : GOAPAgent
    {
        public string Firstname { get { return _firstname; } }
        public string Lastname { get { return _lastname; } }
        public string Fullname { get { return Firstname + " " + Lastname; } }
        public Needs Needs { get; private set; }
        private string _firstname;
        private string _lastname;
        private bool _requestedResidence;
        private VillageManager _villagerManager;
        private Movement _movement;
        private Thing _thing;
        private Inventory _inventory;
        private Game _game;

        /* 
            Survival
        */


        public override void Awake()
        {
            base.Awake();

            _thing = GetComponent<Thing>();

            _game = FindObjectOfType<Game>();
            
            _movement = transform.gameObject.AddComponent<Movement>();
            _inventory = _thing.Inventory;

            Needs = transform.gameObject.AddComponent<Needs>();

            _firstname = NameGenerator.GenerateFirstName();
            _lastname = NameGenerator.GenerateLastName();

            _thing.name = string.Format("{0} {1}", _firstname, _lastname);

            _villagerManager = MonoBehaviour.FindObjectOfType<VillageManager>();

            //IdleAction = new Idle(this, _game, _movement);
            
            AddGoal(new IdleGoal());
            AddGoal(new WorkingGoal());

            AddGoal(new DrinkGoal(Needs));
            AddGoal(new EatGoal(Needs));
            AddGoal(new RestGoal(_game, Needs));

            /*
                Misc
            */

            AddAction(new Drop(this, _game));
            AddAction(new Sleep(this, _game, _thing, _movement, this, Needs));
            AddAction(new Idle(this, _game, _movement));


            AddAction(new ChopWood(this, _game, _movement));
            AddAction(new PickMushroom(this, _game, _movement));
            AddAction(new GetClayFromGround(this, _game, _movement));

            /*
                Survival
            */

            AddAction(new DrinkFromStream(this, _game, _movement));
            AddAction(new EastSomething(this, _game, _thing));
            

            AddAction(new HasEdibleThing(this, _game, TypeOfThing.Mushroom));


            /*
                Resources
            */
            var resources = new TypeOfThing[] 
            {
                TypeOfThing.Water,
                TypeOfThing.Stream,
                TypeOfThing.Rock,
                TypeOfThing.Stone,
                TypeOfThing.MushroomGrowing,
                TypeOfThing.Mushroom,
                TypeOfThing.Tree,
                TypeOfThing.FallenWood,
                TypeOfThing.Wood,
                TypeOfThing.WoodenPlanks,
                TypeOfThing.ClayFloor,
                TypeOfThing.Clay,
                TypeOfThing.OreFloor,
                TypeOfThing.Ore,
                TypeOfThing.Iron,
                TypeOfThing.CabbageSeed,
                TypeOfThing.PotatoSeed,
                TypeOfThing.CarrotSeed,
                TypeOfThing.PumpkinSeed,
                TypeOfThing.HopSeed,
                TypeOfThing.TomatoSeed,
                TypeOfThing.Cabbage,
                TypeOfThing.PotatoSeed,
                TypeOfThing.Carrot,
                TypeOfThing.Hops,
                TypeOfThing.Axe,
                TypeOfThing.Hoe,
                TypeOfThing.WateringPot
            };

            foreach(var resource in resources)
            {
                AddAction(new GetThing(this, _game, _thing, _movement, resource));
                //AddAction(new GetThingToMoveToStorage(this, _game, _thing, _movement, resource, this));
                //AddAction(new FillStorage(this, _game, _movement, _thing.Inventory, resource));
            }

            /*
                Factory Job
            */

            // var factoryJobs = new List<Tuple<TypeOfThing, TypeOfThing, TypeOfThing, bool>>()
            // {
            //     Tuple.Create(TypeOfThing.ClayForge, TypeOfThing.Ore, TypeOfThing.Iron, false),
            //     Tuple.Create(TypeOfThing.Workbench, TypeOfThing.Iron, TypeOfThing.Axe, true),
            //     Tuple.Create(TypeOfThing.Workbench, TypeOfThing.Iron, TypeOfThing.Hoe, true),
            //     Tuple.Create(TypeOfThing.CarpentersBench, TypeOfThing.Wood, TypeOfThing.WoodenPlanks, true),
            //     Tuple.Create(TypeOfThing.Kiln, TypeOfThing.Clay, TypeOfThing.WateringPot, false)
            // };

            // foreach(var job in factoryJobs)
            // {
            //     AddAction(new SubmitFactoryJob(this, _game, _thing, _movement, job.Item1, job.Item2, job.Item3, job.Item4));
            // }

            /*
                Construction
            */

            AddAction(new Construct(this, _game, _movement, TypeOfThing.Wood, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.WoodenPlanks, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.Stone, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.Clay, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.Hoe, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.CabbageSeed, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.PotatoSeed, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.TomatoSeed, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.PumpkinSeed, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.CarrotSeed, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.Water, _thing));
            AddAction(new Construct(this, _game, _movement, TypeOfThing.None, _thing));
        }

        public override void UpdateState(Dictionary<string, object> state)
        {
            /*
                Resources
            */
            // foreach(InventorySlot slot in Enum.GetValues(typeof(InventorySlot)))
            // {
            //     state[GOAPAction.Effect.IS_HOLDING_THING + slot] = _inventory.IsHoldingThing(slot);
            //     state[GOAPAction.Effect.HAS_THING + slot] = _inventory.GetTypeOfThing(slot);

            // }

            state[GOAPAction.Effect.HAS_THING] = _inventory.GetHoldingThingType();
            state[GOAPAction.Effect.HAS_EDIBLE_THING] = _inventory.IsHoldingSomethingToEat();


            /*
                Survival
            */

            state[GOAPAction.Effect.IS_THIRSTY] = Needs.Thirst < 0f;
            state[GOAPAction.Effect.IS_HUNGRY] = Needs.Hunger < 0f;
            state[GOAPAction.Effect.IS_RESTED] = Needs.Rest > 0f;
            // state[GOAPAction.Effect.IS_WARM] = !_needs.IsCold();
            state[GOAPAction.Effect.IS_WORKING] = false;
        }

        // public override void GetGoalState(Dictionary<string, object> goal)
        // {
        //     goal[GOAPAction.Effect.IS_WORKING] = true;
        //     goal[GOAPAction.Effect.IS_HUNGRY] = false;
        //     goal[GOAPAction.Effect.IS_RESTED] = true;
        //     goal[GOAPAction.Effect.IS_WARM] = true;
        //     goal[GOAPAction.Effect.IS_THIRSTY] = false;
        // }

        // public override void GetWorldState(Dictionary<string, object> world)
        // {
        //     /*
        //         Resources
        //     */
        //     foreach(InventorySlot slot in Enum.GetValues(typeof(InventorySlot)))
        //     {
        //         world[GOAPAction.Effect.IS_HOLDING_THING + slot] = _inventory.IsHoldingThing(slot);
        //         world[GOAPAction.Effect.HAS_THING + slot] = _inventory.GetTypeOfThing(slot);

        //     }

        //     world[GOAPAction.Effect.HAS_EDIBLE_THING] = _inventory.IsHoldingSomethingToEat();


        //     /*
        //         Survival
        //     */

        //     world[GOAPAction.Effect.IS_THIRSTY] = _thirst < 0f;
        //     world[GOAPAction.Effect.IS_HUNGRY] = _hunger < 0f;
        //     world[GOAPAction.Effect.IS_RESTED] = _rest > 0f;
        //     world[GOAPAction.Effect.IS_WARM] = !_needs.IsCold();
        //     world[GOAPAction.Effect.IS_WORKING] = false;
        // }

        public override void ActionCompleted(GOAPAction action)
        {

            // if (action.Effects.ContainsKey("isRested") && (bool)action.Effects["isRested"])
            //     _needs.SetRest(0f);

            // thirsty after sleeping
            if (action is Sleep)
            {
                Needs.SetThirst(-1f);
                Needs.SetHunger(-1f);
                Needs.SetRest(0f);
            }

            if (action is DrinkFromStream)
                Needs.SetThirst(0f);

            if (action is EastSomething)
                Needs.SetHunger(0f);

        }

        public override void Update()
        {
            base.Update();

            if (Needs.IsDead())
            {
                PauseAgent();
                _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            // if (_game.WorldTime.TimeOfDay == TimeOfDay.Night)
            //     _needs.SetRest(-1f);
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            #if UNITY_EDITOR

            var label = Fullname + "\n";

            if (CurrentAction != null)
                label += $"{CurrentAction.ToString()}\n";

            var style = new GUIStyle();
            style.fontSize = 10;
            style.normal.textColor = Color.white;

            // current actions
            var position = _thing.transform.position + Vector3.up + Vector3.right;
            UnityEditor.Handles.Label(position, label, style);

            #endif
        }

        //         void  OnDrawGizmos()
        //         {
        // #if UNITY_EDITOR

        //             var text = "";

        //             if (_current != null)
        //             {
        //                 text += string.Format("current action: {0}\n", _current.ToString());
        //             }

        //             text += string.Format("hunger: {0}\n", _hunger);
        //             text += string.Format("thirst: {0}\n", _thing);
        //             text += string.Format("warmth: {0}\n", _needs.Warmth);
        //             text += string.Format("rest: {0}\n", _rest);

        //             var style = new GUIStyle();
        //             style.fontSize = 10;
        //             style.normal.textColor = Color.white;

        //             // current actions
        //             var position = _thing.transform.position + Vector3.up;
        //             UnityEditor.Handles.Label(position, text, style);
        // #endif
        //         }
    }

}

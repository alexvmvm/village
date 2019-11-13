using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{
    public class Villager : ThingAgent
    {
        public string Firstname { get { return _firstname; } }
        public string Lastname { get { return _lastname; } }
        public string Fullname { get { return Firstname + " " + Lastname; } }
        private string _firstname;
        private string _lastname;
        private bool _requestedResidence;
        private Dictionary<string, object> _goal;
        private Dictionary<string, object> _world;
        private VillageManager _villagerManager;
        private Movement _movement;
        private Thing _thing;
        private Needs _needs;
        private Inventory _inventory;

        /* 
            Survival
        */

        private float _thirst;
        private float _hunger;
        private float _warmth;
        private float _rest;

        public override void Awake()
        {
            base.Awake();

            _thing = GetComponent<Thing>();
            
            _movement = transform.gameObject.AddComponent<Movement>();
            _inventory = _thing.Inventory;

            _needs = new Needs(_game);

            _firstname = NameGenerator.GenerateFirstName();
            _lastname = NameGenerator.GenerateLastName();

            _thing.name = string.Format("{0} {1}", _firstname, _lastname);

            _goal = new Dictionary<string, object>();
            _world = new Dictionary<string, object>();

            _villagerManager = MonoBehaviour.FindObjectOfType<VillageManager>();
            


            /*
                Misc
            */

            AddAction(new Drop(this, _game, _thing)
            {
                Preconditions = { { "hasFullInventory", true } },
                Effects = { { "hasFullInventory", false } },
            });

            AddAction(new Sleep(this, _game, _thing, _movement, this, _needs)
            {
                Preconditions = { { "isRested", false }, { "hasFullInventory", false } },
                Effects = { { "isRested", true } },
            });

            /*
                Idle
            */

            AddAction(new Idle(this, _game, _movement)
            {
                Preconditions = { { "isWorking", false }, { "hasFullInventory", false } },
                Effects = { { "isWorking", true } },
                Cost = 99999 // last resort
            });

            /*
                Survival
            */

            AddAction(new DrinkFromStream(this, _game, _movement)
            {
                Preconditions = { { "isThirsty", true } },
                Effects = { { "isThirsty", false } }
            });

            AddAction(new EastSomething(this, _game, _thing)
            {
                Preconditions = { { "isHungry", true }, { "hasEdibleThing", true } },
                Effects = { { "isHungry", false } }
            });

            /*
                Resources
            */
            var resources = new TypeOfThing[] 
            {
                TypeOfThing.Rock,
                TypeOfThing.Stone,
                TypeOfThing.MushroomGrowing,
                TypeOfThing.Mushroom,
                TypeOfThing.Tree,
                TypeOfThing.FallenWood,
                TypeOfThing.Wood,
                TypeOfThing.Clay,
                TypeOfThing.Ore,
                TypeOfThing.Iron,
                TypeOfThing.CabbageSeed,
                TypeOfThing.Axe,
                TypeOfThing.Hoe
            };

            foreach(var resource in resources)
            {
                var example = Assets.CreateThingConfig(resource);
                var effects = new Dictionary<string, object>();
                effects.Add("hasThing", example.Produces);
                effects.Add("hasFullInventory", true);
                if(example.Edible) 
                    effects.Add("hasEdibleThing", true);
                AddAction(new GetResource(this, _game, _thing, _movement, resource, this)
                {
                    Preconditions = { { "hasFullInventory", false } },
                    Effects = effects
                });
            }

            var factoryJobs = new List<Tuple<TypeOfThing, TypeOfThing, TypeOfThing>>()
            {
                Tuple.Create(TypeOfThing.ClayForge, TypeOfThing.Ore, TypeOfThing.Iron),
                Tuple.Create(TypeOfThing.Workbench, TypeOfThing.Iron, TypeOfThing.Axe),
                Tuple.Create(TypeOfThing.Workbench, TypeOfThing.Iron, TypeOfThing.Hoe)
            };

            /*
                Factory Job
            */

            foreach(var job in factoryJobs)
            {
                var factory = job.Item1;
                var requires = job.Item2;
                var produces = job.Item3;

                var preconditions = new Dictionary<string, object>
                {
                    { "hasThing", requires }
                };

                var effects = new Dictionary<string, object>
                {
                    { "hasThing", produces },
                    { "isWorking", true }
                };

                AddAction(new SubmitFactoryJob(this, _game, _thing, _movement, factory, produces, false)
                {
                    Preconditions = preconditions,
                    Effects = effects,
                    Cost = 1
                });
            }


            /*  
                Storage
            */

            // foreach(var resource in resources)
            // {
            //     AddAction(new GetResourceToMoveToStorage(_game, thing, _movement, resource, this)
            //     {
            //         Preconditions = { { "hasFullInventory", false } },
            //         Effects = { { "hasThing", resource }, { "hasThingForStorage", true } }
            //     });
            // }


            /*
                Construction
            */

            AddAction(new Construct(this, _game, _movement, TypeOfThing.Wood, _thing)
            {
                Preconditions = { { "hasThing", TypeOfThing.Wood } },
                Effects = { { "isWorking", true }, }
            });

            AddAction(new Construct(this, _game, _movement, TypeOfThing.Stone, _thing)
            {
                Preconditions = { { "hasThing", TypeOfThing.Stone } },
                Effects = { { "isWorking", true }, }
            });

            AddAction(new Construct(this, _game, _movement, TypeOfThing.Clay, _thing)
            {
                Preconditions = { { "hasThing", TypeOfThing.Clay } },
                Effects = { { "isWorking", true }, }
            });

            AddAction(new Construct(this, _game, _movement, TypeOfThing.Hoe, _thing)
            {
                Preconditions = { { "hasThing", TypeOfThing.Hoe } },
                Effects = { { "isWorking", true }, }
            });

            AddAction(new Construct(this, _game, _movement, TypeOfThing.CabbageSeed, _thing)
            {
                Preconditions = { { "hasThing", TypeOfThing.CabbageSeed } },
                Effects = { { "isWorking", true }, }
            });

            AddAction(new Construct(this, _game, _movement, TypeOfThing.None, _thing)
            {
                Effects = { { "isWorking", true }, }
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

            _world["hasThing"] = _inventory.IsHoldingSomething() ?
                _inventory.Holding.Config.TypeOfThing : TypeOfThing.None;

            _world["hasEdibleThing"] =
                _inventory.IsHoldingSomething() &&
                _inventory.IsHoldingSomethingToEat();

            _world["hasFullInventory"] = _inventory.IsHoldingSomething();

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

            if (action.Effects.ContainsKey("isRested") && (bool)action.Effects["isRested"])
                _rest = 0f;


            // thirsty after sleeping
            if (action is Sleep)
            {
                _thirst = -1f;
                _hunger = -1f;
            }

            if (action is DrinkFromStream)
                _thirst = 0f;

            if (action is EastSomething)
                _hunger = 0f;

        }

        public override void Update()
        {
            base.Update();

            if (_needs.IsDead())
            {
                PauseAgent();
                _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            if (_game.WorldTime.GetTimeOfDay() == TimeOfDay.Night && _rest >= 0f)
            {
                _rest = -1f;
            }

            var label = Fullname + "\n";

            if (_needs.IsDead())
            {
                label += $"DEAD\n";
                label += _needs.GetReasonsForDeath();
            }
            else if (CurentAction != null)
                label += $"{CurentAction.ToString()}\n";

            label += string.Format("hunger: {0}\n", _hunger);
            label += string.Format("thirst: {0}\n", _thirst);
            label += string.Format("warmth: {0}\n", _needs.Warmth);
            label += string.Format("rest: {0}\n", _rest);

            SetLabel(label);
        }

        void  OnDrawGizmos()
        {
#if UNITY_EDITOR

            var text = "";

            if (_current != null)
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

}

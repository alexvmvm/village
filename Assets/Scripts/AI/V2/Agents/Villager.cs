using System.Collections;
using System.Collections.Generic;
using SwordGC.AI.Goap;
using UnityEngine;
using Village.Things;

namespace Village.AI.V2
{
    public class Villager : GoapAgent
    {
        public string Firstname { get; protected set; }
        public string Lastname { get; protected set; }
        public string Fullname { get { return Firstname + " " + Lastname; } }

        private Game _game;
        private Thing _thing;
        private Movement _movement;
        private GameObject _currentTarget;
        public override void Awake()
        {
            base.Awake();

            _game = FindObjectOfType<Game>();
            _thing = GetComponent<Thing>();
            _movement = gameObject.AddComponent<Movement>();

            Firstname = NameGenerator.GenerateFirstName();
            Lastname = NameGenerator.GenerateLastName();

            goals.Add(GoapGoal.Goals.CONSTRUCT, new Construct());

            possibleActions.Add(new DropThing(this, _thing.Inventory));

            // get these things
            possibleActions.Add(new GetThing(this, _thing, _game, TypeOfThing.FallenWood, TypeOfThing.Wood));
            //possibleActions.Add(new GetThing(this, _thing, _game, TypeOfThing.MushroomGrowing, TypeOfThing.Mushroom));

            // construct these things that require the supplied building material
            possibleActions.Add(new ConstructThing(this, _thing, _game, TypeOfThing.Wood));
            //possibleActions.Add(new ConstructThing(this, _thing, _game, TypeOfThing.Mushroom));
        }

        protected override void MoveEnd(GoapAction nextAction)
        {
            // _movement.SetStopped(true);
            // _movement.CancelCurrentPath();
        }

        protected override void Move(GoapAction nextAction)
        {
            if(nextAction != null && nextAction.target != null && nextAction.target != _currentTarget)
            {
                _currentTarget = nextAction.target;
                _movement.CancelCurrentPath();
                _movement.MoveTo(nextAction.target.transform.position, nextAction.requiredRange);
                _movement.SetStopped(false);
               
            }
        }    

        void Update()
        {
            dataSet.SetData(GoapAction.Effects.EMPTY_INVENTORY, !_thing.Inventory.IsHoldingSomething());
            dataSet.SetData($"{GoapAction.Effects.HAS_THING}_{TypeOfThing.FallenWood}", _thing.Inventory.IsHolding(TypeOfThing.FallenWood));
            dataSet.SetData($"{GoapAction.Effects.HAS_THING}_{TypeOfThing.Wood}", _thing.Inventory.IsHolding(TypeOfThing.Wood));
        }

        void OnDrawGizmos()
        {
            if(_currentTarget != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(_currentTarget.transform.position, transform.position);
            }
        }
    }
}


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

        public override void Awake()
        {
            base.Awake();

            _game = FindObjectOfType<Game>();
            _thing = GetComponent<Thing>();

            Firstname = NameGenerator.GenerateFirstName();
            Lastname = NameGenerator.GenerateLastName();

            goals.Add(GoapGoal.Goals.CONSTRUCT, new Construct());

            possibleActions.Add(new DropThing(this, _thing.Inventory));

            // get these things
            possibleActions.Add(new GetThing(this, _thing, _game, TypeOfThing.FallenWood));
            possibleActions.Add(new GetThing(this, _thing, _game, TypeOfThing.Mushroom));

            // construct these things that require the supplied building material
            possibleActions.Add(new ConstructThing(this, _thing, _game, TypeOfThing.FallenWood));
            possibleActions.Add(new ConstructThing(this, _thing, _game, TypeOfThing.Mushroom));
        }

        protected override void Move(GoapAction nextAction)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextAction.target.transform.position, Time.deltaTime);
        }    

        void Update()
        {
            dataSet.SetData(GoapAction.Effects.EMPTY_INVENTORY, !_thing.Inventory.IsHoldingSomething());
            dataSet.SetData($"{GoapAction.Effects.HAS_THING}_{TypeOfThing.FallenWood}", _thing.Inventory.IsHolding(TypeOfThing.FallenWood));
        }
    }
}


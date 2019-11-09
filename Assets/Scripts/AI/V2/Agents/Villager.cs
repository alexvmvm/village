using System.Collections;
using System.Collections.Generic;
using SwordGC.AI.Goap;
using UnityEngine;

namespace Village.AI.V2
{
    public class Villager : GoapAgent
    {
        public string Firstname { get; protected set; }
        public string Lastname { get; protected set; }
        public string Fullname { get { return Firstname + " " + Lastname; } }

        public override void Awake()
        {
            base.Awake();

            Firstname = NameGenerator.GenerateFirstName();
            Lastname = NameGenerator.GenerateLastName();

            goals.Add(GoapGoal.Goals.CONSTRUCT, new Construct());

            possibleActions.Add(new DropThing(this));

            // get these things
            possibleActions.Add(new GetThing(this, TypeOfThing.FallenWood));
            possibleActions.Add(new GetThing(this, TypeOfThing.Mushroom));

            // construct these things that require the supplied building material
            possibleActions.Add(new ConstructThing(this, TypeOfThing.FallenWood));
            possibleActions.Add(new ConstructThing(this, TypeOfThing.Mushroom));
        }

        protected override void Move(GoapAction nextAction)
        {

        }    
    }
}


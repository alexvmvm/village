using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

namespace Village.Things
{

    public class Inventory : ITrait
    {
        public Thing Holding { get; private set; }
        private Thing _parent;

        public Inventory(Thing parent)
        {
            _parent = parent;
        }

        public void HoldThing(Thing thing)
        {
            Holding = thing;

            //if (_holding.Agent != null)
            //    _holding.Agent.PauseAgent();

            Holding.transform.SetParent(_parent.transform);
            Holding.transform.localPosition = Vector3.up;
        }

        public Thing Drop()
        {
            if (Holding != null)
            {
                var thing = Holding;
                Holding.transform.SetParent(null);
                Holding = null;
                return thing;
            }

            return null;
        }

        public bool IsHoldingSomething()
        {
            return Holding != null;
        }

        public bool IsHoldingSomethingToEat()
        {
            return Holding != null && Holding.Config.Edible;
        }

        public bool IsHolding(TypeOfThing type)
        {
            return Holding != null && Holding.Config.TypeOfThing == type;
        }

        public bool IsHoldingTool()
        {
            return IsHoldingSomething() && (IsHolding(TypeOfThing.Hoe) || IsHolding(TypeOfThing.Axe));
        }

        public void Setup()
        {

        }

        public void Update()
        {

        }

        public void DrawGizmos()
        {

        }
    }

}
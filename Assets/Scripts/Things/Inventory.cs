using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

namespace Village.Things
{

    public class Inventory : MonoBehaviour
    {
        public Thing Holding { get; private set; }

        public void HoldThing(Thing thing)
        {
            Holding = thing;

            //if (_holding.Agent != null)
            //    _holding.Agent.PauseAgent();

            Holding.transform.SetParent(transform);
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
            return IsHoldingSomething() && Holding.Config.Tool;
        }
    }

}
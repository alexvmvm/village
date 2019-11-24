using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Village.AI;

namespace Village.Things
{

    public class Inventory : MonoBehaviour
    {
        public Thing Holding { get; private set; }
        public Thing Tool { get; private set; }

        private Game _game;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        public void HoldThing(Thing thing)
        {
            Holding = thing;
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
                
                var position = _game.FindNearestLoosePosition(transform.position.ToVector2IntFloor());

                if(position.HasValue)
                {
                    thing.transform.position = position.Value.ToVector3();
                }
                
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

        public bool IsHoldingTool()
        {
            return IsHoldingSomething() && Holding.Config.Tool;
        }
    }

}
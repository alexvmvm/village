using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Village.AI;
using Village.Things.Config;

namespace Village.Things
{
    public class Inventory : MonoBehaviour
    {
        private Thing _holding;
        private Game _game;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        public void Hold(Thing thing)
        {
            _holding = thing;
            thing.transform.SetParent(transform);
            thing.transform.localPosition = Vector3.up;
        }

        public bool IsHoldingThing()
        {
            return _holding != null;
        }

        public TypeOfThing GetHoldingThingType()
        {
            return IsHoldingThing() ? _holding.Config.TypeOfThing : TypeOfThing.None;
        }

        public bool IsHoldingSomethingToEat()
        {
            return IsHoldingThing() && _holding.Config.Edible;
        }
        

        public Thing GetHoldingThing()
        {
            return _holding;
        }

        public Thing Drop()
        {
            if(!IsHoldingThing())
                return null;

            var thing = GetHoldingThing();
            thing.transform.SetParent(null);
            
            var position = _game.FindNearestLoosePosition(transform.position.ToVector2IntFloor());

            if(position.HasValue)
            {
                thing.transform.position = position.Value.ToVector3();
            }
            
            return thing;
        }
    }

}
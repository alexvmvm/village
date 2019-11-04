using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.AI;

namespace Village.Things
{

    public class Inventory : ITrait
    {
        public Thing Holding { get { return _holding; } }
        private Thing _parent;

        private Thing _holding;

        public Inventory(Thing parent)
        {
            _parent = parent;
        }

        public void HoldThing(Thing thing)
        {
            _holding = thing;

            if (_holding.GetTrait<Agent>() != null)
                _holding.GetTrait<Agent>().PauseAgent();

            _holding.transform.SetParent(_parent.transform);
            _holding.transform.localPosition = Vector3.up;
        }

        public Thing Drop()
        {
            if (_holding != null)
            {
                var thing = _holding;
                _holding.transform.SetParent(null);
                _holding = null;
                return thing;
            }

            return null;
        }

        public bool IsHoldingSomething()
        {
            return _holding != null;
        }

        public bool IsHoldingSomethingToEat()
        {
            return _holding != null && _holding.edible;
        }

        public bool IsHolding(TypeOfThing type)
        {
            return _holding != null && _holding.type == type;
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
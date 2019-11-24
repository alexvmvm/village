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
        private Game _game;
        private Dictionary<InventorySlot, Thing> _inventory;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _inventory = new Dictionary<InventorySlot, Thing>();
        }

        public void Hold(Thing thing)
        {
            _inventory[thing.Config.InventorySlot] = thing;

            thing.transform.SetParent(transform);
            thing.transform.localPosition = Vector3.up;
        }

        public bool IsHoldingThing(InventorySlot slot)
        {
            return _inventory.ContainsKey(slot) && _inventory[slot] != null;
        }

        public TypeOfThing GetTypeOfThing(InventorySlot slot)
        {
            return IsHoldingThing(slot) ?
                _inventory[slot].Config.TypeOfThing :
                TypeOfThing.None;
        }

        public bool IsHoldingSomethingToEat()
        {
            return IsHoldingThing(InventorySlot.Hands) && GetHoldingThing(InventorySlot.Hands).Config.Edible;
        }
        

        public Thing GetHoldingThing(InventorySlot slot)
        {
            return _inventory.ContainsKey(slot) ? _inventory[slot] : null;
        }

        public Thing Drop(InventorySlot slot)
        {
            if(!IsHoldingThing(slot))
                return null;

            var thing = GetHoldingThing(slot);
            thing.transform.SetParent(null);
            
            var position = _game.FindNearestLoosePosition(transform.position.ToVector2IntFloor());

            if(position.HasValue)
            {
                thing.transform.position = position.Value.ToVector3();
            }

            _inventory.Remove(slot);
            
            return thing;
        }
    }

}
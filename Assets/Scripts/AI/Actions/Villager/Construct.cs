using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public class Construct : GOAPAction
    {
        private Movement _movement;
        private Thing _target;
        private bool _started;
        private bool _isDone;
        private TypeOfThing _type;
        private Thing _thing;
        private Inventory _inventory;

        public Construct(Game game, Movement movement, TypeOfThing type, Thing thing) : base(game)
        {
            _movement = movement;
            _type = type;
            _thing = thing;
            _inventory = _thing.Inventory;
        }

        public override bool IsDone()
        {
            return _isDone;
        }

        public override bool IsPossibleToPerform()
        {

            _target = _game.QueryThings()   
                .Where(t => t.Construction != null && t.Construction.Requires == _type)
                .OrderBy(v => Vector2.Distance(v.transform.position, _movement.transform.position))
                .FirstOrDefault();

            return _target != null && _movement.IsPathPossible(_target.transform.position);
        }

        public override bool Perform()
        {
            if (!_started)
            {
                _movement.CancelCurrentPath();
                _movement.MoveTo(_target.transform.position);
                _started = true;
            }

            if (_movement.FailedToFollowPath)
                return false;

            if (_movement.ReachedEndOfPath)
            {
                _target.Construction.Construct();

                if (_inventory.IsHoldingSomething() && !_inventory.IsHoldingTool())
                {
                    var resource = _inventory.Holding;
                    resource.Hitpoints -= 1;

                    if (resource.Hitpoints == 0)
                    {
                        _inventory.Drop();
                        _game.Destroy(resource);
                    }
                }
                _isDone = true;
            }


            return true;
        }

        public override void Reset()
        {
            _started = false;
            _isDone = false;
        }

        public override string ToString()
        {
            if (_target == null || _target.Construction == null)
                return base.ToString();

            return $"Building {_target.Construction.BuildType.ToString()}";
        }
    }

}

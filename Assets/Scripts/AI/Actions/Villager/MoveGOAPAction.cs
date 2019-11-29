using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{

    public abstract class MoveGOAPAction : GOAPAction
    {
        private Movement _movement;
        protected Thing _target;
        private bool _started;
        private bool _isDone;

        public MoveGOAPAction(GOAPAgent agent, Game game, Movement movement) : base(agent, game)
        {
            _movement = movement;
        }

        public virtual void BeforeStartMoving()
        {
            
        }

        public abstract bool Filter(Thing thing);

        public abstract bool PerformAtTarget();

        public override bool IsDone()
        {
            return _isDone;
        }

        public override bool IsPossibleToPerform()
        {
            _target = _game.IsPathPossibleToThing(_agent.transform.position.ToVector2IntFloor(), Filter);
            return _target != null;
        }

        public override bool Perform()
        {
            if (!_started)
            {
                BeforeStartMoving();

                if(_target == null)
                    return false;

                _movement.CancelCurrentPath();
                _movement.MoveTo(_target.transform.position);
                _started = true;
            }

            if (_movement.FailedToFollowPath)
                return false;

            if (_movement.ReachedEndOfPath)
            {
                if(!PerformAtTarget())
                    return true;

                _isDone = true;
                 
            }

            return true;
        }

        public override void Reset()
        {
            _target = null;
            _started = false;
            _isDone = false;
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            if(_target != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(_target.transform.position, _agent.transform.position);
            }
        }
    }

}

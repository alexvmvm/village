using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village.Things;

namespace Village.AI
{
    public class Sleep : GOAPAction
    {
        private Thing _thing;
        private Movement _movement;
        private bool _started;
        private Vector3 _target;
        private Villager _villager;
        private Needs _needs;

        public Sleep(GOAPAgent agent, Game game, Thing thing, Movement movement, Villager villager, Needs needs) : base(agent, game)
        {
            _thing = thing;
            _movement = movement;
            _villager = villager;
            _needs = needs;

            Goal = GOAPGoal.Goal.IS_RESTED;

            Preconditions.Add(GOAPAction.Effect.IS_RESTED, false);
            Preconditions.Add(GOAPAction.Effect.HAS_THING, TypeOfThing.None);
            
            Effects.Add(GOAPAction.Effect.IS_RESTED, true);
        }


        Thing FindBed()
        {
            return _game.QueryThings()
                .Where(t => 
                    t.Config.TypeOfThing == TypeOfThing.Bed || 
                    t.Config.TypeOfThing == TypeOfThing.ForagedBed && 
                    (t.ownedBy == _villager.Fullname || string.IsNullOrEmpty(t.ownedBy)))
                .OrderByDescending(t => t.ownedBy == _villager.Fullname)
                .FirstOrDefault();
        }

        public override bool IsDone()
        {
            return _game.WorldTime.TimeOfDay == TimeOfDay.Day;
        }

        public override bool IsPossibleToPerform()
        {
            return _game.WorldTime.TimeOfDay == TimeOfDay.Night;
        }

        public override bool Perform()
        {
            if(!_started)
            {
                var bed = FindBed();

                if(bed == null)
                {
                    _target = _thing.transform.position; 
                    
                    _needs.Trigger(NeedsEvent.SLEPT_OUTSIDE);
                }
                else
                {
                    _target = bed.transform.position;   
                    if(bed.ownedBy != _villager.Fullname)
                        bed.ownedBy = _villager.Fullname;
                    
                    _needs.Trigger(NeedsEvent.SLEPT_IN_BED);
                }
            
                _movement.CancelCurrentPath();
                _movement.MoveTo(_target);
                _started = true;
            }

            if(_movement.FailedToFollowPath)
                return false;

            if(_movement.ReachedEndOfPath)
            {
                _thing.transform.position = _target;
                _thing.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            
            return true;
        }

        public override void Reset()
        {
            _thing.transform.rotation = Quaternion.Euler(0, 0, 0);
            _started = false;
        }

        public override string ToString()
        {
            return "Sleeping";
        }
    }

}

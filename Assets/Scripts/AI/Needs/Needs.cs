using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Village.AI
{

    public enum NeedsEvent
    {
        SLEPT_OUTSIDE,
        SLEPT_IN_BED
    }

    public struct EventRecord
    {
        public NeedsEvent Event;
        public float WorldTime;
    }

    public class Needs
    {
        public float Warmth { get { return _warmth; } }
        private float _thirst;
        private float _hunger;
        private float _warmth;
        private float _rest;
        private Game _game;
        private List<EventRecord> _events;

        public Needs(Game game)
        {
            _game = game;
            _events = new List<EventRecord>();
        }

        public void Trigger(NeedsEvent needEvent)
        {
            switch (needEvent)
            {
                case NeedsEvent.SLEPT_OUTSIDE:
                    _warmth -= 0.5f;
                    break;
                case NeedsEvent.SLEPT_IN_BED:
                    _warmth = 0f;
                    break;
            }

            _events.Add(new EventRecord { Event = needEvent, WorldTime = _game.WorldTime.TimeSinceStart });
        }

        public bool IsCold()
        {
            return _warmth < 0f;
        }

        public bool IsDead()
        {
            return _warmth <= -3f;
        }

        public string GetReasonsForDeath()
        {
            var reasons = "";

            var numSleptOutside = _events.Count(e => e.Event == NeedsEvent.SLEPT_OUTSIDE);
            if (numSleptOutside > 0)
                reasons += $"<color=red>Spent {numSleptOutside} night{(numSleptOutside > 1 ? "s" : "")} outside.</color>\n";

            if (_warmth <= -3f)
                reasons += $"<color=red>Suffering from the cold.</color>\n";

            return reasons;
        }
    }

}
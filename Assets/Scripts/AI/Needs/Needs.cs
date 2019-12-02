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

    public class Needs : MonoBehaviour
    {
        public float Thirst { get; private set; }
        public float Hunger { get; private set; }
        public float Warmth { get; private set; }
        public float Rest { get; private set; }

        private Game _game;
        private List<EventRecord> _events;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _events = new List<EventRecord>();
        }

        public void Trigger(NeedsEvent needEvent)
        {
            switch (needEvent)
            {
                case NeedsEvent.SLEPT_OUTSIDE:
                    Warmth -= 0.5f;
                    break;
                case NeedsEvent.SLEPT_IN_BED:
                    Warmth = 0f;
                    break;
            }

            _events.Add(new EventRecord { Event = needEvent, WorldTime = _game.WorldTime.TimeSinceStart });
        }

        public bool IsCold()
        {
            return Warmth < 0f;
        }

        public void SetRest(float rest)
        {
            Rest = rest;
        }

        public void SetThirst(float thirst)
        {
            Thirst = thirst;
        }

        public void SetHunger(float hunger)
        {
            Hunger = hunger;
        }

        public bool IsDead()
        {
            return Warmth <= -3f;
        }

        public string GetReasonsForDeath()
        {
            var reasons = "";

            var numSleptOutside = _events.Count(e => e.Event == NeedsEvent.SLEPT_OUTSIDE);
            if (numSleptOutside > 0)
                reasons += $"<color=red>Spent {numSleptOutside} night{(numSleptOutside > 1 ? "s" : "")} outside.</color>\n";

            if (Warmth <= -3f)
                reasons += $"<color=red>Suffering from the cold.</color>\n";

            return reasons;
        }

        void OnDrawGizmos()
        {
            #if UNITY_EDITOR

            var label = "";

            if (IsDead())
            {
                label += $"DEAD\n";
                label += GetReasonsForDeath();
            }

            label += string.Format("hunger: {0}\n", Hunger);
            label += string.Format("thirst: {0}\n", Thirst);
            label += string.Format("warmth: {0}\n", Warmth);
            label += string.Format("rest: {0}\n", Rest);

            var style = new GUIStyle();
            style.fontSize = 10;
            style.normal.textColor = Color.white;

            // current actions
            var position = transform.position + Vector3.up;
            UnityEditor.Handles.Label(position, label, style);

            #endif
        }
    }

}
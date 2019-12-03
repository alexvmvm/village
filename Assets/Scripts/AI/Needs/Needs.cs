using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Village.AI
{
    public enum Need
    {
        WARMTH,
        THIRST,
        HUNGER,
        REST
    }

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

    // last time shown
    // duration to show
    // last need

    public class Needs : MonoBehaviour
    {
        // public float Thirst { get; private set; }
        // public float Hunger { get; private set; }
        // public float Warmth { get; private set; }
        // public float Rest { get; private set; }
        
        private Dictionary<Need, float> _needs;
        private Game _game;
        private List<EventRecord> _events;
        private SpriteRenderer _spriteRenderer;

        void Awake()
        {
            _game = FindObjectOfType<Game>();
            _events = new List<EventRecord>();
            
            // need dictionary
            _needs = new Dictionary<Need, float>();
            foreach(Need need in Enum.GetValues(typeof(Need)))
                _needs[need] = 0f;

            if(_spriteRenderer == null)
            {
                var obj = new GameObject();
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.up;
                _spriteRenderer = obj.AddComponent<SpriteRenderer>();
            }
        }

        public void Trigger(NeedsEvent needEvent)
        {
            switch (needEvent)
            {
                case NeedsEvent.SLEPT_OUTSIDE:
                    IncrementNeed(Need.WARMTH, -0.5f);
                    break;
                case NeedsEvent.SLEPT_IN_BED:
                    SetNeed(Need.WARMTH, 0f);
                    break;
            }

            _events.Add(new EventRecord { Event = needEvent, WorldTime = _game.WorldTime.TimeSinceStart });
        }

        public bool IsNeedCritical(Need need)
        {
            return _needs[need] < 0f;
        }

        public void SetNeed(Need need, float value)
        {
            _needs[need] = value;
        }

        public void IncrementNeed(Need need, float increment)
        {
            _needs[need] += increment;
        }

        public float GetNeedValue(Need need)
        {
            return _needs[need];
        }

        public bool IsDead()
        {
            return GetNeedValue(Need.WARMTH) <= -3f;
        }

        public string GetReasonsForDeath()
        {
            var reasons = "";

            var numSleptOutside = _events.Count(e => e.Event == NeedsEvent.SLEPT_OUTSIDE);
            if (numSleptOutside > 0)
                reasons += $"<color=red>Spent {numSleptOutside} night{(numSleptOutside > 1 ? "s" : "")} outside.</color>\n";

            if (GetNeedValue(Need.WARMTH) <= -3f)
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

            label += string.Format("hunger: {0}\n", GetNeedValue(Need.HUNGER));
            label += string.Format("thirst: {0}\n", GetNeedValue(Need.THIRST));
            label += string.Format("warmth: {0}\n", GetNeedValue(Need.WARMTH));
            label += string.Format("rest: {0}\n", GetNeedValue(Need.REST));

            var style = new GUIStyle();
            style.fontSize = 10;
            style.normal.textColor = Color.white;

            // current actions
            var position = transform.position + Vector3.down + Vector3.right;
            UnityEditor.Handles.Label(position, label, style);

            #endif
        }
    }

}
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

    public class Needs : MonoBehaviour
    {        
        private Dictionary<Need, float> _needs;
        private Game _game;
        private List<EventRecord> _events;
        private SpriteRenderer _spriteRenderer;
        private GameObject _obj;
        private static WaitForSeconds _showIconWait;
        private static WaitForSeconds _pauseBetweenIconWait;

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
                _obj = Instantiate(Assets.GetPrefab("Need"));
                _obj.transform.SetParent(transform);
                _obj.transform.localPosition = Vector3.up * 2;
                _spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                _obj.SetActive(false);
            }

            if(_showIconWait == null)
                _showIconWait = new WaitForSeconds(2f);

            if(_pauseBetweenIconWait == null)
                _pauseBetweenIconWait = new WaitForSeconds(5f);
        }

        void Start()
        {
            SetNeed(Need.HUNGER, -1f);
            SetNeed(Need.THIRST, -1f);

            StartCoroutine(UpdateNotification());
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

        public static Sprite GetNeedSprite(Need need)
        {
            switch(need)
            {
                case Need.HUNGER:
                    return Assets.GetSprite("colored_transparent_942");
                case Need.THIRST:
                    return Assets.GetSprite("colored_transparent_589");
                case Need.REST:
                    return Assets.GetSprite("colored_transparent_260");
                case Need.WARMTH:
                    return Assets.GetSprite("colored_transparent_333");
                default:
                    return Assets.GetSprite("");
            }
        }

        public float GetNeedValue(Need need)
        {
            return _needs[need];
        }

        public bool IsDead()
        {
            return GetNeedValue(Need.WARMTH) <= -1.5f;
        }

        IEnumerator UpdateNotification()
        {
            while(true)
            {
                if(IsDead())
                {
                    yield return _pauseBetweenIconWait;
                }
                else
                {
                    foreach(Need need in Enum.GetValues(typeof(Need)))
                    {
                        if(IsNeedCritical(need))
                        {
                            _spriteRenderer.sprite = GetNeedSprite(need);
                            _obj.SetActive(true);
                            yield return _showIconWait;
                            _spriteRenderer.sprite = null;
                            _obj.SetActive(false);
                            yield return _pauseBetweenIconWait;
                            
                        }    
                    }
                }

                yield return null;
            }
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
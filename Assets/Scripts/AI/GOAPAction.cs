using System.Collections;
using System.Collections.Generic;
using System;

namespace Village.AI
{
    [Serializable]
    public struct ActionState
    {
        public string action;
        public bool state;
    }

    public abstract class GOAPAction
    {
        public Dictionary<string, object> Preconditions;
        public Dictionary<string, object> Effects;

        public float Cost = 1f;

        protected Game _game;
        protected Agent _agent;

        public GOAPAction(Agent agent, Game game)
        {
            _game = game;
            _agent = agent;

            Preconditions = new Dictionary<string, object>();
            Effects = new Dictionary<string, object>();
        }

        abstract public bool Perform();
        abstract public bool IsDone();
        abstract public bool IsPossibleToPerform();

        abstract public void Reset();

        public override string ToString()
        {
            return this.GetType().ToString();
        }

        public class Effect
        {
            public const string IS_WORKING = "IS_WORKING";
            public const string IS_RESTED = "IS_RESTED";
            public const string IS_THIRSTY = "IS_THIRSTY";
            public const string IS_HUNGRY = "IS_HUNGRY";
            public const string IS_WARM = "IS_WARM";
            public const string IS_HOLDING_THING = "IS_HOLDING_THING";
            public const string HAS_THING = "HAS_THING";            
            public const string HAS_EDIBLE_THING = "HAS_EDIBLE_THING";
            public const string HAS_THING_FOR_STORAGE = "HAS_THING_FOR_STORAGE";
        }
    }
}


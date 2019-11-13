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
    }
}


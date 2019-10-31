using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{

    public class RequestResidence : GOAPAction
    {
        private bool _isDone;
        private Thing _thing;

        public RequestResidence(Game game, Thing thing) : base(game)
        {
            _thing = thing;
        }

        public override bool IsDone()
        {
            return _isDone;
        }

        public override bool IsPossibleToPerform()
        {
            return !_isDone;
        }

        public override bool Perform()
        {
            _isDone = true;
            return true;
        }

        public override void Reset()
        {

        }
    }

}

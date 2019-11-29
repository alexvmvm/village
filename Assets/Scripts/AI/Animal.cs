using System.Collections.Generic;
using Village.Things;

namespace Village.AI
{

    public class Animal : GOAPAgent
    {
        private Movement _movement;
        private Thing _thing;
        private Game _game;

        public override void Awake()
        {
            base.Awake();

            _game = FindObjectOfType<Game>();

            _thing = GetComponent<Thing>();
            _movement = _thing.gameObject.AddComponent<Movement>();

            AddAction(new Idle(this, _game, _movement));
        }

        public override void UpdateState(Dictionary<string, object> state)
        {
            
        }

        // public override void PauseAgent()
        // {
        //     base.PauseAgent();

        //     _movement.CancelCurrentPath();
        //     _movement.SetStopped(true);
        // }

        // public override void UnPauseAgent()
        // {
        //     base.UnPauseAgent();

        //     _movement.SetStopped(false);
        // }

        // public override void ActionCompleted(GOAPAction action)
        // {

        // }

        // public override void GetGoalState(Dictionary<string, object> goal)
        // {
        //     goal[GOAPAction.Effect.IS_WORKING] = true;
        // }

        // public override void GetWorldState(Dictionary<string, object> world)
        // {
        //     world[GOAPAction.Effect.IS_WORKING]  = false;
        // }

        // public override void Update()
        // {
        //     base.Update();

        //     //SetLabel($"{_thing.Config.TypeOfThing.ToString()}\n{(CurentAction == null ? "" : CurentAction.ToString())}");
        // }
    }

}

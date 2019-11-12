using SwordGC.AI.Goap;

namespace Village.AI.V2
{
    public class Director : GoapAgent
    {
        private Game _game;

        public override void Awake()
        {
            base.Awake();

            _game = FindObjectOfType<Game>();

            goals.Add(GoapGoal.Goals.SPAWN_VILLAGERS, new SpawnVillagers());

            possibleActions.Add(new SpawnVillager(this, _game));
        }
    }
}


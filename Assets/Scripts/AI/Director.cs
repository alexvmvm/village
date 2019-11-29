using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Village.AI
{

    public class Director : GOAPAgent
    {
        private Game _game;


        public override void Awake()
        {
            base.Awake();

            _game = FindObjectOfType<Game>();

            AddAction(new SpawnVillager(this, _game));
        }

        public override void UpdateState(Dictionary<string, object> state)
        {
            
        }

        
        public override void ActionCompleted(GOAPAction action)
        {

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Village.AI;

namespace Village.UI.VillagerPanels
{
    public class VillagerVerticalList : MonoBehaviour
    {
        public ObjectPooler ButtonPooler;     
        public VillagerActionPanel VillagerActionPanel;
        private Game _game;   
        
        void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        void Start()
        {
            StartCoroutine(UpdateList());
        }

        IEnumerator UpdateList()
        {
            while(true)
            {
                ButtonPooler.DeactivateAll();

                foreach(var villager in _game.QueryThings()
                    .Where(t => t.Config.TypeOfThing == TypeOfThing.Villager)
                    .Select(t => t.GetComponent<Villager>())
                    .Where(v => !v.Needs.IsDead()))
                    {
                        var obj = ButtonPooler.GetPooledObject();
                        var btn = obj.GetComponent<VillagerButton>();
                        btn.Text.text = villager.Fullname;      

                        btn.Button.onClick.RemoveAllListeners();
                        btn.Button.onClick.AddListener(() => {
                            VillagerActionPanel.SetupVillager(villager);
                        });
                        
                        obj.SetActive(true);
                    }

                yield return new WaitForSeconds(1f);
            }
        }
    }
}


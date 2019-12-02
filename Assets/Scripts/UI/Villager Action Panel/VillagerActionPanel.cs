using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village.AI;

namespace Village.UI.VillagerPanels
{
    public class VillagerActionPanel : MonoBehaviour
    {
        public Image Image;
        public Text Name;

        public void SetupVillager(Villager villager)
        {
            Name.text = villager.Fullname;

            gameObject.SetActive(true);
        }
    }
}


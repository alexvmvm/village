using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum VillagerEvent
{
    VillagerArrived,
    VillagerLeft
}

public class VillageManager : MonoBehaviour
{
    public VillagerArrivedPanel VillagerArrivedPanel;

    private Game _game;

    void Awake()
    {
        _game = FindObjectOfType<Game>();
    }

    public IEnumerable<string> GetDistinctSurnames()
    {
        return _game.Things
            .Where(t => t.type == TypeOfThing.Villager && t.agent != null)
            .Select(t => (t.agent as Villager))
            .Select(v => v.Lastname)
            .Distinct();
    }

    public void TriggerEvent(VillagerEvent villagerEvent, Villager villager)
    {
        switch(villagerEvent)
        {
            case VillagerEvent.VillagerArrived:
                VillagerArrivedPanel.ShowMessage(string.Format("{0} has arrived in the village. Find them a place to live.", villager.Fullname));
            break;
            case VillagerEvent.VillagerLeft:
                VillagerArrivedPanel.ShowMessage(string.Format("{0} has decided to find a home elsewhere and is leaving.", villager.Fullname));
            break;
        }
    }
}

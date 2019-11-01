using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Village.Things;
using Village.AI;
using Village;

public enum VillagerEvent
{
    VillagerArrived,
    VillagerLeft,
    VillagerFirstNightAtHome
}

public class VillageManager : MonoBehaviour
{
    public VillagerArrivedPanel VillagerArrivedPanel;
    public ObjectPooler FamilyPanels;
    private Game _game;

    void Awake()
    {
        _game = FindObjectOfType<Session>().Game;
    }

    void OnEnable()
    {
        EventManager.StartListening(Constants.VILLAGER_ARRIVED, UpdateFamilyPanels);
        EventManager.StartListening(Constants.VILLAGER_LEFT, UpdateFamilyPanels);
    }

    public IEnumerable<IGrouping<string, Thing>> GetFamiliesByLastname()
    {
        return _game.QueryThings()
            .Where(t => t.type == TypeOfThing.Villager && t.HasTrait<Villager>())
            .GroupBy(t => t.GetTrait<Villager>().Lastname);
    }

    void UpdateFamilyPanels()
    {
        FamilyPanels.DeactivateAll();
        foreach(var group in GetFamiliesByLastname())
        {
            var obj = FamilyPanels.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = group.Key;

            // var familyPanel = obj.GetComponent<FamilyPanel>();
            // familyPanel.FamilyName.text = group.Key;

            // familyPanel.PeoplePooler.DeactivateAll();
            // foreach(var thing in group)
            // {
            //     var personObj = familyPanel.PeoplePooler.GetPooledObject();
            //     personObj.GetComponentInChildren<Text>().text = (thing.agent as Villager).Fullname;
            //     personObj.SetActive(true);
            // }

            obj.SetActive(true);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(FamilyPanels.transform as RectTransform);
    }

    public IEnumerable<string> GetDistinctSurnames()
    {
        return _game.QueryThings()
            .Where(t => t.type == TypeOfThing.Villager)
            .Select(t => t.GetTrait<Villager>())
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
            case VillagerEvent.VillagerFirstNightAtHome:
                VillagerArrivedPanel.ShowMessage(string.Format("{0} spent their first night at home. Looks like they're staying.", villager.Fullname));
            break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VillageManager : MonoBehaviour
{
    public VillagerArrivedPanel VillagerArrivedPanel;

    private Game _game;

    void Awake()
    {
        _game = FindObjectOfType<Game>();
    }

    void Start()
    {
        EventManager.StartListening(Constants.VILLAGER_ARRIVED, VillagerArrived);
    }

    void VillagerArrived()
    {
        var thing = _game.Things
            .Where(t => t.type == TypeOfThing.Villager)
            .OrderByDescending(t => t.agent.Created)
            .ToArray()
            .FirstOrDefault();

        Debug.Log("Villager Arrived " + thing.name);
        VillagerArrivedPanel.Activate(thing.name);
    }
}

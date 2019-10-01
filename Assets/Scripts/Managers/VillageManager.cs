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

    public IEnumerable<string> GetDistinctSurnames()
    {
        return _game.Things
            .Where(t => t.type == TypeOfThing.Villager && t.agent != null)
            .Select(t => (t.agent as Villager))
            .Select(v => v.Lastname)
            .Distinct();
    }

    public void VillagerArrived(string name)
    {
        VillagerArrivedPanel.Activate(name);
    }
}

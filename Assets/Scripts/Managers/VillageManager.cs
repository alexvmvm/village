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

    public void VillagerArrived(string name)
    {
        VillagerArrivedPanel.Activate(name);
    }
}

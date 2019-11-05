using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village;

public class RegionManager 
{   
    private List<Region> _regions;

    public RegionManager(Game game)
    {
        _regions = new List<Region>();

        for(var x = 0; x < game.Size.x; x += 10)
        {
            for(var y = 0; y < game.Size.y; y += 10)
            {
                var min = new Vector2Int(x, y);
                var max = min + Vector2Int.one * 10;
                _regions.Add(new Region(game, min, max));
            }
        }
    }

    public void DrawGizmos()
    {
        foreach(var region in _regions)
        {
            region.DrawGizmos();
        }
    }

    public void Update()
    {
        foreach(var region in _regions)
        {
            region.Update();
        }
    }

}

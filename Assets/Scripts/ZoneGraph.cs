using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
using Village.Things;
using Village;

public class ZoneGraph 
{

    private Game _game;
    private Graph<RectInt> _graph;
    private Queue<Vector2Int> _queue;
    private HashSet<Vector2Int> _seen;
    private List<RectInt> _regions;
    private List<Vector2Int> _toCheck;
    private List<RectInt> _regionsToRemove;
    private List<RectInt> _newRegions;
    private Vector2Int[] _neighbours;

    public ZoneGraph(Game game)
    {
        _game = game;
        _game.OnThingAdded += ThingAdded;
        _game.OnThingRemoved += ThingRemoved;
    
        _regions = new List<RectInt>();
        _queue = new Queue<Vector2Int>();
        _seen = new HashSet<Vector2Int>();
        _toCheck = new List<Vector2Int>();
        _regionsToRemove = new List<RectInt>();
        _newRegions = new List<RectInt>();

        _graph = new Graph<RectInt>();

        _neighbours = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }

    public void Start()
    {
        CreateRegionsThatDoNotExist();
    }
    
    void ThingAdded(Thing thing)
    {
        if(!thing.fixedToGrid)
            return;

        if(!_toCheck.Contains(thing.position))  
            _toCheck.Add(thing.position);
    }

    void ThingRemoved(Thing thing)
    {
        if(!thing.fixedToGrid)
            return;

         if(!_toCheck.Contains(thing.position))  
            _toCheck.Add(thing.position);
    }
    
    public void UpdateRegions()
    {
        Profiler.BeginSample("ZoneGraph_UpdateRegions");

        _regionsToRemove.Clear();

        foreach(var position in _toCheck)
        {
            if(IsRegionAtPoint(position))
            {
                _regionsToRemove.Add(FindRegionAtPosition(position));
            }
        }
        
        if(_regionsToRemove.Count == 0)
        {
            Profiler.EndSample();
            return;
        }
          

        foreach(var region in _regionsToRemove)
        {
            _graph.Remove(region);
            _regions.Remove(region);
        }

        CreateRegionsThatDoNotExist();

        _toCheck.Clear();

        Profiler.EndSample();
    }

    public void CreateRegionsThatDoNotExist()
    {
        Profiler.BeginSample("ZoneGraph_CreateRegionsThatDoNotExist");

        _queue.Clear();
        _seen.Clear();
        _newRegions.Clear();


        foreach(var position in _game.QueryThings().Where(t => t.fixedToGrid && t.floor).Select(t => t.position))
        {
            _queue.Enqueue(position);
        }

        Profiler.BeginSample("ZoneGraph_CreateRegionsThatDoNotExist_CreateNewRects");

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current) || _regions.Any(r => r.Contains(current)))
                continue;

            _seen.Add(current);

            var rect = FindRect(current);

            // var rect = new RectInt(current.x, current.y, 1, 1);

            // while(IsSuitableForZonePlacement(rect.max + Vector2Int.right))
            //     rect.max += Vector2Int.right;

            // while(IsSuitableForZonePlacement(rect.min + Vector2Int.left))
            //     rect.min += Vector2Int.left;

            // while(IsClearHorizontally(rect.min.x, rect.max.x, rect.max.y + 1))
            //     rect.max += Vector2Int.up;

            // while(IsClearHorizontally(rect.min.x, rect.max.x, rect.min.y - 1))
            //     rect.min += Vector2Int.up;
            
                    
            _regions.Add(rect);
            _newRegions.Add(rect);
        }

        Profiler.EndSample();

        Profiler.BeginSample("ZoneGraph_CreateRegionsThatDoNotExist_GraphUpdate");

        // remove regions from graph
        foreach(var region in _regionsToRemove)
        {
            _graph.Remove(region);
        }

        foreach(var region in _newRegions)
        { 
            _graph.AddNode(region);
        }

        foreach(var region in _newRegions)
        {
            foreach(var n in _regions.Where(r => region.AdjacentTo(r)))
            {
                _graph.AddUndirectedEdge(_graph.GetNodeByValue(region), _graph.GetNodeByValue(n), 0);
            }
        }

        Profiler.EndSample();


        Profiler.EndSample();
    }

    RectInt FindRect(Vector2Int position)
    {        
        Profiler.BeginSample("FindRect_MinMaxLeftRight");

        var max = FindPosition(position, Vector2Int.right);
        var min = FindPosition(position, Vector2Int.left);

        Profiler.EndSample();

        Profiler.BeginSample("FindRect_MinMaxUpDown");

        while(IsClearBelow(min, max))
        {
            min.y -= 1;
        }

        while(IsClearAbove(min, max))
        {
            max.y += 1;
        }

        Profiler.EndSample();

        var width = max.x - min.x + 1;
        var height = max.y - min.y + 1;

        return new RectInt(min.x, min.y, width, height);
    }

    Vector2Int FindPosition(Vector2Int start, Vector2Int direction)
    {
        while(true)
        {
            if(!IsSuitableForZonePlacement(start + direction))
                return start;
            start += direction;
        }
    }

    bool IsClearBelow(Vector2Int min, Vector2Int max)
    {
        for(var x = min.x; x <= max.x; x++)
        {
            if(!IsSuitableForZonePlacement(new Vector2Int(x, min.y - 1)))
                return false;
        }

        return true;
    }

    bool IsClearAbove(Vector2Int min, Vector2Int max)
    {
        for(var x = min.x; x <= max.x; x++)
        {
            if(!IsSuitableForZonePlacement(new Vector2Int(x, max.y + 1)))
                return false;
        }

        return true;
    }

    bool IsClearHorizontally(int xMin, int xMax, int y)
    {
        for(var x = xMin; x <= xMax; x++)
        {
            if(!IsSuitableForZonePlacement(new Vector2Int(x, y)))
                return false;
        }

        return true;
    }

    bool IsSuitableForZonePlacement(Vector2Int position)
    {
        if(_regions.Any(r => r.Contains(position)))
            return false;

        var thing = _game.GetThingOnGrid(position);

        if(thing == null)
            return false;

        return thing.fixedToGrid && thing.floor;
    }

    bool IsRegionAtPoint(Vector2Int position)
    {
        return _regions.Any(r => r.Contains(position) || r.AdjacentTo(position));
    }

    RectInt FindRegionAtPosition(Vector2Int position)
    {
        return _regions.First(r => r.Contains(position) || r.AdjacentTo(position));
    }

    // public bool IsPathPossible(Vector3 start, Vector3 end)
    // {
    //     Profiler.BeginSample("ZoneGraph_IsPathPossible");

    //     var startVec2Int = start.ToVector2IntFloor();
    //     var endVec2Int = end.ToVector2IntFloor();

    //     if(!IsRegionAtPoint(startVec2Int) || !(IsRegionAtPoint(endVec2Int)))
    //     {
    //         Profiler.EndSample();
    //         return false;
    //     }

    //     var a = FindRegionAtPosition(startVec2Int);
    //     var b = FindRegionAtPosition(endVec2Int);

    //     if(a.Equals(b))
    //     {
    //         Profiler.EndSample();
    //         return true;
    //     }

    //     Profiler.EndSample();

    //     return _graph.IsPathBetweenNodes(_graph.GetNodeByValue(a), _graph.GetNodeByValue(b));
    // }

    public void Update()
    {
        if(_toCheck.Count > 0)
        {
            UpdateRegions();
        }
    }

    public void DrawGizmos()
    {
        var offset = Vector3.one * 0.5f;
        foreach(var region in _regions)
        {
            // Gizmos.color = Color.blue;
            // Gizmos.DrawCube(region.center.ToVector3() - offset, region.size.ToVector3());
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(region.center.ToVector3() - offset, region.size.ToVector3());

            Gizmos.DrawWireSphere(region.center.ToVector3() - offset, 0.5f);
            Gizmos.DrawLine(region.center.ToVector3() - offset, region.max.ToVector3() - offset);
            Gizmos.DrawLine(region.center.ToVector3() - offset, region.min.ToVector3() - offset);
// #if UNITY_EDITOR
        

//         var style = new GUIStyle();
//         style.fontSize = 10;
//         style.normal.textColor = Color.white;

//         // current actions
//         UnityEditor.Handles.Label(region.center, $"neighbours: {_graph.GetNodeByValue(region).Neighbors.Count()}", style);
// #endif  
        
        }

        Gizmos.color = Color.white;
        foreach(var region in _regions)
        {
            var node = _graph.GetNodeByValue(region);
            if(node == null)
                continue;
            foreach(var neighbour in node.Neighbors)
            {
                Gizmos.DrawLine(region.center, neighbour.Value.center);
            }
        }

    }
}

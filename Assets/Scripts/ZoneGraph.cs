using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneGraph 
{

    private Game _game;
    private bool _dirty;
    private Graph<RectInt> _graph;
    private Queue<Vector2Int> _queue;
    private HashSet<Vector2Int> _seen;
    private List<RectInt> _regions;
    private List<Vector2Int> _toCheck;

    public ZoneGraph(Game game)
    {
        _game = game;
        _game.OnThingAdded += ThingAdded;
        _game.OnThingRemoved += ThingRemoved;
    
        _regions = new List<RectInt>();
        _queue = new Queue<Vector2Int>();
        _seen = new HashSet<Vector2Int>();
        _toCheck = new List<Vector2Int>();
        _graph = new Graph<RectInt>();
    }
    
    void ThingAdded(Thing thing)
    {
        if(!thing.fixedToGrid)
            return;

        if(!_toCheck.Contains(thing.gridPosition))  
            _toCheck.Add(thing.gridPosition);
    }

    void ThingRemoved(Thing thing)
    {
        if(!thing.fixedToGrid)
            return;

         if(!_toCheck.Contains(thing.gridPosition))  
            _toCheck.Add(thing.gridPosition);
    }
    
    public void UpdateRegions()
    {
        var overlappingRegions = _regions.Where(r => _toCheck.Any(p => r.Contains(p))).ToArray();

        foreach(var region in overlappingRegions)
        {
            _graph.Remove(region);
            _regions.Remove(region);
        }

        Init();

        UpdateGraph();

        _toCheck.Clear();
    }

    public void Init()
    {
        _regions.Clear();
        _queue.Clear();
        _seen.Clear();

        foreach(var position in _game.Things.Where(t => t.fixedToGrid && t.floor).Select(t => t.gridPosition))
        {
            _queue.Enqueue(position);
        }

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current) || _regions.Any(r => r.Contains(current)))
                continue;

            _seen.Add(current);

            var rect = FindRect(current);

            _regions.Add(rect);
        }

        UpdateGraph();
    }

    void UpdateGraph()
    {        
        _graph.Clear();

        foreach(var region in _regions)
        { 
            if(!_graph.Contains(region))
                _graph.AddNode(region);

            foreach(var n in _regions.Where(r => region.AdjacentTo(r)))
            {
                if(!_graph.Contains(n))
                    _graph.AddNode(n);

                _graph.AddUndirectedEdge(_graph.GetNodeByValue(region), _graph.GetNodeByValue(n), 0);
            }
        }
    }

    RectInt FindRect(Vector2Int position)
    {        
        var max = FindPosition(position, Vector2Int.right);
        var min = FindPosition(position, Vector2Int.left);
    
        while(IsClearBelow(min, max))
        {
            min.y -= 1;
        }

        while(IsClearAbove(min, max))
        {
            max.y += 1;
        }

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

    bool IsSuitableForZonePlacement(Vector2Int position)
    {
        return 
            !_regions.Any(r => r.Contains(position)) &&
            _game.GetThingOnGrid(position) != null && 
            _game.GetThingOnGrid(position).fixedToGrid &&
            _game.GetThingOnGrid(position).floor;
    }

    bool RegionExistsAtPoint(Vector3 position)
    {
        return _regions.Any(r => r.Contains(position.ToVector2IntFloor()));
    }

    public bool IsPathPossible(Vector3 start, Vector3 end)
    {
        if(!RegionExistsAtPoint(start) || !RegionExistsAtPoint(end))
            return false;

        var a = _regions.Where(r => r.Contains(start.ToVector2IntFloor())).First();
        var b = _regions.Where(r => r.Contains(end.ToVector2IntFloor())).First();

        return _graph.ShortestPathToVertex(a, b, (rect) => false) != null;
    }

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
            foreach(var neighbour in node.Neighbors)
            {
                Gizmos.DrawLine(region.center, neighbour.Value.center);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Village;
using Village.Things;

public class RegionManager 
{   
    private Dictionary<Vector2Int, Region> _regions;
    private int _regionSize = 10;
    private Game _game;
    private Queue<Region> _update;
    private Queue<Region> _connections;
    private Graph<SubRegion> _graph;

    private Dictionary<Edge, SubRegion> _top;
    private Dictionary<Edge, SubRegion> _bottom;
    private Dictionary<Edge, SubRegion> _left;
    private Dictionary<Edge, SubRegion> _right;

    private static Vector2Int[] _neighbours = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
    
    public RegionManager(Game game)
    {
        _regions = new Dictionary<Vector2Int, Region>();

        _update = new Queue<Region>();
        _graph = new Graph<SubRegion>();
        _connections = new Queue<Region>();

        _top = new Dictionary<Edge, SubRegion>();
        _bottom = new Dictionary<Edge, SubRegion>();
        _left = new Dictionary<Edge, SubRegion>();
        _right = new Dictionary<Edge, SubRegion>();

        _game = game;
        _game.OnThingAdded += OnThingAdded;
        _game.OnThingRemoved += OnThingRemoved;

        for(var x = 0; x < game.Size.x; x += _regionSize)
        {
            for(var y = 0; y < game.Size.y; y += _regionSize)
            {
                var min = new Vector2Int(x, y);
                var max = min + Vector2Int.one * _regionSize;
                var region = new Region(game, min, max);
                var regionPosition = ToRegionPosition(min);
                _regions.Add(regionPosition, region);
            }
        }
    }

    /*
        Region Helper Functions
    */
    Vector2Int ToRegionPosition(Vector2Int position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / _regionSize), 
            Mathf.FloorToInt(position.y / _regionSize));
    }

    Region GetRegionAtPosition(Vector2Int position)
    {
        return _regions.ContainsKey(position) ? _regions[position] : null;
    }

    bool RegionExists(Vector2Int position)
    {
        return _regions.ContainsKey(position);
    }

    /*
        Querying
    */

    public SubRegion GetSubRegionAtPosition(Vector2Int position)
    {
        var region = GetRegionAtPosition(ToRegionPosition(position));
        var subRegion = GetSubRegionAtPosition(position);
        if(subRegion != null)
            return subRegion;

        foreach(var n in _neighbours)
        {
            var otherSubRegion = region.GetSubRegionAtPosition(position + n);
            if(otherSubRegion != null)
                return otherSubRegion;
        }

        foreach(var n in _neighbours)
        {
            var otherRegion = GetRegionAtPosition(position + n);
            if(otherRegion != null)
            {
                var otherSubRegion = otherRegion.GetSubRegionAtPosition(position + n);
                if(otherSubRegion != null)
                    return otherSubRegion;
            }
        }

        return null;
    }
    
    public bool IsPathPossbile(Vector2Int start, Vector2Int end)
    {
        var startRegion = GetRegionAtPosition(ToRegionPosition(start));
        var endRegion = GetRegionAtPosition(ToRegionPosition(end));

        if(startRegion == null || endRegion == null)
            return false;

        var startSubRegion = startRegion.GetSubRegionAtPosition(start);
        var endSubRegion = endRegion.GetSubRegionAtPosition(end);
        
        if(startSubRegion == null || endSubRegion == null)
            return false;

        return _graph.IsPathBetweenNodes(startSubRegion, endSubRegion);
    }

    public Thing IsPathPossbileToThing(Vector2Int start, TypeOfThing type, Func<Thing, bool> filter)
    {
        var startRegion = GetRegionAtPosition(ToRegionPosition(start));

        if(startRegion == null)
            return null;

        var startSubRegion = startRegion.GetSubRegionAtPosition(start);
       
        if(startSubRegion == null)
            return null;

        var subRegion = _graph.IsPathToNodes(startSubRegion, (region) => region.HasTypeOfThing(type) && region.GetThings(type).Any(filter));

        if(subRegion != null)
        {
            return subRegion.GetThings(type).FirstOrDefault(filter);
        }

        return null;
    }
    

    /*
        Game Events
    */
    void OnThingAdded(Thing thing)
    {
        FindRegionsToUpdate(ToRegionPosition(thing.Position));
    }

    void OnThingRemoved(Thing thing)
    {
        FindRegionsToUpdate(ToRegionPosition(thing.Position));
    }

    void FindRegionsToUpdate(Vector2Int regionPosition)
    {   
        if(RegionExists(regionPosition))
        {
            var current = GetRegionAtPosition(regionPosition);
            _update.Enqueue(current);

            foreach(var neighbour in _neighbours)
            {
                var position = regionPosition + neighbour;
                if(RegionExists(position))
                    _update.Enqueue(GetRegionAtPosition(position));
            }
        }
    }

    public void DrawGizmos()
    {
        foreach(var region in _regions.Values)
        {
            region.DrawGizmos();
        }
        
        Gizmos.color = Color.white;
        foreach(GraphNode<SubRegion> node in _graph.Nodes)
        {
            foreach(var n in node.Neighbors)
            {
                Gizmos.DrawLine(node.Value.Min.ToVector3(), n.Value.Min.ToVector3());
            }
        }
    }

    void RemoveEdgesForSubRegion(SubRegion subRegion)
    {
        foreach(var topEdge in subRegion.Top)
            _top.Remove(topEdge);
        foreach(var bottomEdge in subRegion.Bottom)
            _bottom.Remove(bottomEdge);
        foreach(var leftEdge in subRegion.Left)
            _left.Remove(leftEdge);
        foreach(var rightEdge in subRegion.Right)
            _right.Remove(rightEdge);
    }

    void AddEdgesForSubRegion(SubRegion subRegion)
    {
        foreach(var topEdge in subRegion.Top)
            _top.Add(topEdge, subRegion);
        foreach(var bottomEdge in subRegion.Bottom)
            _bottom.Add(bottomEdge, subRegion);
        foreach(var leftEdge in subRegion.Left)
            _left.Add(leftEdge, subRegion);
        foreach(var rightEdge in subRegion.Right)
            _right.Add(rightEdge, subRegion);
    }

    public void Update()
    {
        if(_update.Count == 0)
            return;


        while(_update.Count > 0)
        {
            var current = _update.Dequeue();

            // remove subregions from graph
            foreach(var subRegion in current.SubRegions)
            {
                subRegion.RemoveListeners();
                _graph.Remove(subRegion);
                RemoveEdgesForSubRegion(subRegion);
            }
                
            current.Refresh();
            
            // add subregions to graph
            foreach(var subRegion in current.SubRegions)
            {
                subRegion.AddListeners();
                _graph.AddNode(subRegion);
                AddEdgesForSubRegion(subRegion);
            }
                
            
            if(!_connections.Contains(current))
                _connections.Enqueue(current);
        }

        while(_connections.Count > 0)
        {
            var current = _connections.Dequeue();

            foreach(var subRegion in current.SubRegions)
            {
                var node = _graph.GetNodeByValue(subRegion);
                
                foreach(var topEdge in subRegion.Top)
                {
                    if(_bottom.ContainsKey(topEdge))
                        _graph.AddUndirectedEdge(node, _graph.GetNodeByValue(_bottom[topEdge]), 0);
                }

                foreach(var bottomEdge in subRegion.Bottom)
                {
                    if(_top.ContainsKey(bottomEdge))
                        _graph.AddUndirectedEdge(node, _graph.GetNodeByValue(_top[bottomEdge]), 0);
                }

                foreach(var leftEdge in subRegion.Left)
                {
                    if(_right.ContainsKey(leftEdge))
                        _graph.AddUndirectedEdge(node, _graph.GetNodeByValue(_right[leftEdge]), 0);
                }

                foreach(var rightEdge in subRegion.Right)
                {
                    if(_left.ContainsKey(rightEdge))
                        _graph.AddUndirectedEdge(node, _graph.GetNodeByValue(_left[rightEdge]), 0);
                }
            }
        }
    }

}

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

        // seach nearby if current position is 
        // not a sub region.
        if(startSubRegion == null)
        {
            foreach(var n in _neighbours)
            {
                startSubRegion = startRegion.GetSubRegionAtPosition(start + n);
                if(startSubRegion != null)
                    break;
            }
        }

        var endSubRegion = endRegion.GetSubRegionAtPosition(end);
        
        if(startSubRegion == null || endSubRegion == null)
            return false;

        if(startSubRegion.Equals(endSubRegion))
            return true;

        return _graph.IsPathBetweenNodes(_graph.GetNodeByValue(startSubRegion), _graph.GetNodeByValue(endSubRegion));
    }

    /*
        Game Events
    */
    void OnThingAdded(Thing thing)
    {
        FindRegionsToUpdate(ToRegionPosition(thing.position));
    }

    void OnThingRemoved(Thing thing)
    {
        FindRegionsToUpdate(ToRegionPosition(thing.position));
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
                _graph.Remove(subRegion);

                foreach(var topEdge in subRegion.Top)
                    _top.Remove(topEdge);
                foreach(var bottomEdge in subRegion.Bottom)
                    _bottom.Remove(bottomEdge);
                foreach(var leftEdge in subRegion.Left)
                    _left.Remove(leftEdge);
                foreach(var rightEdge in subRegion.Right)
                    _right.Remove(rightEdge);
            }
                


            current.Refresh();
            
            // add subregions to graph
            foreach(var subRegion in current.SubRegions)
            {
                _graph.AddNode(subRegion);

                foreach(var topEdge in subRegion.Top)
                    _top.Add(topEdge, subRegion);
                foreach(var bottomEdge in subRegion.Bottom)
                    _bottom.Add(bottomEdge, subRegion);
                foreach(var leftEdge in subRegion.Left)
                    _left.Add(leftEdge, subRegion);
                foreach(var rightEdge in subRegion.Right)
                    _right.Add(rightEdge, subRegion);
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

            // var gridPosition = ToRegionPosition(current.Min);

            // var upPosition = gridPosition.Up();
            // var downPosition = gridPosition.Down();
            // var leftPosition = gridPosition.Left();
            // var rightPosition = gridPosition.Right();

            // var up = RegionExists(upPosition) ? GetRegionAtPosition(gridPosition.Up()) : null;
            // var down = RegionExists(downPosition) ? GetRegionAtPosition(gridPosition.Down()) : null;
            // var left = RegionExists(leftPosition) ? GetRegionAtPosition(gridPosition.Left()) : null;
            // var right = RegionExists(rightPosition) ? GetRegionAtPosition(gridPosition.Right()) : null;
            
            // foreach(var subRegion in current.SubRegions)
            // {
            //     // local
            //     {
            //         var connectedUp = current.SubRegions.FirstOrDefault(s => s.Down.Overlap(subRegion.Up));
            //         if(connectedUp != null)
            //             _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedUp), 0);
                    
            //         var connectedDown = current.SubRegions.FirstOrDefault(s => s.Up.Overlap(subRegion.Down));
            //         if(connectedDown != null)
            //             _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedDown), 0);
                    
            //         var connectedLeft = current.SubRegions.FirstOrDefault(s => s.Right.Overlap(subRegion.Left));
            //         if(connectedLeft != null)
            //             _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedLeft), 0);
                    
            //         var connectedRight = current.SubRegions.FirstOrDefault(s => s.Left.Overlap(subRegion.Right));
            //         if(connectedRight != null)
            //             _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedRight), 0);
            //     }
                

            //     // neighbours
            //     {
            //         if(up != null)
            //         {
            //             var connectedUp = up.SubRegions.FirstOrDefault(s => s.Down.Overlap(subRegion.Up));
            //             if(connectedUp != null)
            //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedUp), 0);
            //         }

            //         if(down != null)
            //         {
            //             var connectedDown = down.SubRegions.FirstOrDefault(s => s.Up.Overlap(subRegion.Down));
            //             if(connectedDown != null)
            //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedDown), 0);
            //         }
                    
            //         if(left != null)
            //         {
            //             var connectedLeft = left.SubRegions.FirstOrDefault(s => s.Right.Overlap(subRegion.Left));
            //             if(connectedLeft != null)
            //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedLeft), 0);
            //         }
                    
            //         if(right != null)
            //         {
            //             var connectedRight = right.SubRegions.FirstOrDefault(s => s.Left.Overlap(subRegion.Right));
            //             if(connectedRight != null)
            //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedRight), 0);
            //         }
            //     }
            // }

            
        }


    }

}

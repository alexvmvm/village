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
                _graph.Remove(subRegion);


            current.Refresh();
            
            // add subregions to graph
            foreach(var subRegion in current.SubRegions)
                _graph.AddNode(subRegion);
            
            if(!_connections.Contains(current))
                _connections.Enqueue(current);
        }

        while(_connections.Count > 0)
        {
            var current = _connections.Dequeue();
            var gridPosition = ToRegionPosition(current.Min);

            var upPosition = gridPosition.Up();
            var downPosition = gridPosition.Down();
            var leftPosition = gridPosition.Left();
            var rightPosition = gridPosition.Right();

            var up = RegionExists(upPosition) ? GetRegionAtPosition(gridPosition.Up()) : null;
            var down = RegionExists(downPosition) ? GetRegionAtPosition(gridPosition.Down()) : null;
            var left = RegionExists(leftPosition) ? GetRegionAtPosition(gridPosition.Left()) : null;
            var right = RegionExists(rightPosition) ? GetRegionAtPosition(gridPosition.Right()) : null;
            
            foreach(var subRegion in current.SubRegions)
            {
                // local
                {
                    var connectedUp = current.SubRegions.FirstOrDefault(s => s.Down.Overlap(subRegion.Up));
                    if(connectedUp != null)
                        _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedUp), 0);
                    
                    var connectedDown = current.SubRegions.FirstOrDefault(s => s.Up.Overlap(subRegion.Down));
                    if(connectedDown != null)
                        _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedDown), 0);
                    
                    var connectedLeft = current.SubRegions.FirstOrDefault(s => s.Right.Overlap(subRegion.Left));
                    if(connectedLeft != null)
                        _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedLeft), 0);
                    
                    var connectedRight = current.SubRegions.FirstOrDefault(s => s.Left.Overlap(subRegion.Right));
                    if(connectedRight != null)
                        _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedRight), 0);
                }
                

                // neighbours
                {
                    if(up != null)
                    {
                        var connectedUp = up.SubRegions.FirstOrDefault(s => s.Down.Overlap(subRegion.Up));
                        if(connectedUp != null)
                            _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedUp), 0);
                    }

                    if(down != null)
                    {
                        var connectedDown = down.SubRegions.FirstOrDefault(s => s.Up.Overlap(subRegion.Down));
                        if(connectedDown != null)
                            _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedDown), 0);
                    }
                    
                    if(left != null)
                    {
                        var connectedLeft = left.SubRegions.FirstOrDefault(s => s.Right.Overlap(subRegion.Left));
                        if(connectedLeft != null)
                            _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedLeft), 0);
                    }
                    
                    if(right != null)
                    {
                        var connectedRight = right.SubRegions.FirstOrDefault(s => s.Left.Overlap(subRegion.Right));
                        if(connectedRight != null)
                            _graph.AddUndirectedEdge(_graph.GetNodeByValue(subRegion), _graph.GetNodeByValue(connectedRight), 0);
                    }
                }
            }

            // 1. loop sub regions
            // 2. find connected sub regions

            // get all surrounding sub regions
            // foreach(var n in _neighbours)
            // {
            //     var neighbour = gridPosition + n;
            //     if(!RegionExists(neighbour))
            //         continue;
            //     var region = GetRegionAtPosition(neighbour);
                
            //     foreach(var subRegion in current.SubRegions)
            //     {
            //         var connected = new SubRegion[]
            //         {
            //             region.SubRegions.FirstOrDefault(r => r.Up.Overlap(subRegion.Down)),
            //             region.SubRegions.FirstOrDefault(r => r.Down.Overlap(subRegion.Up)),
            //             region.SubRegions.FirstOrDefault(r => r.Right.Overlap(subRegion.Left)),
            //             region.SubRegions.FirstOrDefault(r => r.Left.Overlap(subRegion.Right)),
            //         };

            //         foreach(var connectedRegion in connected)
            //         {
            //             if(connectedRegion != null)
            //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(connectedRegion), _graph.GetNodeByValue(subRegion), 0);
            //         }

            //     }


            // }
            
        }

        // var allConnections = _connections.ToArray();
        // while(_connections.Count > 0)   
        // {
        //     var current = _connections.Dequeue();
        //     var gridPosition = ToRegionPosition(current.Min);

        //     var surrounding = GetSubRegionsSurrounding(gridPosition);

        //     foreach(var subRegion in current.SubRegions)
        //     {
        //          var connected = new SubRegion[] 
        //         {
        //             surrounding.FirstOrDefault(r => r.Up.Overlap(subRegion.Down)),
        //             surrounding.FirstOrDefault(r => r.Down.Overlap(subRegion.Up)),
        //             surrounding.FirstOrDefault(r => r.Right.Overlap(subRegion.Left)),
        //             surrounding.FirstOrDefault(r => r.Left.Overlap(subRegion.Right))
        //         };

        //         foreach(var region in connected)
        //         {
        //             if(region != null)
        //                 _graph.AddUndirectedEdge(_graph.GetNodeByValue(region), _graph.GetNodeByValue(subRegion), 0);
        //         }
        //     }

        // }

    }

}

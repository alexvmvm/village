using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village;
using Village.Things;

/*
    - search quickly for objects in the scene
*/

public struct Edge
{
    public int x0;
    public int y0;
    public int x1;
    public int y1;
    public int overlap;

    public Edge(int x0, int y0, int x1, int y1, int overlap)
    {
        this.x0 = x0;
        this.y0 = y0;
        this.x1 = x1;
        this.y1 = y1;
        this.overlap = overlap;
    }

    public bool Overlap(Edge edge)
    {
        return 
            (this.x0 <= edge.x1 && this.x1 >= edge.x0) &&
            (this.y0 <= edge.y1 && this.y1 >= edge.y0) &&
            (this.overlap & edge.overlap) > 0;
    }

    bool IsBitSet(int b, int pos)
    {
    return (b & (1 << pos)) != 0;
    }

    public void DrawGizmos()
    {
        var bitPosition = 0;
        for(var x = x0; x <= x1; x++)
        {
            for(var y = y0; y <= y1; y++)
            {
                if(!IsBitSet(this.overlap, bitPosition))
                {
                    bitPosition++;
                    continue;
                }
                    
                Gizmos.color = new Color(0, 1, 0, 0.4f);
                Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                bitPosition++;
            }
        }
    }
}

public class SubRegion
{
    public Edge Right { get { return _right; } }
    public Edge Left { get { return _left; } }
    public Edge Up { get { return _up; } }
    public Edge Down { get { return _down; } }
    public Vector2Int Min { get { return _min; } }
    private Edge _right;
    private Edge _left;
    private Edge _up;
    private Edge _down;

    public HashSet<Vector2Int> Positions { get { return _positions; } }
    private HashSet<Vector2Int> _positions;
    private Game _game;
    private Vector2Int _min;
    private Vector2Int _max;
    
    public SubRegion(Game game, IEnumerable<Vector2Int> positions)
    {
        _game = game;
        _positions = new HashSet<Vector2Int>(positions);

        _max = positions.ElementAt(0);
        _min = positions.ElementAt(0);

        foreach(var p in positions)
        {
            if(p.x > _max.x)
                _max.x = p.x;
            else if(p.x < _min.x)
                _min.x = p.x;

            if(p.y > _max.y)
                _max.y = p.y;
            else if(p.y < _min.y)
                _min.y = p.y;
        }

        {
            var x = _max.x + 1;
            var mask = 0;
            for(var y = _min.y; y <= _max.y; y++)
            {
                var bitPosition = y - _min.y;
                var bit = positions.Contains(new Vector2Int(x - 1, y)) && _game.IsFloorForRegion(new Vector2Int(x, y)) ? 1 : 0;
                mask |= bit << bitPosition;
            }

            _right = new Edge(x, _min.y, x, _max.y, mask);
        }

        {
            var x = _min.x;
            var mask = 0;
            for(var y = _min.y; y <= _max.y; y++)
            {
                var bitPosition = y - _min.y;
                var bit = positions.Contains(new Vector2Int(x, y)) && _game.IsFloorForRegion(new Vector2Int(x, y))  ? 1 : 0;
                mask |= bit << bitPosition;
            }
            
            _left = new Edge(x, _min.y, x, _max.y, mask);
        }


        {
            var y = _max.y + 1;
            var mask = 0;
            for(var x = _min.x; x <= _max.x; x++)
            {
                var bitPosition = x - _min.x;
                var bit = positions.Contains(new Vector2Int(x, y - 1)) && _game.IsFloorForRegion(new Vector2Int(x, y))  ? 1 : 0;
                mask |= bit << bitPosition;
            }
            
            _up = new Edge(_min.x, y, _max.x, y, mask);
        }

        {
            var y = _min.y;
            var mask = 0;
            for(var x = _min.x; x <= _max.x; x++)
            {
                var bitPosition = x - _min.x;
                var bit = positions.Contains(new Vector2Int(x, y)) && _game.IsFloorForRegion(new Vector2Int(x, y))  ? 1 : 0;
                mask |= bit << bitPosition;
            }
            
            _down = new Edge(_min.x, y, _max.x, y, mask);
        }
    }

    public bool Contains(Vector2Int position)
    {
        return _positions.Contains(position);
    }

    public void DrawGizmos()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector2IntFloor();
        var mouseOver = _positions.Contains(mousePosition);
        // Gizmos.color = mouseOver ? new Color(0, 1, 0, 0.6f) : new Color(0, 0, 1, 0.4f);   
        
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        foreach(var p in Positions)
        {
            Gizmos.DrawCube(p.ToVector3(), Vector3.one);
        }

        if(!mouseOver)
            return;

        _left.DrawGizmos();
        _right.DrawGizmos();
        _up.DrawGizmos();
        _down.DrawGizmos();
    }
}

public class Region
{
    public List<SubRegion> SubRegions { get { return _regions; } }
    public Vector2Int Min { get { return _min; } }
    private Vector2Int _min;
    private Vector2Int _max;
    private RectInt _rect;
    private Game _game;
    private List<SubRegion> _regions;
    private HashSet<Vector2Int> _seen;
    private Queue<Vector2Int> _queue;
    private Dictionary<Vector2Int, SubRegion> _subRegionMap;

    public Region(Game game, Vector2Int min, Vector2Int max)
    {
        _game = game;
        _min = min;
        _max = max;
        _rect = new RectInt(_min.x, _min.y, _max.x - _min.x, _max.y - _min.y);
        _regions = new List<SubRegion>();
        _seen = new HashSet<Vector2Int>();
        _queue = new Queue<Vector2Int>();
        _subRegionMap = new Dictionary<Vector2Int, SubRegion>();
    }

    public void Refresh()
    {
        _regions.Clear();

        _queue.Clear();
        _seen.Clear();
        _subRegionMap.Clear();

        _queue.Enqueue(_min);

        for(var x = _min.x; x < _max.x; x++)
        {
            for(var y = _min.y; y < _max.y; y++)
            {
                var current = new Vector2Int(x, y);
                if(_seen.Contains(current))
                    continue;

                if(IsFloorForRegion(current))
                {
                    var region = CreateRegion(current);
                    if(region != null)
                    {
                        _regions.Add(region);

                        foreach(var position in region.Positions)
                        {
                            _subRegionMap.Add(position, region);
                        }
                    }
                        
                }
                else
                {
                    _seen.Add(current);
                }
                
            }  
        }
    }
    
    public SubRegion GetSubRegionAtPosition(Vector2Int position)
    {
        return _subRegionMap.ContainsKey(position) ? _subRegionMap[position] : null;
    }

    bool IsFloorForRegion(Vector2Int position)
    {
        return 
            _rect.Contains(position) && 
            _game.IsOnGrid(position.x, position.y) &&
            _game.IsFloorForRegion(position);
    }

    SubRegion CreateRegion(Vector2Int position)
    {
        _queue.Enqueue(position);

        var positions = new List<Vector2Int>();

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current) || !IsFloorForRegion(current))
                continue;
            
            _seen.Add(current);

            if(IsFloorForRegion(current))
            {
                positions.Add(current);
            }

            _queue.Enqueue(current.Up());
            _queue.Enqueue(current.Down());
            _queue.Enqueue(current.Left());
            _queue.Enqueue(current.Right());

        }

        return positions.Count > 0 ? new SubRegion(_game, positions) : null;      
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.red;
        var size = _rect.size.ToVector3();
        var center = _rect.min.ToVector3() + size/2 - Vector3.one * 0.5f;
        Gizmos.DrawWireCube(center, size);

        foreach(var region in _regions)
        {
            region.DrawGizmos();
        }
    }
}

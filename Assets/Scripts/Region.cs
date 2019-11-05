using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village;
using Village.Things;

/*
    - search quickly for objects in the scene
*/

public class SubRegion
{
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

        

        // _right += $"{_min.y}";    
        // for(var y = _min.y; y <= _max.y; y++)
        // {
        //     var x = _max.x + 1;
        //     var position = new Vector2Int(x, y);
        //     _right += $"{(IsFloorForRegion(position) ? 1 : 0)}";
        // }
    }

    bool IsFloorForRegion(Vector2Int position)
    {
        return _game.GetThingOnGrid(position) != null && !_game.GetThingOnGrid(position).blocksPath;
    }

    public bool Contains(Vector2Int position)
    {
        return _positions.Contains(position);
    }

    public void DrawGizmos()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector2IntFloor();
        var mouseOver = _positions.Contains(mousePosition);
        Gizmos.color = mouseOver ? new Color(0, 1, 0, 0.4f) : new Color(0, 0, 1, 0.4f);   
        
        foreach(var p in Positions)
        {
            Gizmos.DrawCube(p.ToVector3(), Vector3.one);
        }

        if(!mouseOver)
            return;

        Gizmos.color = new Color(0, 1, 1, 0.4f);
        for(var y = _min.y; y <= _max.y; y++)
        {
            var x = _max.x + 1;
            Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
        }
        
    }
}

public class Region
{
    private Vector2Int _min;
    private Vector2Int _max;
    private RectInt _rect;
    private Game _game;
    private List<SubRegion> _regions;
    private HashSet<Vector2Int> _seen;
    private Queue<Vector2Int> _queue;
    private bool _dirty;

    public Region(Game game, Vector2Int min, Vector2Int max)
    {
        _game = game;
        _min = min;
        _max = max;
        _rect = new RectInt(_min.x, _min.y, _max.x - _min.x, _max.y - _min.y);
        _regions = new List<SubRegion>();
        _seen = new HashSet<Vector2Int>();
        _queue = new Queue<Vector2Int>();

        _game.OnThingAdded += OnThingAdded;
        _game.OnThingRemoved += OnThingRemoved;
    }

    public void Search()
    {
        Debug.Log($"Redrawing {_rect.ToString()}");

        _regions.Clear();

        _queue.Clear();
        _seen.Clear();

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
                        _regions.Add(region);
                }
                else
                {
                    _seen.Add(current);
                }
                
            }  
        }
    }

    bool IsFloorForRegion(Vector2Int position)
    {
        return 
            _rect.Contains(position) && 
            _game.IsOnGrid(position.x, position.y) &&
            _game.GetThingOnGrid(position) != null && 
            !_game.GetThingOnGrid(position).blocksPath;
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

    void OnThingAdded(Thing thing)
    {
        if(_rect.Contains(thing.position))
            _dirty = true;
    }

    void OnThingRemoved(Thing thing)
    {
        if(_rect.Contains(thing.position))
            _dirty = true;
    }

    public void Update()
    {
        if(_dirty)
        {
            Search();
            _dirty = false;
        }
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

﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Village;
using Village.Things;
using System;

/*
    - search quickly for objects in the scene
*/

public struct Edge
{
    public int x0;
    public int y0;
    public int x1;
    public int y1;

    public Edge(int x0, int y0, int x1, int y1)
    {
        this.x0 = x0;
        this.y0 = y0;
        this.x1 = x1;
        this.y1 = y1;
    }

    public override int GetHashCode()
    {
        return $"{x0}-{y0}-{x1}-{y1}".GetHashCode();
    }

    public void DrawGizmos()
    {
        for(var x = x0; x <= x1; x++)
        {
            for(var y = y0; y <= y1; y++)
            {            
                Gizmos.color = new Color(0, 1, 0, 0.4f);
                Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
            }
        }
    }
}

public class SubRegion
{
    public List<Edge> Top { get { return _top; } }
    public List<Edge> Bottom { get { return _bottom; } }
    public List<Edge> Left { get { return _left; } }
    public List<Edge> Right { get { return _right; } }
    public Vector2Int Min { get { return _min; } }

    private List<Edge> _top;
    private List<Edge> _bottom;
    private List<Edge> _left;
    private List<Edge> _right;
    public HashSet<Vector2Int> Positions { get { return _positions; } }
    private HashSet<Vector2Int> _positions;
    private Game _game;
    private Region _region;
    private Vector2Int _min;
    private Vector2Int _max;
    // private Dictionary<TypeOfThing, List<Thing>> _things;
    private bool _dirty;
    
    public SubRegion(Game game, IEnumerable<Vector2Int> positions)
    {
        _game = game;
        _positions = new HashSet<Vector2Int>(positions);

        _top = new List<Edge>();
        _bottom = new List<Edge>();
        _left = new List<Edge>();
        _right = new List<Edge>();
        
        // _things = new Dictionary<TypeOfThing, List<Thing>>();

        // foreach(var p in _positions)
        // {
        //     AddThingToCache(_game.GetThingOnFloor(p));
        //     foreach(var t in _game.QueryLooseThings().Where(t => IsInRegion(t.Position)))
        //         AddThingToCache(t);
        // }

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

        PopulateTopEdges();
        PopulateBottomEdges();
        PopulateRightEdges();
        PopulateLeftEdges();

        UpdateCachedThings();
    }

    public void AddThingToCache(Thing thing)
    {
        if(thing == null)
            return;

        _dirty = true;
    }

    public void AddListeners()
    {
        _game.OnThingAdded += OnThingAdded;
        _game.OnThingRemoved += OnThingRemoved;
        Thing.OnThingMoved += OnThingMoved;
    }

    public void RemoveListeners()
    {
        _game.OnThingAdded -= OnThingAdded;
        _game.OnThingRemoved -= OnThingRemoved;
        Thing.OnThingMoved -= OnThingMoved;
    }

    void OnThingAdded(Thing thing)
    {
        if(!IsInRegion(thing.Position))
            return;

        _dirty = true;
    }

    void OnThingRemoved(Thing thing)
    {
        if(!IsInRegion(thing.Position)) // || !_things.ContainsKey(thing.Config.TypeOfThing))
            return;

        _dirty = true;
    }

    void OnThingMoved(Thing thing, Vector2Int previous, Vector2Int current)
    {
        if(IsInRegion(previous) && !IsInRegion(current))
        {
            _dirty = true;
        }
        else if(!IsInRegion(previous) && IsInRegion(current))
        {
            _dirty = true;
        }
    }

    public bool IsInRegion(Vector2Int position)
    {
        return _positions.Contains(position);
    }

    /*
        Querying
    */
    
    void UpdateCachedThings()
    {
         _cachedList.Clear();

        foreach(var p in _positions)
        {
            var floor = _game.GetThingOnFloor(p);
            if(!_cachedList.Contains(floor))
                _cachedList.Add(floor);

            foreach(var t in _game.QueryLooseThings().Where(t => IsInRegion(t.Position)))
            {
                if(!_cachedList.Contains(t))
                    _cachedList.Add(t);
            }
        }
    }

    private List<Thing> _cachedList = new List<Thing>();
    private List<Thing> CachedList
    {
        get
        {
            if(_dirty)
            {
                UpdateCachedThings();
                _dirty = false;
            }

            return _cachedList;
        }
    }

    public bool HasThing(Func<Thing, bool> filter)
    {
        return CachedList.Any(filter);
    }

    public Thing GetThing(Func<Thing, bool> filter)
    {
        return CachedList.Where(filter).First();
    }

    /*
        Edges
    */

    public void PopulateTopEdges()
    {
        var y = _max.y + 1;
        int? startX = null;
        for(var x = _min.x; x <= _max.x; x++)
        {
            var validPoint = _positions.Contains(new Vector2Int(x, y - 1)) && _game.IsFloorForRegion(new Vector2Int(x, y)); 
            if(validPoint && !startX.HasValue)
            {
                startX = x;
            }
            
            if(startX.HasValue && (!validPoint || x == _max.x))
            {
                var endX = x == _max.x && validPoint ? x : x - 1;
                _top.Add(new Edge(startX.Value, y, endX, y));
                startX = null;
            }
        }
    }

    public void PopulateBottomEdges()
    {
        var y = _min.y;
        int? startX = null;
        for(var x = _min.x; x <= _max.x; x++)
        {
            var validPoint = _positions.Contains(new Vector2Int(x, y)) && _game.IsFloorForRegion(new Vector2Int(x, y)); 
            if(validPoint && !startX.HasValue)
            {
                startX = x;
            }
            
            if(startX.HasValue && (!validPoint || x == _max.x))
            {
                var endX = x == _max.x && validPoint ? x : x - 1;
                _bottom.Add(new Edge(startX.Value, y, endX, y));
                startX = null;
            }
        }
    }

    public void PopulateRightEdges()
    {
        var x = _max.x + 1;
        int? startY = null;
        for(var y = _min.y; y <= _max.y; y++)
        {
            var validPoint = _positions.Contains(new Vector2Int(x - 1, y)) && _game.IsFloorForRegion(new Vector2Int(x, y)); 
            if(validPoint && !startY.HasValue)
            {
                startY = y;
            }
            
            if(startY.HasValue && (!validPoint || y == _max.y))
            {
                var endY = y == _max.y && validPoint ? y : y - 1;
                _right.Add(new Edge(x, startY.Value, x, endY));
                startY = null;
            }
        }
    }

    public void PopulateLeftEdges()
    {
        var x = _min.x;
        int? startY = null;
        for(var y = _min.y; y <= _max.y; y++)
        {
            var validPoint = _positions.Contains(new Vector2Int(x, y)) && _game.IsFloorForRegion(new Vector2Int(x - 1, y)); 
            if(validPoint && !startY.HasValue)
            {
                startY = y;
            }
            
            if(startY.HasValue && (!validPoint || y == _max.y))
            {
                var endY = y == _max.y && validPoint ? y : y - 1;
                _left.Add(new Edge(x, startY.Value, x, endY));
                startY = null;
            }
        }
    }

    public void DrawGizmos()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector2IntFloor();
        var mouseOver = _positions.Contains(mousePosition);
        
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        foreach(var p in Positions)
        {
            Gizmos.DrawCube(p.ToVector3(), Vector3.one);
        }

        if(!mouseOver)
            return;

        foreach(var edge in _top)
        {
            edge.DrawGizmos();
        }

        foreach(var edge in _bottom)
        {
            edge.DrawGizmos();
        }

        foreach(var edge in _left)
        {
            edge.DrawGizmos();
        }

        foreach(var edge in _right)
        {
            edge.DrawGizmos();
        }

        Gizmos.color = Color.green;
        foreach(var p in _positions)
        {
            Gizmos.DrawWireSphere(p.ToVector3(), 0.2f);
        }

#if UNITY_EDITOR

            var style = new GUIStyle();
            style.fontSize = 15;
            style.normal.textColor = Color.white;

            var label = "";

            var types = CachedList.Select(t => t.Config.TypeOfThing).Distinct();
            foreach(var type in types)
            {
                label += $"{type}: {CachedList.Count(t => t.Config.TypeOfThing == type)}\n";
            }

            // current actions
            UnityEditor.Handles.Label(Min.ToVector3(), label, style);

#endif
        
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
    private List<Vector2Int> _positions;

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
        _positions = new List<Vector2Int>();
    }

    public void Refresh()
    {
        _regions.Clear();

        _queue.Clear();
        _seen.Clear();
        _subRegionMap.Clear();

        _queue.Enqueue(_min);

        var current = new Vector2Int();

        for(var x = _min.x; x < _max.x; x++)
        {
            for(var y = _min.y; y < _max.y; y++)
            {
                current.x = x;
                current.y = y;

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
            _game.IsInBounds(position) &&
            _game.IsFloorForRegion(position);
    }

    SubRegion CreateRegion(Vector2Int position)
    {
        _queue.Enqueue(position);

        _positions.Clear();

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            
            if(_seen.Contains(current) || !IsFloorForRegion(current))
                continue;
            
            _seen.Add(current);

            if(IsFloorForRegion(current))
            {
                _positions.Add(current);
            }

            _queue.Enqueue(current.Up());
            _queue.Enqueue(current.Down());
            _queue.Enqueue(current.Left());
            _queue.Enqueue(current.Right());

        }

        return _positions.Count > 0 ? new SubRegion(_game, _positions) : null;      
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

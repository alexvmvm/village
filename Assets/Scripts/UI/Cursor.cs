using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor 
{
    private Game _game;
    private Transform _cursor;
    private SpriteRenderer _spriteRenderer;

    private Vector3 _down;
    private Vector3 _move;
    private Vector3 _min { get { return Vector3.Min(_down, _move);  } }
    private Vector3 _max { get { return Vector3.Max(_down, _move) + Vector3.one;  } }
    private bool _mouseDown;
    private Thing _current;

    public GameCursor(Game game)
    {
        _game = game;
        _cursor = _game.ObjectPooler.GetPooledObject().transform;

        _spriteRenderer = _cursor.gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _game.GetSprite("crosshair025");
        _spriteRenderer.sortingOrder = (int)SortingOrders.UI;
    }

    void MouseUp()
    {
        if(_current == null)
            return;


        for(var x = Mathf.FloorToInt(_min.x); x < Mathf.FloorToInt(_max.x); x++)
        {
            for(var y = Mathf.FloorToInt(_min.y); y < Mathf.FloorToInt(_max.y); y++)
            {
                if(!_game.IsOnGrid(x, y))
                    continue;
                _game.AddThing(_game.Create(_game.CurrentType.Value, x, y));        
            }
        }
        
    }

    public void Update()
    {
        if(MouseOverUIElement.MouseOverElement)
        {
            _cursor.gameObject.SetActive(false);
            return;
        }
        else
        {
            _cursor.gameObject.SetActive(true);
        }

        // setup example thing
        if(!_game.CurrentType.HasValue && _current != null)
        {
            _current = null;
        }
        else if(_game.CurrentType.HasValue && (_current == null || _current.type != _game.CurrentType.Value))
        {   
            _current = _game.Create(_game.CurrentType.Value);
        }

    

        // update cursor position
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var position = new Vector3(
            Mathf.RoundToInt(mousePosition.x), 
            Mathf.RoundToInt(mousePosition.y)
        );  

        _cursor.transform.position = position;

        // if(Input.GetKeyDown(KeyCode.Mouse0))
        // {
        //     var x = Mathf.FloorToInt(position.x);
        //     var y = Mathf.FloorToInt(position.y);
        //     var current = _game.GetThingOnGrid(x, y);
        //     _mouseDown = true;

        //     if(current == null) 
        //         return;
        //     if(_game.CurrentType.HasValue)            
        //         _game.AddThing(_game.Create(_game.CurrentType.Value, x, y));
        // }

        // check for dragging
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _mouseDown = true;
            _down = _cursor.transform.position;
        }
        
        if(_mouseDown)
        {
            _move = _cursor.transform.position;

            if(_current != null && _current.tileRule != null && _current.tileRule is TileRuleDefinition)
            {   
                if (_max.x - _min.x > _max.y - _min.y)
                {
                    _move.y = _down.y;
                }
                else
                {
                    _move.x = _down.x;
                }
            }

        }

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            _mouseDown = false;
            MouseUp();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            _game.CurrentType = null;
        }

    }

    public void DrawGizmos()
    {
        if(MouseOverUIElement.MouseOverElement)
            return;

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var position = new Vector3(
            Mathf.RoundToInt(mousePosition.x), 
            Mathf.RoundToInt(mousePosition.y)
        );

        if(_mouseDown)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_min + (_max - _min) / 2, _max - _min);
        
            Gizmos.DrawWireSphere(_min, 0.2f);
            Gizmos.DrawWireSphere(_max, 0.4f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, Vector3.one);
    }
}

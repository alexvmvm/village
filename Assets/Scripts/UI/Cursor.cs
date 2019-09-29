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
    private GameObject _cursorObj;
    private MeshFromPoints _meshFromPoints;
    private MeshRenderer _meshRenderer;
    private Vector2 _validUV = new Vector2(0.5f, 0.5f);
    private Vector2 _invalidUV = new Vector2(0, 0);
    
    public GameCursor(Game game)
    {
        _game = game;
        _cursor = _game.ObjectPooler.GetPooledObject().transform;

        _spriteRenderer = _cursor.gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _game.GetSprite("crosshair025");
        _spriteRenderer.sortingOrder = (int)SortingOrders.UI;
        
        _cursorObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Cursor Mesh"));
        _cursorObj.transform.SetParent(_game.transform);
        _cursorObj.transform.position = new Vector3(0, 0, -2);

        _meshFromPoints = _cursorObj.AddComponent<MeshFromPoints>();
        _meshRenderer = _cursorObj.AddComponent<MeshRenderer>();
        _cursorObj.SetActive(false);

    }

    void MouseDown()
    {
        if(_current == null)
            return;
    }

    void MouseUp()
    {
        if(_current == null)
            return;

        for(var x = Mathf.FloorToInt(_min.x); x < Mathf.FloorToInt(_max.x); x++)
        {
            for(var y = Mathf.FloorToInt(_min.y); y < Mathf.FloorToInt(_max.y); y++)
            {
                if(_current.construction != null && !_current.construction.IsPlaceableAt(x, y))
                    continue;

                _game.AddThing(_game.Create(_game.CurrentType.Value, x, y));       
            }
        }
        
    }

    void MouseMove()
    {
        if(_current == null)
            return;

        var list = new List<Quad>();
        for(var x = Mathf.FloorToInt(_min.x); x < Mathf.FloorToInt(_max.x); x++)
        {
            for(var y = Mathf.FloorToInt(_min.y); y < Mathf.FloorToInt(_max.y); y++)
            {
                var valid = _current.construction != null && !_current.construction.IsPlaceableAt(x, y) ?
                    _invalidUV : _validUV;

                list.Add(new Quad {
                    position = new Vector2(x - 0.5f, y - 0.5f),
                    uv = valid,
                    tUnit = 0.5f
                });    
            }
        }

        _meshFromPoints.Create(list.ToArray());
    }

    public void Update()
    {
        if(MouseOverUIElement.MouseOverElement)
        {
            _spriteRenderer.enabled = false;
            return;
        }
        else
        {
            _spriteRenderer.enabled = true;
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

        _cursorObj.SetActive(_current != null);

        // check for dragging
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _mouseDown = true;
            _down = _cursor.transform.position;
            MouseDown();
        }
        
        // update move position
        // check if current tile is rule based so we can setup
        // the correct max/min
        if(_mouseDown)
        {
            _move = _cursor.transform.position;

            if(_current != null && _current.pipe)
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
        
            // move mouse callback for drawing previews etc.
        }
        else
        {
            _down = _cursor.transform.position;
            _move = _down;
        }

        MouseMove();

        // if player has finished interaction
        // call mouse up event
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            _mouseDown = false;
            MouseUp();
        }

        // if right click cancel current
        // tile
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

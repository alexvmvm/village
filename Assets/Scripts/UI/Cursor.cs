using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village;
using Village.Things;

public class GameCursor 
{
    public TypeOfThing? CurrentType;
    private Game _game;
    private Transform _crosshairCursor;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _down;
    private Vector3 _move;
    private Vector3 _min { get { return Vector3.Min(_down, _move);  } }
    private Vector3 _max { get { return Vector3.Max(_down, _move) + Vector3.one;  } }
    private bool _mouseDown;
    private Thing _currentToBuild;
    private GameObject _cursorMeshObj;
    private MeshFromPoints _meshFromPoints;
    private MeshRenderer _meshRenderer;
    private Vector2 _validUV = new Vector2(0.5f, 0.5f);
    private Vector2 _invalidUV = new Vector2(0, 0);
    private Thing _mouseOverThing;
    private InfoPanel _infoPanel;
    private Vector3 _cursorPosition;
    private bool _fixCursorPosition;
    private ActionPanel _actionPanel;
    
    public GameCursor(Game game)
    {
        _game = game;
        _crosshairCursor = new GameObject("cursor").transform;

        _spriteRenderer = _crosshairCursor.gameObject.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Assets.GetSprite("crosshair025");
        _spriteRenderer.sortingOrder = (int)SortingOrders.UI;
        
        _cursorMeshObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Cursor Mesh"));
        _cursorMeshObj.transform.position = new Vector3(0, 0, -2);

        _meshFromPoints = _cursorMeshObj.GetComponent<MeshFromPoints>();
        _meshRenderer = _cursorMeshObj.GetComponent<MeshRenderer>();
        _cursorMeshObj.SetActive(false);

        _infoPanel = MonoBehaviour.FindObjectOfType<InfoPanel>();
        _actionPanel = MonoBehaviour.FindObjectOfType<ActionPanel>();

    }

    void MouseDown()
    {
        if(_currentToBuild == null && _mouseOverThing != null && _mouseOverThing.CanBeSeletected())
        {
            _fixCursorPosition = true;
        }
    }

    void MouseUp()
    {
        if(_currentToBuild == null)
            return;

        for(var x = Mathf.FloorToInt(_min.x); x < Mathf.FloorToInt(_max.x); x++)
        {
            for(var y = Mathf.FloorToInt(_min.y); y < Mathf.FloorToInt(_max.y); y++)
            {
                if(_currentToBuild.construction != null && !_currentToBuild.construction.IsPlaceableAt(x, y))
                    continue;

                _game.AddThing(_game.Create(CurrentType.Value, x, y));       
            }
        }
        
    }

    void MouseMove()
    {
        if(_currentToBuild == null)
            return;

        var list = new List<Quad>();
        for(var x = Mathf.FloorToInt(_min.x); x < Mathf.FloorToInt(_max.x); x++)
        {
            for(var y = Mathf.FloorToInt(_min.y); y < Mathf.FloorToInt(_max.y); y++)
            {
                var valid = _currentToBuild.construction != null && !_currentToBuild.construction.IsPlaceableAt(x, y) ?
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
        // if currently over another panel 
        // don't do anything
        if(MouseOverUIElement.MouseOverElement)
        {
            //_spriteRenderer.enabled = false;
            return;
        }

        // if spriteRenderer has been disabled
        // due to mouse over panel, re-enable
        // if(!_spriteRenderer.enabled)
        //     _spriteRenderer.enabled = true;


        // setup example thing on _current from 
        // selected type on game
        if(!CurrentType.HasValue && _currentToBuild != null)
        {
            _currentToBuild = null;
        }
        else if(CurrentType.HasValue && (_currentToBuild == null || _currentToBuild.type != CurrentType.Value))
        {   
            _currentToBuild = _game.Create(CurrentType.Value);
        }

        // update cursor position to mouse position
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var position = new Vector3(
            Mathf.RoundToInt(mousePosition.x), 
            Mathf.RoundToInt(mousePosition.y)
        );  

        if(_currentToBuild != null)
            _fixCursorPosition = false;
        
        // if cursor has moved to a different grid position
        if(_cursorPosition != position && !_fixCursorPosition)
        {
            _cursorPosition = position;
            _crosshairCursor.transform.position = position;
            _mouseOverThing = _game.GetThingOnGrid(position.ToVector2IntFloor());
            
            if(_infoPanel != null)
                _infoPanel.Setup(_mouseOverThing);
            if(_actionPanel != null)
                _actionPanel.Setup(_mouseOverThing);
        }

        // disable cursor mesh if no current selected
        // thing being constructed
        _cursorMeshObj.SetActive(_currentToBuild != null);

        
        // check for dragging
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _mouseDown = true;
            _down = _crosshairCursor.transform.position;
            MouseDown();
        }
        
        // update move position
        // check if current tile is rule based so we can setup
        // the correct max/min
        if(_mouseDown)
        {
            _move = _crosshairCursor.transform.position;

            if(_currentToBuild != null && _currentToBuild.pipe)
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
            _down = _crosshairCursor.transform.position;
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
            CurrentType = null;
            _fixCursorPosition = false;
            _actionPanel.Clear();
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

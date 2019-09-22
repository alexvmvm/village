using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor 
{
    private Game _game;
    private Transform _cursor;
    private SpriteRenderer _spriteRenderer;

    private Vector3 _min;
    private Vector3 _max;
    private bool _mouseDown;

    private Vector2Int _minVec2Int { get { return _min.ToVector2IntFloor(); } }
    private Vector2Int _maxVec2Int { get { return _max.ToVector2IntFloor(); } }

    public GameCursor(Game game)
    {
        _game = game;
        _cursor = _game.ObjectPooler.GetPooledObject().transform;

        _spriteRenderer = _cursor.gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _game.GetSprite("crosshair025");
        _spriteRenderer.sortingOrder = (int)SortingOrders.UI;
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

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var position = new Vector3(
            Mathf.RoundToInt(mousePosition.x), 
            Mathf.RoundToInt(mousePosition.y)
        );  

        _cursor.transform.position = position;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _mouseDown = true;
            _min = _cursor.transform.position;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            _mouseDown = false;
        }

        if(_mouseDown)
        {
            _max = _cursor.transform.position;
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
        
            Gizmos.DrawWireSphere(_minVec2Int.ToVector3(), 0.2f);
            Gizmos.DrawWireSphere(_maxVec2Int.ToVector3(), 0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, Vector3.one);
    }
}

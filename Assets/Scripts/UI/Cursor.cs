using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor 
{
    private Game _game;
    private Transform _cursor;
    private SpriteRenderer _spriteRenderer;

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

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, Vector3.one);
    }
}

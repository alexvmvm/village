using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor 
{
    private Game _game;

    public GameCursor(Game game)
    {
        _game = game;
    }

    public void Update()
    {

    }

    public void DrawGizmos()
    {
        if(MouseOverUIElement.MouseOverElement)
            return;

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var position = new Vector3(
            Mathf.FloorToInt(mousePosition.x), 
            Mathf.FloorToInt(mousePosition.y)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, Vector3.one);
    }
}

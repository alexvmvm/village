using UnityEngine;
using System;

public class Thing
{   
    // properties    
    public Sprites sprite;
    public Game main;
    public Transform transform; 
    public SpriteRenderer spriteRenderer;
    public bool fixedToGrid;
    public ITileRule tileRule;
    public int group;
    public TypeOfThing type;
    public bool floor;
    public bool wall;
    public bool gurder;
    public bool pressurized;

    public Thing(TypeOfThing type)
    {
        this.type = type;
    }

    public Vector2Int gridPosition
    {
        get
        {
            return new Vector2Int(
                Mathf.FloorToInt(transform.position.x),
                Mathf.FloorToInt(transform.position.y));
        }
    }

    public void Setup()
    {
        RefreshSprite();
    }

    public void SetSprite()
    {
        this.spriteRenderer.sprite = tileRule != null ?
            main.GetSprite(tileRule.GetSprite(GetGridPositions())) :
            main.GetSprite(sprite);
    }

    public void RefreshSprite()
    {
        SetSprite();

        var px = Mathf.FloorToInt(transform.position.x);
        var py = Mathf.FloorToInt(transform.position.y);
        
        for(var x = px - 1; x <= px + 1; x++)
        {
            for(var y = py - 1; y <= py + 1; y++)
            {
                if(x == px && y == py)
                    continue;
                
                var thing = main.GetThingOnGrid(x, y);
                if(thing != null)
                {
                    thing.SetSprite();
                }
            }
        }      
    }
    
    public Position GetGridPositions()
    {
        var position = Position.None;

        var px = Mathf.FloorToInt(transform.position.x);
        var py = Mathf.FloorToInt(transform.position.y);

        for(var x = px - 1; x <= px + 1; x++)
        {
            for(var y = py - 1; y <= py + 1; y++)
            {
                if(x == px && y == py)
                    continue;
                
                var thing = main.GetThingOnGrid(x, y);
                if(thing != null && thing.group == group)
                {
                    var vector = new Vector2Int(x - px, y - py);
                    if(vector == Vector2Int.up)
                        position = position | Position.Top;
                    else if(vector == Vector2Int.down)
                        position = position | Position.Bottom;
                    else if(vector == Vector2Int.left)
                        position = position | Position.Left;
                    else if(vector == Vector2Int.right)
                        position = position | Position.Right;
                    // else if(vector == Vector2Int.up + Vector2Int.left)
                    //     position = position | Position.TopLeft;
                    // else if(vector == Vector2Int.up + Vector2Int.right)
                    //     position = position | Position.TopRight;
                    // else if(vector == Vector2Int.down + Vector2Int.left)
                    //     position = position | Position.BottomLeft;
                    // else if(vector == Vector2Int.down + Vector2Int.right)
                    //     position = position | Position.BottomRight;
                }

            }
        }

        return position;
    }

    public void Destroy()
    {
        transform.gameObject.SetActive(false);
    }
    
    public void Update()
    {
        
    }

    public void DrawGizmos()
    {
        if(floor)
        {
            Gizmos.color = pressurized ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
  
    }
}
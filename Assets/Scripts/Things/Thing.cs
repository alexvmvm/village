using UnityEngine;
using System;

public class Thing
{   
    // properties    
    public string sprite;
    public string name;
    public string belongsToFamily;
    public bool assignToFamily;
    public Game game;
    public int hitpoints = 100;
    public Transform transform; 
    public SpriteRenderer spriteRenderer;
    public Color color = Color.white;
    public Vector3 scale = Vector3.one;
    public int sortingOrder;
    public bool fixedToGrid;
    public ITileRule tileRule;
    public int group;
    public TypeOfThing type;
    public bool floor;
    public bool playerBuiltFloor;
    public bool wall;
    public bool buildOn;
    public bool pipe;
    public bool edible;
    
    /*
        Get Thing
    */
    public bool resource;
    public TypeOfThing produces;
    public TypeOfThing requiredToGet;
    
    /*
        Audio
    */
    public string positionalAudioGroup;
    public string pathTag;
    public bool walkable = true;
    public Construction construction;
    public Agent agent;
    public FamilyChest familyChest;
    public Coop coop;
    public Inventory inventory;

    /*
        Show Label
    */
    public bool showLabel;

    private TextMesh _textMesh;
    private GameObject _labelObj;

    public Thing(TypeOfThing type, Transform transform)
    {
        this.type = type;
        this.transform = transform;
        this.spriteRenderer = transform.GetComponent<SpriteRenderer>();
        this.produces = type;
        this.requiredToGet = TypeOfThing.None;
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

    public void SetLabel(string label)
    {
        if(_labelObj == null)
        {
            _labelObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Label"));    
            _labelObj.transform.SetParent(transform);
            _labelObj.transform.localPosition = new Vector3(0, -0.8f, 0);
            _labelObj.GetComponentInChildren<MeshRenderer>().sortingOrder = (int)SortingOrders.Labels;
            _textMesh = _labelObj.GetComponentInChildren<TextMesh>();
        }

        _textMesh.text = label;
    }

    public void Setup()
    {
        RefreshSprite();

        if(!string.IsNullOrEmpty(pathTag))
        {
            game.UpdateAstarPath(transform.position.ToVector2IntFloor(), pathTag, walkable);
        }
    }

    public void SetSprite()
    {
        var spriteName = tileRule != null ? tileRule.GetSprite(GetGridPositions()) : sprite;
        this.spriteRenderer.sprite = game.GetSprite(spriteName);
        this.spriteRenderer.sortingOrder = sortingOrder;
        this.spriteRenderer.color = color;
        this.transform.localScale = scale;
        this.transform.rotation = game.GetSpriteRotation(spriteName);
    }

    public void RefreshSprite()
    {
        SetSprite();

        if(!fixedToGrid)
            return;
        
        var px = Mathf.FloorToInt(transform.position.x);
        var py = Mathf.FloorToInt(transform.position.y);
        
        for(var x = px - 1; x <= px + 1; x++)
        {
            for(var y = py - 1; y <= py + 1; y++)
            {
                if(x == px && y == py)
                    continue;
                
                var thing = game.GetThingOnGrid(x, y);
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
                
                var thing = game.GetThingOnGrid(x, y);
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
                }

            }
        }

        return position;
    }

    public Thing[] GetNeighboursOnGrid()
    {
        return new Thing[]
        {
            game.GetThingOnGrid(gridPosition + Vector2Int.up),
            game.GetThingOnGrid(gridPosition + Vector2Int.down),
            game.GetThingOnGrid(gridPosition + Vector2Int.left),
            game.GetThingOnGrid(gridPosition + Vector2Int.right)
        };
    }

    public bool CanBeSeletected()
    {
        return assignToFamily;
    }

    public void OnSelected()
    {
        switch(type)
        {
            case TypeOfThing.FamilyChest:
                Debug.Log("Selcted");
                break;
            default:
                break;
        }
    }

    public void Destroy()
    {
        if(!string.IsNullOrEmpty(pathTag))
        {
            game.UpdateAstarPath(transform.position.ToVector2IntFloor(), "ground", true);
        }

        if(_labelObj != null)
            GameObject.Destroy(_labelObj);

        transform.gameObject.SetActive(false);
        game.Things.Remove(this);
    }
    
    public virtual void Update()
    {
        if(agent != null)
            agent.Update();
        
        if(familyChest != null)
            familyChest.Update();

        if(coop != null)
            coop.Update();

        if(resource)
            SetLabel($"x{hitpoints}");
    }

    public void DrawGizmos()
    {
        if(familyChest != null)
            familyChest.DrawGizmos();

        if(agent != null)
            agent.DrawGizmos();

        if(coop != null)
            coop.DrawGizmos();

#if UNITY_EDITOR
               
            if(!string.IsNullOrEmpty(belongsToFamily))
            {
                var style = new GUIStyle();
                style.fontSize = 10;
                style.normal.textColor = Color.white;

                // current actions
                var position = transform.position + Vector3.up;
                UnityEditor.Handles.Label(position, belongsToFamily, style);
            }

       
#endif
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfThing
{
    Grass,
    Wall,
    Gurder
}


[Flags]
public enum Position
{
    None        = 0,
    Top         = 1,
    TopRight    = 2,
    Right       = 4,
    BottomRight = 8,
    Bottom      = 16,
    BottomLeft  = 32,
    Left        = 64,
    TopLeft     = 128
}

public class Game : MonoBehaviour
{
    public Vector2Int MapSize = Vector2Int.one * 10;
    public ObjectPooler ObjectPooler;
    public Thing[,] Grid;
    public List<Thing> Things;
    private Dictionary<string, Sprite> _sprites;

    // simulations
    void Awake()
    {
        // create array of things
        Things = new List<Thing>();

        // simulations

        // load all sprites
        _sprites = new Dictionary<string, Sprite>();
        foreach(var sprite in Resources.LoadAll<Sprite>(""))
        {
            _sprites.Add(sprite.name, sprite);
        }

        Grid = new Thing[MapSize.x, MapSize.y];
    }

    void Start()
    {
        for(var x = 0; x < MapSize.x; x++)
        {
            for(var y = 0; y < MapSize.y; y++)
            {
                AddThing(Create(TypeOfThing.Grass, x, y));
            }
        }
    }

    /*
        Sprites
    */

    public Sprite GetSprite(Sprites sprite)
    {
        return GetSprite(sprite.ToString().ToLower());
    }

    public Sprite GetSprite(string name)
    {
        if(!_sprites.ContainsKey(name))
            throw new System.Exception(string.Format("Unable to find sprite {0} in resources", name));

        return _sprites[name];
    }

    public Thing GetThingOnGrid(Vector2Int position)
    {
        return GetThingOnGrid(position.x, position.y);
    }

    public Thing GetThingOnGrid(int x, int y)
    {
        if(x >= 0 && x < MapSize.x && y >= 0 && y < MapSize.y)
        {
            return Grid[x, y];
        }

        return null;
    }

    /*
        Things
    */
    public void AddThing(Thing thing)
    {
        if(thing.fixedToGrid)
        {
            var existing = GetThingOnGrid(thing.gridPosition.x, thing.gridPosition.y);
            if(existing != null)
            {
                RemoveThing(existing);
            }

            Grid[thing.gridPosition.x, thing.gridPosition.y] = thing;
        }

        Things.Add(thing);
        
        thing.Setup();
    }

    public void RemoveThing(Thing thing)
    {
        if(thing == null)
            return;

        if(thing.fixedToGrid)
        {
            Grid[thing.gridPosition.x, thing.gridPosition.y] = null;
        }

        thing.Destroy();
        Things.Remove(thing);
    }

    public Thing Create(TypeOfThing thingType, int x, int y)
    {
        var transform = ObjectPooler.GetPooledObject().transform;
        transform.position = new Vector3(x, y, 0);

        var thing = new Thing(thingType)
        {
            transform = transform,
            main = this,
            spriteRenderer = transform.GetComponent<SpriteRenderer>()
        };

        switch(thingType)
        {
            case TypeOfThing.Grass:
                thing.sprite = Sprites.Grass_0;
                thing.fixedToGrid = true;
                thing.tileRule = new RandomTiles(Sprites.Colored_5, Sprites.Colored_6, Sprites.Colored_7);
                thing.floor = true;
                break;
            case TypeOfThing.Wall:
                thing.fixedToGrid = true;
                thing.tileRule = new TileRuleDefinition(Sprites.Wall_None);
                thing.group = 1;
                thing.wall = true;
                break;
            case TypeOfThing.Gurder:
                thing.fixedToGrid = true;
                thing.tileRule = new TileRuleDefinition(Sprites.Gurder_None);
                thing.group = 2;
                thing.gurder= true;
                break;
            default:
                throw new System.Exception(string.Format("Unable to create tile {0}", thingType.ToString()));
        }
                
        return thing;
    }

    void Update()
    {
        for(var i = 0; i < Things.Count; i++) 
        {
            Things[i].Update();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            AddThing(Create(TypeOfThing.Wall, 
                UnityEngine.Random.Range(0, MapSize.x), 
                UnityEngine.Random.Range(0, MapSize.y)));
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            var random = Things[UnityEngine.Random.Range(0, Things.Count)];
            RemoveThing(random);
        }
    }
    
    #if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;

        for(var i = 0; i < Things.Count; i++) 
        {
            Things[i].DrawGizmos();
        }
    }

    #endif
}

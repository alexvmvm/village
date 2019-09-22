using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfThing
{
    Grass,
    Stream,
    Path,
    Tree,
    Stone
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
    private Dictionary<string, AudioClip> _audioClips;
    private GameCursor _cursor;

    private List<PositionalAudio> _positionalAudio;

    // simulations
    void Awake()
    {
        // load all sprites
        _sprites = new Dictionary<string, Sprite>();
        foreach(var sprite in Resources.LoadAll<Sprite>(""))
        {
            _sprites.Add(sprite.name, sprite);
        }

        Debug.Log(string.Format("Loaded {0} sprites", _sprites.Count));

        // load all audio
        _audioClips = new Dictionary<string, AudioClip>();
        foreach(var audioClip in Resources.LoadAll<AudioClip>("Music"))
        {
            _audioClips.Add(audioClip.name, audioClip);
        }

        Debug.Log(string.Format("Loaded {0} audio clips", _audioClips.Count));

        // create array of things
        Things = new List<Thing>();

        // cursor
        _cursor = new GameCursor(this);

        Grid = new Thing[MapSize.x, MapSize.y];

        // positional audio
        _positionalAudio = new List<PositionalAudio>()
        {
            new PositionalAudio(this, "river", "running_water"),
            new PositionalAudio(this, "trees", "birds")
        };
    }

    void Start()
    {
        for(var x = 0; x < MapSize.x; x++)
        {
            for(var y = 0; y < MapSize.y; y++)
            {

                if(x == MapSize.x - 3)
                {
                    AddThing(Create(TypeOfThing.Stream, x, y));
                }
                else if(x == Mathf.FloorToInt(MapSize.x / 2))
                {
                    AddThing(Create(TypeOfThing.Path, x, y));
                }
                else if(Mathf.PerlinNoise(x * 0.1f, y * 0.1f) > 0.6f)
                {
                    AddThing(Create(TypeOfThing.Tree, x, y));
                }
                else if(Mathf.PerlinNoise(x * 100.1f, y * 100.1f) > 0.8f)
                {
                    AddThing(Create(TypeOfThing.Stone, x, y));
                }
                else
                {
                     AddThing(Create(TypeOfThing.Grass, x, y)); 
                }
                
            }
        }

        for(var y = 0; y < MapSize.y; y++)
        {
          
        }
    }

    /*
        Sprites
    */

    public Sprite GetSprite(string name)
    {
        var spriteName = name.Contains("!") ? name.Substring(0, name.IndexOf('!')) : name;
        if(!_sprites.ContainsKey(spriteName))
            throw new System.Exception(string.Format("Unable to find sprite {0} in resources", spriteName));

        return _sprites[spriteName];
    }

    public Quaternion GetSpriteRotation(string name)
    {
        if(!name.Contains("!"))
            return Quaternion.identity;
        var rotation = int.Parse(name.Substring(name.IndexOf('!') + 1));
        return Quaternion.Euler(0, 0, rotation);
    }

    public AudioClip GetAudioClip(string name)
    {
        if(!_audioClips.ContainsKey(name))
            throw new System.Exception(string.Format("Unable to find audio clip {0} in resources", name));

        return _audioClips[name];
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
                thing.sprite = "grass_0";
                thing.fixedToGrid = true;
                thing.tileRule = new RandomTiles("colored_5", "colored_6", "colored_7");
                thing.floor = true;
                break;
            case TypeOfThing.Stream:
                thing.fixedToGrid = true;
                thing.tileRule = new TileRuleDefinition(
                    "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
                    "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
                    "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
                    "stream_4", "stream_1", "stream_1!90");
                thing.group = 1;
                thing.wall = true;
                thing.positionalAudioGroup = "river";
                break;
            case TypeOfThing.Path:
                thing.fixedToGrid = true;
                thing.tileRule = new TileRuleDefinition(
                    "path_5!180", "path_5", "path_5!270", "path_5!90", 
                    "path_2!180", "path_2!90", "path_2", "path_2!270",
                    "path_3", "path_3!180", "path_3!90", "path_3!270",
                    "path_4", "path_1", "path_1!90");
                thing.group = 1;
                thing.wall = true;
                break;
            case TypeOfThing.Tree:
                thing.sprite = "grass_0";
                thing.fixedToGrid = true;
                thing.tileRule = new RandomTiles("tree_1", "tree_2", "tree_3");
                thing.floor = true;
                thing.positionalAudioGroup = "trees";
                break;
            case TypeOfThing.Stone:
                thing.sprite = "stone_1";
                thing.fixedToGrid = true;
                thing.floor = true;
                break;
            default:
                throw new System.Exception(string.Format("Unable to create tile {0}", thingType.ToString()));
        }
                
        return thing;
    }

    void Update()
    {
        _cursor.Update();

        for(var i = 0; i < Things.Count; i++) 
        {
            Things[i].Update();
        }

        foreach(var positionalAudio in _positionalAudio)
        {
            positionalAudio.Update();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            AddThing(Create(TypeOfThing.Stream, 
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

        _cursor.DrawGizmos();

        for(var i = 0; i < Things.Count; i++) 
        {
            Things[i].DrawGizmos();
        }

        foreach(var positionalAudio in _positionalAudio)
        {
            positionalAudio.DrawGizmos();
        }
    }

    #endif
}

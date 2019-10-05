using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum TypeOfThing
{
    None,
    Grass,
    Stream,
    Path,
    Rock,
    Stone,
    Clay,
    Ore,
    Tree,
    BerryBush,
    Mushroom,
    Wood,
    FallenWood,
    Villager,
    Hen,
    Chick,
    Rooster,
    MudFloor,
    MudFloorBlueprint,
    StoneFloor,
    StoneWall,
    WoodFloor,
    WoodFloorBlueprint,
    ForagedWall,
    ForagedWallBlueprint,
    WoodWall,
    WoodWallBlueprint,
    StoneFloorBlueprint,
    StoneWallBlueprint,
    FenceBlueprint,
    Fence,
    DoorBlueprint,
    Door,
    GateBlueprint,
    Gate,
    ChickenCoopBlueprint,
    ChickenCoop,
    BedBlueprint,
    Bed,
    ForagedBed,
    ForagedBedBlueprint,
    FamilyChest,
    FamilyChestBlueprint,
    ClayForge,
    ClayForgeBlueprint,
    Axe
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

public delegate void ThingAdded(Thing thing);
public delegate void ThingRemoved(Thing thing);

public class Game : MonoBehaviour
{
    public ZoneGraph ZoneGraph { get { return _zoneGraph; } }
    public Vector2Int MapSize = Vector2Int.one * 10;
    public ObjectPooler ObjectPooler;
    public AstarPath AstarPath;
    public Thing[,] Grid;
    public List<Thing> Things;
    public List<Thing> AllThings;
    public TypeOfThing? CurrentType;
    public WorldTime WorldTime;
    public ThingAdded OnThingAdded;
    public ThingRemoved OnThingRemoved;
    private Dictionary<string, Sprite> _sprites;
    private Dictionary<string, AudioClip> _audioClips;
    private Dictionary<string, Material> _materials;
    private GameCursor _cursor;
    private List<PositionalAudio> _positionalAudio;
    private Director _director;
    private ZoneGraph _zoneGraph;
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

        _materials = new Dictionary<string, Material>();
        foreach(var material in Resources.LoadAll<Material>("Materials"))
        {
            _materials.Add(material.name, material);
        }

        Debug.Log(string.Format("Loaded {0} materials", _materials.Count));

        // zone graph
        _zoneGraph = new ZoneGraph(this);

        // create array of things
        Things = new List<Thing>();
        
        // setup all things
        AllThings = new List<Thing>();

        foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
        {
            if(thingType == TypeOfThing.None)
                continue;
            var thing = Create(thingType);
            if(thing.agent != null)
                thing.agent.PauseAgent();
            AllThings.Add(thing);
        }

        // cursor
        _cursor = new GameCursor(this);

        Grid = new Thing[MapSize.x, MapSize.y];

        // time 
        WorldTime = new WorldTime(360, 5, 23);

        // positional audio
        _positionalAudio = new List<PositionalAudio>()
        {
            new PositionalAudio(this, "river", "running_water"),
            new PositionalAudio(this, "trees", "birds")
        };

        // director ai
        _director = new Director(this);
    }

    void Start()
    {
        for(var x = 0; x < MapSize.x; x++)
        {
            for(var y = 0; y < MapSize.y; y++)
            {

                var noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                var noise2 = Mathf.PerlinNoise(x * 1234.1f, y * 1234.1f);
                var noise3 = Mathf.PerlinNoise(x * 34343.1f, y * 34343.1f);
                
                if(noise > 0.6f && noise < 0.65f)
                {
                    AddThing(Create(TypeOfThing.BerryBush, x, y));
                }
                else if(noise > 0.65f)
                {
                    AddThing(Create(TypeOfThing.Tree, x, y));
                }
                else if(noise2 > 0.8f)
                {
                    AddThing(Create(TypeOfThing.Rock, x, y));
                }
                else if(noise3 > 0.85f)
                {
                    AddThing(Create(TypeOfThing.Mushroom, x, y));
                }
                else
                {
                     AddThing(Create(TypeOfThing.Grass, x, y)); 
                }
                
            }
        }

        for(var y = 0; y < MapSize.y; y++)
        {
            var x = Mathf.FloorToInt(MapSize.x / 2);
            AddThing(Create(TypeOfThing.Path, x, y));
        }

        var rand = 0;
        for(var y = 0; y < MapSize.y; y++)
        {
            var x = MapSize.x - 3;
            rand = UnityEngine.Random.Range(0, 4);
            AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x - 1, y));
            AddThing(Create(TypeOfThing.Stream, x, y));
            rand = UnityEngine.Random.Range(0, 2);
            AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x + 1, y));
        }

        for(var i = 0; i < 20; i++)
        {
            var x = UnityEngine.Random.Range(0, MapSize.x);
            var y = UnityEngine.Random.Range(0, MapSize.y);

            CreateAndAddThing(TypeOfThing.FallenWood, x, y);
        }

        // for(var i = 0; i < 5; i++) 
        // {
        //     var x = UnityEngine.Random.Range(0, MapSize.x);
        //     var y = UnityEngine.Random.Range(0, MapSize.y);

        //     AddThing(Create(TypeOfThing.Hen, x, y));
        // }

        // for(var i = 0; i < 10; i++) 
        // {
        //     var x = UnityEngine.Random.Range(0, MapSize.x);
        //     var y = UnityEngine.Random.Range(0, MapSize.y);

        //     AddThing(Create(TypeOfThing.Chick, x, y));
        // }

        // for(var i = 0; i < 3; i++) 
        // {
        //     var x = UnityEngine.Random.Range(0, MapSize.x);
        //     var y = UnityEngine.Random.Range(0, MapSize.y);

        //     AddThing(Create(TypeOfThing.Rooster, x, y));
        // }

        _zoneGraph.Start();
    }

    /*
        Sprites
    */

    public Sprite GetSprite(string name)
    {
        if(string.IsNullOrEmpty(name))
            throw new System.Exception("Sprite name is null");

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

    public Material GetMaterial(string name)
    {
        if(!_materials.ContainsKey(name))
            throw new System.Exception(string.Format("Unable to find material {0} in resources", name));

        return _materials[name];
    }

    public Thing GetThingOnGrid(Vector2Int position)
    {
        return GetThingOnGrid(position.x, position.y);
    }

    public bool IsOnGrid(int x, int y)
    {
        return x >= 0 && x < MapSize.x && y >= 0 && y < MapSize.y;
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
    public Thing AddThing(Thing thing)
    {
        if(thing.fixedToGrid)
        {
            if(IsOnGrid(thing.gridPosition.x, thing.gridPosition.y))
            {
                var existing = GetThingOnGrid(thing.gridPosition.x, thing.gridPosition.y);
                if(existing != null)
                {
                    RemoveThing(existing);
                }

                Grid[thing.gridPosition.x, thing.gridPosition.y] = thing;

                Things.Add(thing);
                thing.Setup();

                if(OnThingAdded != null)
                    OnThingAdded(thing);
            }
        }
        else
        {
            Things.Add(thing);
            thing.Setup();

            if(OnThingAdded != null)
                    OnThingAdded(thing);
        }

        switch(thing.type)
        {
            case TypeOfThing.Villager:
                EventManager.TriggerEvent(Constants.VILLAGER_ARRIVED);
            break;
        }

        return thing;
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

        if(OnThingRemoved != null)
            OnThingRemoved(thing);
    }

    public Thing Create(TypeOfThing thingType)
    {
        return Create(thingType, 0, 0);
    }

    public Thing Create(TypeOfThing thingType, int x, int y)
    {
        var obj = ObjectPooler.GetPooledObject();
        obj.transform.position = new Vector3(x, y, 0);
        obj.transform.name = thingType.ToString();

        var thing = new Thing(thingType, obj.transform) 
        { 
            game = this 
        };

        switch(thingType)
        {
            case TypeOfThing.Grass:
                thing.name = "grass";
                thing.sprite = "colored_5";
                thing.fixedToGrid = true;
                thing.tileRule = new RandomTiles("colored_5", "colored_6", "colored_7");
                thing.floor = true;
                thing.pathTag = "ground";
                thing.buildOn = true;
                break;
            case TypeOfThing.Stream:
                thing.name = "stream";
                thing.sprite = "stream_4";
                thing.fixedToGrid = true;
                thing.pipe = true;
                thing.tileRule = new TileRuleDefinition(
                    "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
                    "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
                    "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
                    "stream_4", "stream_1", "stream_1!90");
                thing.group = 1;
                thing.positionalAudioGroup = "river";
                thing.pathTag = "blocking";
                break;
            case TypeOfThing.Path:
                thing.name = "path";
                thing.sprite = "path_4";
                thing.fixedToGrid = true;
                thing.pipe = true;
                thing.tileRule = new TileRuleDefinition(
                    "path_5!180", "path_5", "path_5!270", "path_5!90", 
                    "path_2!180", "path_2!90", "path_2", "path_2!270",
                    "path_3", "path_3!180", "path_3!90", "path_3!270",
                    "path_4", "path_1", "path_1!90");
                thing.group = 1;
                thing.floor = true;
                break;
            case TypeOfThing.Tree:
                thing.name = "tree";
                thing.sprite = "tree_1";
                thing.fixedToGrid = true;
                thing.tileRule = new RandomTiles("tree_1", "tree_2", "tree_3");
                thing.positionalAudioGroup = "trees";
                thing.floor = true;
                // thing.pathTag = "foliage";
                thing.resource = true;
                thing.produces = TypeOfThing.Wood;
                thing.hitpoints = 20;
                break;
            case TypeOfThing.BerryBush:
                thing.name = "apple tree";
                thing.sprite = "colored_68";
                thing.fixedToGrid = true;
                // thing.pathTag = "foliage";
                thing.floor = true;
                thing.edible = true;
                thing.resource = true;
                thing.hitpoints = 20;
                break;
            case TypeOfThing.Mushroom:
                thing.name = "mushroom";
                thing.sprite = "colored_71";
                thing.fixedToGrid = true;
                // thing.pathTag = "foliage";
                thing.floor = true;
                thing.edible = true;
                thing.resource = true;
                thing.hitpoints = 1;
                break;
            case TypeOfThing.Wood:
                thing.name = "Wood";
                thing.sprite = "colored_transparent_209";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.resource = true;
            break;
            case TypeOfThing.FallenWood:
                thing.name = "Wood";
                thing.sprite = "colored_70";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.resource = true;
                thing.produces = TypeOfThing.Wood;
                thing.hitpoints = 5;
                thing.fixedToGrid = true;
            break;
            case TypeOfThing.Rock:
                thing.name = "rock";
                thing.sprite = "stone_1";
                thing.fixedToGrid = true;
                // thing.pathTag = "foliage";
                thing.floor = true;
                thing.resource = true;
                thing.produces = TypeOfThing.Stone;
                thing.hitpoints = 20;
                break;
            case TypeOfThing.Stone:
                thing.name = "stone";
                thing.sprite = "colored_transparent_68";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.resource = true;
            break;
            case TypeOfThing.Clay:
                thing.name = "clay";
                thing.sprite = "colored_1";
                thing.fixedToGrid = true;
                thing.resource = true;
                thing.floor = true;
                thing.hitpoints = 10;
                break;
            case TypeOfThing.Ore:
                thing.name = "ore";
                thing.sprite = "colored_4";
                thing.fixedToGrid = true;
                thing.resource = true;
                thing.floor = true;
                thing.hitpoints = 5;
                break;
            case TypeOfThing.MudFloor:
                thing.name = "mud floor";
                thing.sprite = "colored_0";
                thing.fixedToGrid = true;
                thing.floor = true;
                thing.playerBuiltFloor = true;
                thing.buildOn = true;
            break;
            case TypeOfThing.MudFloorBlueprint:
                thing.name = "mud floor";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.MudFloor, ConstructionGroup.Floors, TypeOfThing.None);
                break;
            case TypeOfThing.WoodFloor:
                thing.name = "wood floor";
                thing.sprite = "colored_16";
                thing.fixedToGrid = true;
                thing.floor = true;
                thing.playerBuiltFloor = true;
                thing.buildOn = true;
            break;
            case TypeOfThing.WoodFloorBlueprint:
                thing.name = "Wood";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.WoodFloor, ConstructionGroup.Floors, TypeOfThing.Wood);
                break;
            case TypeOfThing.WoodWall:
                thing.name = "wood wall";
                thing.sprite = "colored_98";
                thing.fixedToGrid = true;
                thing.pipe = true;
                thing.pathTag = "blocking";
                thing.tileRule = new TileRuleDefinition(
                    "colored_98", "colored_98", "colored_98", "colored_98", 
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98");
            break;
            case TypeOfThing.WoodWallBlueprint:
                thing.name = "Wood";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.WoodWall, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.pipe = true;
                break;
            case TypeOfThing.ForagedWall:
                thing.name = "Foraged wall";
                thing.sprite = "colored_70";
                thing.fixedToGrid = true;
                thing.pipe = true;
                thing.pathTag = "blocking";
                // thing.tileRule = new TileRuleDefinition(
                //     "colored_98", "colored_98", "colored_98", "colored_98", 
                //     "colored_98", "colored_98", "colored_98", "colored_98",
                //     "colored_98", "colored_98", "colored_98", "colored_98",
                //     "colored_98", "colored_98", "colored_98");
            break;
            case TypeOfThing.ForagedWallBlueprint:
                thing.name = "Foraged Wall";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.ForagedWall, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.pipe = true;
                break;
            case TypeOfThing.Fence:
                thing.name = "fence";
                thing.sprite = "colored_98";
                thing.fixedToGrid = true;
                thing.pipe = true;
                thing.pathTag = "blocking";
                thing.tileRule = new TileRuleDefinition(
                    "colored_98", "colored_98", "colored_98", "colored_98", 
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98");
            break;
            case TypeOfThing.FenceBlueprint:
                thing.name = "fence";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.Fence, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.pipe = true;
                break;
            case TypeOfThing.StoneFloor:
                thing.name = "stone floor";
                thing.sprite = "colored_416";
                thing.fixedToGrid = true;
                thing.floor = true;
                thing.playerBuiltFloor = true;
                thing.buildOn = true;
            break;
            case TypeOfThing.StoneFloorBlueprint:
                thing.name = "Stone";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.StoneFloor, ConstructionGroup.Floors, TypeOfThing.Stone);
                break;
            case TypeOfThing.StoneWall:
                thing.name = "stone wall";
                thing.sprite = "colored_580";
                thing.fixedToGrid = true;
                thing.pathTag = "blocking";
                thing.pipe = true;
            break;
            case TypeOfThing.StoneWallBlueprint:
                thing.name = "Stone";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.StoneWall, ConstructionGroup.Walls, TypeOfThing.Stone);
                thing.pipe = true;
                break;

            /*
                Furniture
            */
            case TypeOfThing.DoorBlueprint:
                thing.name = "Door";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(
                    this, thing, 
                    TypeOfThing.Grass | TypeOfThing.WoodWall | TypeOfThing.StoneWall, 
                    TypeOfThing.Door, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            case TypeOfThing.Door:
                thing.name = "Door";
                thing.sprite = "colored_297";
                thing.fixedToGrid = true;
                thing.floor = true;
                break;

            case TypeOfThing.GateBlueprint:
                thing.name = "Gate";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(
                    this, thing, 
                    TypeOfThing.Grass | TypeOfThing.Fence, 
                    TypeOfThing.Gate, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            case TypeOfThing.Gate:
                thing.name = "Gate";
                thing.sprite = "colored_99";
                thing.fixedToGrid = true;
                thing.floor = true;
                break;

            case TypeOfThing.ForagedBedBlueprint:
                thing.name = "Foraged Bed";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.ForagedBed, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            
            case TypeOfThing.ForagedBed:
                thing.name = "Foraged Bed";
                thing.sprite = "colored_204";
                thing.fixedToGrid = true;
                thing.floor = true;
                break;

            case TypeOfThing.BedBlueprint:
                thing.name = "Bed";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.Bed, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            
            case TypeOfThing.Bed:
                thing.name = "Bed";
                thing.sprite = "colored_261";
                thing.fixedToGrid = true;
                thing.floor = true;
                break;

            case TypeOfThing.FamilyChestBlueprint:
                thing.name = "Chest";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.FamilyChest, ConstructionGroup.Furniture, TypeOfThing.Wood);
            break;
            case TypeOfThing.FamilyChest:
                thing.name = "Chest";
                thing.sprite = "colored_200";
                thing.familyChest = new FamilyChest(this, thing);
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.fixedToGrid = true;
                thing.assignToFamily = true;
            break;

              case TypeOfThing.ClayForgeBlueprint:
                thing.name = "Clay Forge";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.ClayForge, ConstructionGroup.Furniture, TypeOfThing.Clay);
            break;
            case TypeOfThing.ClayForge:
                thing.name = "Clay Forge";
                thing.sprite = "colored_257";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.fixedToGrid = true;
                thing.assignToFamily = true;
            break;

            case TypeOfThing.ChickenCoopBlueprint:
                thing.name = "Coop";
                thing.sprite = "colored_transparent_855";
                thing.floor = true;
                thing.sortingOrder = (int)SortingOrders.Blueprints;
                thing.construction = new Construction(this, thing, TypeOfThing.Grass, TypeOfThing.ChickenCoop, ConstructionGroup.Furniture, TypeOfThing.Wood);
            break;
            case TypeOfThing.ChickenCoop:
                thing.name = "Coop";
                thing.sprite = "colored_265";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.coop = new Coop(this, thing);
                thing.fixedToGrid = true;
                thing.assignToFamily = true;
            break;
            case TypeOfThing.Axe:
                thing.name = "axe";
                thing.sprite = "colored_transparent_937";
                thing.sortingOrder = (int)SortingOrders.Objects;
                break;

            /*
                Objects
            */
            case TypeOfThing.Villager:
                thing.name = "Villager";
                thing.sprite = "colored_transparent_24";
                thing.color = new Color(235/255f, 155/255f, 200/255f);
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.inventory = new Inventory(thing);
                thing.agent = new Villager(this, thing);
            break;
            case TypeOfThing.Hen:
                thing.name = "Chicken";
                thing.sprite = "colored_transparent_249";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.agent = new Animal(this, thing);
            break;
            case TypeOfThing.Rooster:
                thing.name = "Cockerl";
                thing.sprite = "colored_transparent_249";
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.color = Color.red;
                thing.agent = new Animal(this, thing);
            break;
            case TypeOfThing.Chick:
                thing.name = "Chick";
                thing.scale = Vector3.one * 0.5f;
                thing.sprite = "colored_transparent_248";
                thing.color = Color.yellow;
                thing.sortingOrder = (int)SortingOrders.Objects;
                thing.agent = new Animal(this, thing);
            break;

            /*
                Misc
            */



            default:
                throw new System.Exception(string.Format("Unable to create tile {0}", thingType.ToString()));
        }
                
        return thing;
    }

    public Thing CreateAndAddThing(TypeOfThing type, int x, int y)
    {
        return AddThing(Create(type, x, y));
    }


    /*
        Pathfinding
    */
    public int TagFromString(string tag)
    {
        return System.Array.IndexOf(AstarPath.GetTagNames(), tag);
    }

    public void UpdateAstarPath(Vector2Int position, string pathTag, bool walkable)
    {
        var graphupdate = new GraphUpdateObject
        {
            setTag = TagFromString(pathTag),
            bounds = new Bounds(new Vector3(position.x, position.y, 0), Vector3.one),
            modifyWalkability = true,
            setWalkability = walkable,
            modifyTag = true
        };

        AstarPath.UpdateGraphs(graphupdate);
    }

    /*
        Querying
    */
    
    public Thing FindChestForFamily(string lastname)
    {
        return Things
            .Where(t => t.type == TypeOfThing.FamilyChest && t.belongsToFamily == lastname)
            .FirstOrDefault();
    }
    
    public IEnumerable<Thing> FindCoopsForFamily(string lastname)
    {
        return Things.Where(t => t.type == TypeOfThing.ChickenCoop && t.belongsToFamily == lastname);
    }

    public Vector3 GetVillageExit()
    {
        return new Vector3(Mathf.FloorToInt(MapSize.x / 2), 0);
    }

    public void MoveToNight()
    {
        WorldTime.TimeSinceStart += (1 - WorldTime.NormalizedTimeOfDay) * WorldTime.SecondsInADay;
    }

    public void MoveToDay()
    {
        WorldTime.TimeSinceStart += 
            (1 - WorldTime.NormalizedTimeOfDay) * WorldTime.SecondsInADay +
            (WorldTime.SecondsInADay/2);
    }

    void Update()
    {
        _cursor.Update();
        _director.Update();

        WorldTime.Update();

        _zoneGraph.Update();

        for(var i = 0; i < Things.Count; i++) 
        {
            Things[i].Update();
        }

        foreach(var positionalAudio in _positionalAudio)
        {
            positionalAudio.Update();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            //FindObjectOfType<VillageManager>().CreateNewVillager();

            AddThing(Create(TypeOfThing.Villager, 
                Mathf.FloorToInt(MapSize.x / 2), 
                UnityEngine.Random.Range(0, MapSize.y)));
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            var random = Things[UnityEngine.Random.Range(0, Things.Count)];
            RemoveThing(random);
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            var toBuild = Things.Where(t => t.construction != null).ToArray();
            foreach(var thing in toBuild)
            {
                thing.construction.Construct();
                thing.Destroy();
            }
        }
    }
    
    #if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;

        _cursor.DrawGizmos();
        _zoneGraph.DrawGizmos();

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

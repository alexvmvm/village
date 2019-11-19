﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Village.Things;
using Village.AI;
using Village;
using System.Linq;
using Village.Things.Config;
using Village.Things.Serialization;

public class Assets 
{
    private static Dictionary<string, Sprite> _sprites;
    private static Dictionary<string, AudioClip> _audioClips;
    private static Dictionary<string, Material> _materials;
    private static Dictionary<string, GameObject> _prefabs;
    private static ThingSerialization _thingSerialization;
    private static ThingConfig[] _things;

    public static Sprite GetSprite(string name)
    {
        if(_sprites == null)
        {
            _sprites = new Dictionary<string, Sprite>();
            foreach(var sprite in Resources.LoadAll<Sprite>(""))
                _sprites[sprite.name] = sprite;
        }

        if(string.IsNullOrEmpty(name))
            throw new System.Exception($"Sprite name is null");

        var spriteName = name.Contains("!") ? name.Substring(0, name.IndexOf('!')) : name;
        if(!_sprites.ContainsKey(spriteName))
            _sprites[spriteName] = Resources.Load<Sprite>(spriteName);
            
        if(_sprites[spriteName] == null)
            throw new Exception($"Failed to find sprite: {spriteName}");

        return _sprites[spriteName];
    }

    public static Quaternion GetSpriteRotation(string name)
    {
        if(!name.Contains("!"))
            return Quaternion.identity;
        var rotation = int.Parse(name.Substring(name.IndexOf('!') + 1));
        return Quaternion.Euler(0, 0, rotation);
    }

    public static AudioClip GetAudioClip(string name)
    {
        if(_audioClips == null)
            _audioClips = new Dictionary<string, AudioClip>();

        if(string.IsNullOrEmpty(name))
            throw new System.Exception("AudioClip name is null");

        if(!_audioClips.ContainsKey(name))
            _audioClips[name] = Resources.Load<AudioClip>($"Music/{name}");

        return _audioClips[name];
    }

    public static GameObject GetPrefab(string name)
    {
        if(_prefabs == null)
            _prefabs = new Dictionary<string, GameObject>();

        if(string.IsNullOrEmpty(name))
            throw new System.Exception("GameObject name is null");

        if(!_prefabs.ContainsKey(name))
            _prefabs[name] = Resources.Load<GameObject>($"Prefabs/{name}");

        return _prefabs[name];
    }

     public static Material GetMaterial(string name)
    {
        if(_materials == null)
            _materials = new Dictionary<string, Material>();

        if(string.IsNullOrEmpty(name))
            throw new System.Exception("AudioClip name is null");

        if(!_materials.ContainsKey(name))
            _materials[name] = Resources.Load<Material>($"Materials/{name}");

        return _materials[name];
    }

    public static IEnumerable<ThingConfig> AllThingConfigs()
    {
         return Enum.GetValues(typeof(TypeOfThing))
                .Cast<TypeOfThing>()
                .Select(t => Assets.CreateThingConfig(t));
    }

    public static ThingConfig CreateThingConfig(TypeOfThing thingType)
    {
        if(_thingSerialization == null)
        {
            _thingSerialization = new ThingSerialization();
            _things = _thingSerialization.LoadFromFile($"{Application.dataPath}/Resources/Config/Things.xml");
        }

        var thing2 = _things.FirstOrDefault(t => t.TypeOfThing == thingType);

        if(thing2 == null && thingType != TypeOfThing.None)
        {
            throw new Exception($"unable to find {thingType}");
        }

        return thing2;

        var thing = new ThingConfig();
        thing.TypeOfThing = thingType;
        thing.Produces = thingType;

        switch(thingType)
        {
            case TypeOfThing.None:
                thing.Sprite = "colored_0";
            break;
            case TypeOfThing.Grass:
                thing.Name = "grass";
                thing.Sprite = "colored_5";
                thing.FixedToFloor = true;
                //thing.TileRule = new RandomTiles("colored_5", "colored_6", "colored_7");
                thing.TileRuleConfig = new TileRuleConfig
                {
                    Sprites = new string[] { "colored_5", "colored_6", "colored_7" },
                    Type = "RandomTiles"
                };
                thing.Floor = true;
                thing.BuildSite = true;
                break;
            case TypeOfThing.Stream:
                thing.Name = "stream";
                thing.Sprite = "stream_4";
                thing.FixedToFloor = true;
                thing.Pipe = true;
                // thing.TileRule = new TileRuleDefinition(
                //     "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
                //     "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
                //     "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
                //     "stream_4", "stream_1", "stream_1!90");
                thing.TileRuleConfig = new TileRuleConfig
                {
                    Sprites = new string[] { "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
                        "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
                        "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
                        "stream_4", "stream_1", "stream_1!90" },
                    Type = "TileRuleDefinition"
                };
                thing.GridGroup = 1;
                thing.PositionalAudioGroup = "river";
                thing.PathTag = Movement.TAG_BLOCKING;
                break;
            case TypeOfThing.Path:
                thing.Name = "path";
                thing.Sprite = "path_4";
                thing.FixedToFloor = true;
                thing.Pipe = true;
                // thing.TileRule = new TileRuleDefinition(
                //     "path_5!180", "path_5", "path_5!270", "path_5!90", 
                //     "path_2!180", "path_2!90", "path_2", "path_2!270",
                //     "path_3", "path_3!180", "path_3!90", "path_3!270",
                //     "path_4", "path_1", "path_1!90");
                thing.TileRuleConfig = new TileRuleConfig()
                {
                    Sprites = new string[] 
                    {
                        "path_5!180", "path_5", "path_5!270", "path_5!90", 
                        "path_2!180", "path_2!90", "path_2", "path_2!270",
                        "path_3", "path_3!180", "path_3!90", "path_3!270",
                        "path_4", "path_1", "path_1!90"
                    },
                    Type = "TileRuleDefinition"
                };
                thing.GridGroup = 2;
                thing.Floor = true;
                break;
            case TypeOfThing.Tree:
                thing.Name = "tree";
                thing.Sprite = "tree_1";
                thing.FixedToFloor = true;
                thing.TileRuleConfig = new TileRuleConfig()
                {
                    Sprites = new string[] { "tree_1", "tree_2", "tree_3" },
                    Type = "RandomTiles"
                };
                thing.PositionalAudioGroup = "trees";
                thing.Floor = true;
                thing.LightBlocking = true;
                thing.PathTag = Movement.TAG_AVOID;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Wood;
                thing.RequiredToProduce = TypeOfThing.Axe;
                thing.Hitpoints = 20;
                break;
            case TypeOfThing.BerryBush:
                thing.Name = "apple tree";
                thing.Sprite = "colored_68";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.Edible = true;
                thing.Resource = true;
                thing.Hitpoints = 20;
                break;
             case TypeOfThing.MushroomGrowing:
                thing.Name = "mushroom";
                thing.Sprite = "colored_71";
                thing.FixedToFloor = true;
                thing.Edible = true;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Mushroom;
                thing.Hitpoints = 1;
                break;
            case TypeOfThing.Mushroom:
                thing.Name = "mushroom";
                thing.Sprite = "colored_71";
                thing.Edible = true;
                thing.Resource = true;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Hitpoints = 1;
                break;
            case TypeOfThing.Wood:
                thing.Name = "Wood";
                thing.Sprite = "colored_transparent_209";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Resource = true;
                thing.Storeable = true;
                thing.StoreGroup = "resource";
                thing.Produces = TypeOfThing.Wood;
            break;
            case TypeOfThing.WoodenPlanks:
                thing.Name = "Planks";
                thing.Sprite = "colored_transparent_531";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Resource = true;
                thing.Storeable = true;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Wood };
                thing.StoreGroup = "resource";
            break;
            case TypeOfThing.FallenWood:
                thing.Name = "Wood";
                thing.Sprite = "colored_70";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Wood;
                thing.Hitpoints = 5;
                thing.FixedToFloor = true;
            break;
            case TypeOfThing.Rock:
                thing.Name = "rock";
                thing.Sprite = "stone_1";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Stone;
                thing.Hitpoints = 20;
                break;
            case TypeOfThing.Stone:
                thing.Name = "stone";
                thing.Sprite = "colored_transparent_68";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Resource = true;
            break;
            case TypeOfThing.ClayFloor:
                thing.Name = "clay";
                thing.Sprite = "colored_1";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.Hitpoints = 10;
                thing.Produces = TypeOfThing.Clay;
                break;
            case TypeOfThing.Clay:
                thing.Name = "clay";
                thing.Sprite = "colored_1";
                thing.Resource = true;
                thing.Hitpoints = 10;
                thing.SortingOrder = (int)SortingOrders.Objects;
                break;
            case TypeOfThing.OreFloor:
                thing.Name = "ore";
                thing.Sprite = "colored_4";
                thing.FixedToFloor = true;
                thing.Resource = true;
                thing.Floor = true;
                thing.Hitpoints = 5;
                thing.Produces = TypeOfThing.Ore;
                break;
            case TypeOfThing.Ore:
                thing.Name = "ore";
                thing.Sprite = "colored_4";
                thing.Resource = true;
                thing.Hitpoints = 5;
                thing.Storeable = true;
                thing.StoreGroup = "resource";
                thing.SortingOrder = (int)SortingOrders.Objects;
                break;
            case TypeOfThing.Iron:
                thing.Name = "iron";
                thing.Description = "A resource used to build tools and machinery.";
                thing.Sprite = "colored_transparent_721";
                thing.Resource = true;
                thing.Hitpoints = 1;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Ore };
                thing.Produces = TypeOfThing.Iron;
                thing.Storeable = true;
                break;
            case TypeOfThing.CabbageSeed:
                thing.Name = "cabbage seed";
                thing.Sprite = "colored_transparent_817";
                thing.Resource = true;
                thing.Hitpoints = 10;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Storeable = true;
                thing.StoreGroup = "seeds";
                break;
            case TypeOfThing.MudFloor:
                thing.Name = "mud floor";
                thing.Sprite = "colored_0";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.BuildSite = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Floors, 
                    Requires = TypeOfThing.None
                };
            break;
            case TypeOfThing.Blueprint:
                thing.Name = "blueprint";
                thing.Sprite = "colored_transparent_855";
                thing.BuildSite = false;
                thing.SortingOrder = 100;
                thing.PathTag = Movement.TAG_AVOID;
            break;
            case TypeOfThing.SoilFloor:
                thing.Name = "soil";
                thing.Sprite = "colored_1";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.BuildSite = true;
                thing.RequiredToProduce = TypeOfThing.Hoe;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Farming, 
                    Requires = TypeOfThing.Hoe
                };
            break;
            case TypeOfThing.CabbageCrop:
            {
                thing.Name = "cabbage";
                thing.Sprite = "colored_transparent_204";
                thing.BuildSite = false;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.CropConfig = new CropConfig()
                {
                    TimeToGrow = 5 * 60,
                    Sprites = new string[] { "colored_transparent_204", "colored_transparent_205", "colored_transparent_206"  },
                    Produces = TypeOfThing.Cabbage

                };
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    BuildOn = TypeOfThing.SoilFloor,
                    Group = ConstructionGroup.Floors, 
                    Requires = TypeOfThing.CabbageSeed
                };
            }
            break;
            case TypeOfThing.Cabbage:
            {
                thing.Name = "cabbage";
                thing.Sprite = "colored_transparent_977";
                thing.BuildSite = false;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Edible = true;
                thing.Storeable = true;
                thing.StoreGroup = "food";
                thing.Hitpoints = 1;
            }
            break;
            case TypeOfThing.WoodFloor:
                thing.Name = "wood floor";
                thing.Sprite = "colored_16";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.BuildSite = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Floors, 
                    Requires = TypeOfThing.WoodenPlanks
                };
            break;
            case TypeOfThing.WoodWall:
                thing.Name = "wood wall";
                thing.Sprite = "colored_98";
                thing.FixedToFloor = true;
                thing.Pipe = true;
                thing.PathTag = Movement.TAG_BLOCKING;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Walls, 
                    Requires = TypeOfThing.Wood
                };
            break;
            case TypeOfThing.ForagedWall:
                thing.Name = "Foraged wall";
                thing.Sprite = "colored_70";
                thing.FixedToFloor = true;
                thing.Pipe = true;
                thing.LightBlocking = true;
                thing.PathTag = Movement.TAG_BLOCKING;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Walls, 
                    Requires = TypeOfThing.Wood
                };
            break;
            // case TypeOfThing.Fence:
            //     thing.Name = "fence";
            //     thing.Sprite = "colored_98";
            //     thing.FixedToFloor = true;
            //     thing.Pipe = true;
            //     thing.PathTag = Movement.TAG_BLOCKING;
            //     thing.ConstructionConfig = new ConstructionConfig() 
            //     {
            //         Group = ConstructionGroup.Walls, 
            //         Requires = TypeOfThing.Wood
            //     };
            // break;
            case TypeOfThing.StoneFloor:
                thing.Name = "stone floor";
                thing.Sprite = "colored_416";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.BuildSite = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Floors, 
                    Requires = TypeOfThing.Stone
                };
            break;
            case TypeOfThing.StoneWall:
                thing.Name = "stone wall";
                thing.Sprite = "colored_580";
                thing.FixedToFloor = true;
                thing.PathTag = Movement.TAG_BLOCKING;
                thing.LightBlocking = true;
                thing.Pipe = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Walls, 
                    Requires = TypeOfThing.Stone
                };
            break;
            // case TypeOfThing.Door:
            //     thing.Name = "Door";
            //     thing.Sprite = "colored_297";
            //     thing.FixedToFloor = true;
            //     thing.Floor = true;
            //     thing.ConstructionConfig = new ConstructionConfig() 
            //     {
            //         Group = ConstructionGroup.Furniture, 
            //         Requires = TypeOfThing.WoodenPlanks
            //     };
            //     break;
            // case TypeOfThing.Bucket:
            //     thing.Name = "Bucket";
            //     thing.Sprite = "colored_transparent_974";
            //     thing.SortingOrder = (int)SortingOrders.Objects;
            //     thing.Floor = true;
            //     thing.ConstructionConfig = new ConstructionConfig() 
            //     {
            //         Group = ConstructionGroup.Furniture, 
            //         Requires = TypeOfThing.WoodenPlanks
            //     };
            //     break;
            case TypeOfThing.Table:
                thing.Name = "Table";
                thing.Sprite = "colored_transparent_302";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.PathTag = Movement.TAG_AVOID;
                thing.Floor = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.WoodenPlanks
                };
                break;
            case TypeOfThing.Chair:
                thing.Name = "Chair";
                thing.Sprite = "colored_transparent_298";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.PathTag = Movement.TAG_AVOID;
                thing.Floor = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.WoodenPlanks
                };
                break;
            case TypeOfThing.ForagedBed:
                thing.Name = "Foraged Bed";
                thing.Sprite = "colored_204";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
                break;
            case TypeOfThing.Bed:
                thing.Name = "Bed";
                thing.Sprite = "colored_261";
                thing.FixedToFloor = true;
                thing.Floor = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
                break;
            case TypeOfThing.ClayForge:
                thing.Name = "Clay Forge";
                thing.Sprite = "colored_680";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.FixedToFloor = true;
                thing.AssignToFamily = true;
                thing.FactoryConfig = new FactoryConfig()
                { 
                    Produces = new TypeOfThing[] { TypeOfThing.Iron }
                };
                thing.Fire = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Clay
                };
            break;
            case TypeOfThing.Workbench:
                thing.Name = "Workbench";
                thing.Sprite = "colored_228";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.FixedToFloor = true;
                thing.AssignToFamily = true;
                thing.FactoryConfig = new FactoryConfig()
                { 
                    Produces = new TypeOfThing[] { TypeOfThing.Axe, TypeOfThing.Hoe }
                };
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
            break;
            case TypeOfThing.CarpentersBench:
                thing.Name = "Carpenters Bench";
                thing.Sprite = "colored_228";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.FixedToFloor = true;
                thing.AssignToFamily = true;
                 thing.FactoryConfig = new FactoryConfig()
                { 
                    Produces = new TypeOfThing[] { TypeOfThing.WoodenPlanks }
                };
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
            break;
            case TypeOfThing.Axe:
                thing.Name = "axe";
                thing.Description = "A tool used to chop down trees.";
                thing.Sprite = "colored_transparent_937";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
                thing.Storeable = true;
                thing.Tool = true;
                thing.StoreGroup = "tool";
                break;
            case TypeOfThing.Hoe:
                thing.Name = "hoe";
                thing.Description = "A tool used to till land for planting crops.";
                thing.Sprite = "colored_transparent_770";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
                thing.Storeable = true;
                thing.Tool = true;
                thing.StoreGroup = "tool";
                break;
            case TypeOfThing.Storage:
                thing.Name = "storage";
                thing.Description = "A storage container used to store resources";
                thing.Sprite = "colored_200";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Storage = true;
                thing.FixedToFloor = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
                break;
            case TypeOfThing.Fire:
                thing.Name = "fire";
                thing.Description = "A fire to keep villagers warm";
                thing.Sprite = "colored_334";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Fire = true;
                thing.ConstructionConfig = new ConstructionConfig() 
                {
                    Group = ConstructionGroup.Furniture, 
                    Requires = TypeOfThing.Wood
                };
                break;
            /*
                Objects
            */
            case TypeOfThing.Villager:
                thing.Name = "Villager";
                thing.Sprite = "colored_transparent_24";
                thing.Color = new Color(235/255f, 155/255f, 200/255f);
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Inventory = true;
                thing.Agent = AgentConfig.Villager;
            break;
            case TypeOfThing.Hen:
                thing.Name = "Chicken";
                thing.Sprite = "colored_transparent_249";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Agent = AgentConfig.Animal;
            break;
            case TypeOfThing.Rooster:
                thing.Name = "Cockerl";
                thing.Sprite = "colored_transparent_249";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Color = Color.red;
                thing.Agent = AgentConfig.Animal;
            break;
            case TypeOfThing.Chick:
                thing.Name = "Chick";
                thing.Scale = Vector3.one * 0.5f;
                thing.Sprite = "colored_transparent_248";
                thing.Color = Color.yellow;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Agent = AgentConfig.Animal;
            break;
        }
        return thing;
    }

    public static Thing Create(TypeOfThing typeOfThing, int x, int y)
    {
        var id = Guid.NewGuid().ToString();
        var config = CreateThingConfig(typeOfThing);
        var gameObject = new GameObject($"{typeOfThing}_{id}");
        var thing = gameObject.AddComponent<Thing>();
        thing.transform.position = new Vector3(x, y);
        thing.id = id;
        thing.Setup(config);
        return thing;
    }
}

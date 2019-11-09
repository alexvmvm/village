using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Village.Things;
using Village.AI;
using Village;

public class Assets 
{
    private static Dictionary<string, Sprite> _sprites;
    private static Dictionary<string, AudioClip> _audioClips;
    private static Dictionary<string, Material> _materials;
    private static Dictionary<string, GameObject> _prefabs;

    public static Sprite GetSprite(string name)
    {
        if(_sprites == null)
        {
            _sprites = new Dictionary<string, Sprite>();
            foreach(var sprite in Resources.LoadAll<Sprite>(""))
                _sprites[sprite.name] = sprite;
        }

        if(string.IsNullOrEmpty(name))
            throw new System.Exception("Sprite name is null");

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

    public static Thing.ThingConfig CreateThingConfig(TypeOfThing thingType)
    {
        var thing = new Thing.ThingConfig();  
        switch(thingType)
        {
            case TypeOfThing.Grass:
                thing.Name = "grass";
                thing.Sprite = "colored_5";
                thing.FixedToGrid = true;
                thing.TileRule = new RandomTiles("colored_5", "colored_6", "colored_7");
                thing.Floor = true;
                thing.PathTag = "ground";
                thing.BuildSite = true;
                break;
            case TypeOfThing.Stream:
                thing.Name = "stream";
                thing.Sprite = "stream_4";
                thing.FixedToGrid = true;
                thing.Pipe = true;
                thing.TileRule = new TileRuleDefinition(
                    "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
                    "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
                    "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
                    "stream_4", "stream_1", "stream_1!90");
                thing.GridGroup = 1;
                thing.PositionalAudioGroup = "river";
                thing.PathTag = "blocking";
                break;
            case TypeOfThing.Path:
                thing.Name = "path";
                thing.Sprite = "path_4";
                thing.FixedToGrid = true;
                thing.Pipe = true;
                thing.TileRule = new TileRuleDefinition(
                    "path_5!180", "path_5", "path_5!270", "path_5!90", 
                    "path_2!180", "path_2!90", "path_2", "path_2!270",
                    "path_3", "path_3!180", "path_3!90", "path_3!270",
                    "path_4", "path_1", "path_1!90");
                thing.GridGroup = 1;
                thing.Floor = true;
                break;
            case TypeOfThing.Tree:
                thing.Name = "tree";
                thing.Sprite = "tree_1";
                thing.FixedToGrid = true;
                thing.TileRule = new RandomTiles("tree_1", "tree_2", "tree_3");
                thing.PositionalAudioGroup = "trees";
                thing.Floor = true;
                thing.LightBlocking = true;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Wood;
                thing.Hitpoints = 20;
                break;
            case TypeOfThing.BerryBush:
                thing.Name = "apple tree";
                thing.Sprite = "colored_68";
                thing.FixedToGrid = true;
                thing.Floor = true;
                thing.Edible = true;
                thing.Resource = true;
                thing.Hitpoints = 20;
                break;
             case TypeOfThing.MushroomGrowing:
                thing.Name = "mushroom";
                thing.Sprite = "colored_71";
                thing.FixedToGrid = true;
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
            break;
            case TypeOfThing.FallenWood:
                thing.Name = "Wood";
                thing.Sprite = "colored_70";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Resource = true;
                thing.Produces = TypeOfThing.Wood;
                thing.Hitpoints = 5;
                thing.FixedToGrid = true;
            break;
            case TypeOfThing.Rock:
                thing.Name = "rock";
                thing.Sprite = "stone_1";
                thing.FixedToGrid = true;
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
            case TypeOfThing.Clay:
                thing.Name = "clay";
                thing.Sprite = "colored_1";
                thing.FixedToGrid = true;
                thing.Resource = true;
                thing.Floor = true;
                thing.Hitpoints = 10;
                break;
            case TypeOfThing.Ore:
                thing.Name = "ore";
                thing.Sprite = "colored_4";
                thing.FixedToGrid = true;
                thing.Resource = true;
                thing.Floor = true;
                thing.Hitpoints = 5;
                thing.Storeable = true;
                thing.StoreGroup = "resource";
                break;
            case TypeOfThing.Iron:
                thing.Name = "iron";
                thing.Description = "A resource used to build tools and machinery.";
                thing.Sprite = "colored_transparent_721";
                thing.Resource = true;
                thing.Hitpoints = 1;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Ore };
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
                thing.FixedToGrid = true;
                thing.Floor = true;
                thing.BuildSite = true;
            break;
            case TypeOfThing.MudFloorBlueprint:
                thing.Name = "mud floor";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.MudFloor, ConstructionGroup.Floors, TypeOfThing.None);
                break;
            case TypeOfThing.SoilFloor:
                thing.Name = "soil";
                thing.Sprite = "colored_1";
                thing.FixedToGrid = true;
                thing.Floor = true;
                thing.BuildSite = true;
            break;
            case TypeOfThing.SoilFloorBlueprint:
                thing.Name = "soil";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.SoilFloor, ConstructionGroup.Farming, TypeOfThing.Hoe);
                break;
            case TypeOfThing.CabbageCrop:
            {
                thing.Name = "cabbage";
                thing.Sprite = "colored_transparent_204";
                thing.BuildSite = false;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Crop = new Thing.CropConfig(360, new string[] { "colored_transparent_204", "colored_transparent_205", "colored_transparent_206" });
            }
            break;
            case TypeOfThing.CabbageCropBlueprint:
                thing.Name = "cabbage";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(TypeOfThing.SoilFloor, TypeOfThing.CabbageCrop, ConstructionGroup.Farming, TypeOfThing.CabbageSeed);
                break;
            case TypeOfThing.WoodFloor:
                thing.Name = "wood floor";
                thing.Sprite = "colored_16";
                thing.FixedToGrid = true;
                thing.Floor = true;
                thing.BuildSite = true;
            break;
            case TypeOfThing.WoodFloorBlueprint:
                thing.Name = "Wood";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.WoodFloor, ConstructionGroup.Floors, TypeOfThing.Wood);
                break;
            case TypeOfThing.WoodWall:
                thing.Name = "wood wall";
                thing.Sprite = "colored_98";
                thing.FixedToGrid = true;
                thing.Pipe = true;
                thing.PathTag = "blocking";
                thing.TileRule = new TileRuleDefinition(
                    "colored_98", "colored_98", "colored_98", "colored_98", 
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98");
            break;
            case TypeOfThing.WoodWallBlueprint:
                thing.Name = "Wood";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.WoodWall, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.Pipe = true;
                break;
            case TypeOfThing.ForagedWall:
                thing.Name = "Foraged wall";
                thing.Sprite = "colored_70";
                thing.FixedToGrid = true;
                thing.Pipe = true;
                thing.LightBlocking = true;
                thing.PathTag = "blocking";
                thing.PathBlocking = true;
            break;
            case TypeOfThing.ForagedWallBlueprint:
                thing.Name = "Foraged Wall";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.ForagedWall, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.Pipe = true;
                break;
            case TypeOfThing.Fence:
                thing.Name = "fence";
                thing.Sprite = "colored_98";
                thing.FixedToGrid = true;
                thing.Pipe = true;
                thing.PathTag = "blocking";
                thing.TileRule = new TileRuleDefinition(
                    "colored_98", "colored_98", "colored_98", "colored_98", 
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98", "colored_98",
                    "colored_98", "colored_98", "colored_98");
            break;
            case TypeOfThing.FenceBlueprint:
                thing.Name = "fence";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Fence, ConstructionGroup.Walls, TypeOfThing.Wood);
                thing.Pipe = true;
                break;
            case TypeOfThing.StoneFloor:
                thing.Name = "stone floor";
                thing.Sprite = "colored_416";
                thing.FixedToGrid = true;
                thing.Floor = true;
                thing.BuildSite = true;
            break;
            case TypeOfThing.StoneFloorBlueprint:
                thing.Name = "Stone";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.StoneFloor, ConstructionGroup.Floors, TypeOfThing.Stone);
                break;
            case TypeOfThing.StoneWall:
                thing.Name = "stone wall";
                thing.Sprite = "colored_580";
                thing.FixedToGrid = true;
                thing.PathTag = "blocking";
                thing.LightBlocking = true;
                thing.Pipe = true;
                thing.PathBlocking = true;
            break;
            case TypeOfThing.StoneWallBlueprint:
                thing.Name = "Stone";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.StoneWall, ConstructionGroup.Walls, TypeOfThing.Stone);
                thing.Pipe = true;
                break;

            /*
                Furniture
            */
            case TypeOfThing.DoorBlueprint:
                thing.Name = "Door";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Door, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            case TypeOfThing.Door:
                thing.Name = "Door";
                thing.Sprite = "colored_297";
                thing.FixedToGrid = true;
                thing.Floor = true;
                break;
            case TypeOfThing.ForagedBedBlueprint:
                thing.Name = "Foraged Bed";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.ForagedBed, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            
            case TypeOfThing.ForagedBed:
                thing.Name = "Foraged Bed";
                thing.Sprite = "colored_204";
                thing.FixedToGrid = true;
                thing.Floor = true;
                break;

            case TypeOfThing.BedBlueprint:
                thing.Name = "Bed";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Bed, ConstructionGroup.Furniture, TypeOfThing.Wood);
                break;
            
            case TypeOfThing.Bed:
                thing.Name = "Bed";
                thing.Sprite = "colored_261";
                thing.FixedToGrid = true;
                thing.Floor = true;
                break;
              case TypeOfThing.ClayForgeBlueprint:
                thing.Name = "Clay Forge";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.ClayForge, ConstructionGroup.Furniture, TypeOfThing.Clay);
            break;
            case TypeOfThing.ClayForge:
                thing.Name = "Clay Forge";
                thing.Sprite = "colored_680";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.FixedToGrid = true;
                thing.AssignToFamily = true;
                thing.Factory = new Thing.FactoryConfig(new TypeOfThing[] { TypeOfThing.Iron });
                thing.Fire = true;
            break;

            case TypeOfThing.WorkbenchBlueprint:
                thing.Name = "Workbench";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Workbench, ConstructionGroup.Furniture, TypeOfThing.Wood);
            break;
            case TypeOfThing.Workbench:
                thing.Name = "Workbench";
                thing.Sprite = "colored_228";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.FixedToGrid = true;
                thing.AssignToFamily = true;
                thing.Factory = new Thing.FactoryConfig(new TypeOfThing[] { TypeOfThing.Axe, TypeOfThing.Hoe });
            break;
            case TypeOfThing.Axe:
                thing.Name = "axe";
                thing.Description = "A tool used to chop down trees.";
                thing.Sprite = "colored_transparent_937";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
                thing.Storeable = true;
                thing.StoreGroup = "tool";
                break;
            case TypeOfThing.Hoe:
                thing.Name = "hoe";
                thing.Description = "A tool used to till land for planting crops.";
                thing.Sprite = "colored_transparent_770";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.RequiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
                thing.Storeable = true;
                thing.StoreGroup = "tool";
                break;
            case TypeOfThing.Storage:
                thing.Name = "storage";
                thing.Description = "A storage container used to store resources";
                thing.Sprite = "colored_200";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Storage = true;
                thing.FixedToGrid = true;
                break;
            case TypeOfThing.StorageBlueprint:
                thing.Name = "storage";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Storage, ConstructionGroup.Furniture, TypeOfThing.Wood);
            break;
            case TypeOfThing.Fire:
                thing.Name = "fire";
                thing.Description = "A fire to keep villagers warm";
                thing.Sprite = "colored_334";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Fire = true;
                break;
            case TypeOfThing.FireBlueprint:
                thing.Name = "fire";
                thing.Sprite = "colored_transparent_855";
                thing.Floor = true;
                thing.SortingOrder = (int)SortingOrders.Blueprints;
                thing.Construction = new Thing.ConstructionConfig(null, TypeOfThing.Fire, ConstructionGroup.Furniture, TypeOfThing.Wood);
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
                thing.Agent = Thing.AgentConfig.Villager;
            break;
            case TypeOfThing.Hen:
                thing.Name = "Chicken";
                thing.Sprite = "colored_transparent_249";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Agent = Thing.AgentConfig.Animal;
            break;
            case TypeOfThing.Rooster:
                thing.Name = "Cockerl";
                thing.Sprite = "colored_transparent_249";
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Color = Color.red;
                thing.Agent = Thing.AgentConfig.Animal;
            break;
            case TypeOfThing.Chick:
                thing.Name = "Chick";
                thing.Scale = Vector3.one * 0.5f;
                thing.Sprite = "colored_transparent_248";
                thing.Color = Color.yellow;
                thing.SortingOrder = (int)SortingOrders.Objects;
                thing.Agent = Thing.AgentConfig.Animal;
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

    // public static Thing Create(Game game, TypeOfThing thingType, int x, int y)
    // {
    //     var thing = new Thing(thingType, game);
    //     thing.transform.position = new Vector3(x, y, 0);

    //     switch(thingType)
    //     {
    //         case TypeOfThing.Grass:
    //             thing.name = "grass";
    //             thing.sprite = "colored_5";
    //             thing.fixedToGrid = true;
    //             thing.tileRule = new RandomTiles("colored_5", "colored_6", "colored_7");
    //             thing.floor = true;
    //             thing.pathTag = "ground";
    //             thing.buildOn = true;
    //             break;
    //         case TypeOfThing.Stream:
    //             thing.name = "stream";
    //             thing.sprite = "stream_4";
    //             thing.fixedToGrid = true;
    //             thing.pipe = true;
    //             thing.tileRule = new TileRuleDefinition(
    //                 "stream_5!180", "stream_5", "stream_5!270", "stream_5!90", 
    //                 "stream_2!180", "stream_2!90", "stream_2", "stream_2!270",
    //                 "stream_3", "stream_3!180", "stream_3!90", "stream_3!270",
    //                 "stream_4", "stream_1", "stream_1!90");
    //             thing.positionalGroup = 1;
    //             thing.positionalAudioGroup = "river";
    //             thing.pathTag = "blocking";
    //             break;
    //         case TypeOfThing.Path:
    //             thing.name = "path";
    //             thing.sprite = "path_4";
    //             thing.fixedToGrid = true;
    //             thing.pipe = true;
    //             thing.tileRule = new TileRuleDefinition(
    //                 "path_5!180", "path_5", "path_5!270", "path_5!90", 
    //                 "path_2!180", "path_2!90", "path_2", "path_2!270",
    //                 "path_3", "path_3!180", "path_3!90", "path_3!270",
    //                 "path_4", "path_1", "path_1!90");
    //             thing.positionalGroup = 1;
    //             thing.floor = true;
    //             break;
    //         case TypeOfThing.Tree:
    //             thing.name = "tree";
    //             thing.sprite = "tree_1";
    //             thing.fixedToGrid = true;
    //             thing.tileRule = new RandomTiles("tree_1", "tree_2", "tree_3");
    //             thing.positionalAudioGroup = "trees";
    //             thing.floor = true;
    //             thing.blocksLight = true;
    //             // thing.pathTag = "foliage";
    //             thing.resource = true;
    //             thing.produces = TypeOfThing.Wood;
    //             thing.Hitpoints = 20;
    //             break;
    //         case TypeOfThing.BerryBush:
    //             thing.name = "apple tree";
    //             thing.sprite = "colored_68";
    //             thing.fixedToGrid = true;
    //             // thing.pathTag = "foliage";
    //             thing.floor = true;
    //             thing.edible = true;
    //             thing.resource = true;
    //             thing.Hitpoints = 20;
    //             break;
    //          case TypeOfThing.MushroomGrowing:
    //             thing.name = "mushroom";
    //             thing.sprite = "colored_71";
    //             thing.fixedToGrid = true;
    //             thing.edible = true;
    //             thing.resource = true;
    //             thing.produces = TypeOfThing.Mushroom;
    //             thing.Hitpoints = 1;
    //             break;
    //         case TypeOfThing.Mushroom:
    //             thing.name = "mushroom";
    //             thing.sprite = "colored_71";
    //             thing.edible = true;
    //             thing.resource = true;
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.Hitpoints = 1;
    //             break;
    //         case TypeOfThing.Wood:
    //             thing.name = "Wood";
    //             thing.sprite = "colored_transparent_209";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.resource = true;
    //         break;
    //         case TypeOfThing.FallenWood:
    //             thing.name = "Wood";
    //             thing.sprite = "colored_70";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.resource = true;
    //             thing.produces = TypeOfThing.Wood;
    //             thing.Hitpoints = 5;
    //             thing.fixedToGrid = true;
    //         break;
    //         case TypeOfThing.Rock:
    //             thing.name = "rock";
    //             thing.sprite = "stone_1";
    //             thing.fixedToGrid = true;
    //             // thing.pathTag = "foliage";
    //             thing.floor = true;
    //             thing.resource = true;
    //             thing.produces = TypeOfThing.Stone;
    //             thing.Hitpoints = 20;
    //             break;
    //         case TypeOfThing.Stone:
    //             thing.name = "stone";
    //             thing.sprite = "colored_transparent_68";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.resource = true;
    //         break;
    //         case TypeOfThing.Clay:
    //             thing.name = "clay";
    //             thing.sprite = "colored_1";
    //             thing.fixedToGrid = true;
    //             thing.resource = true;
    //             thing.floor = true;
    //             thing.Hitpoints = 10;
    //             break;
    //         case TypeOfThing.Ore:
    //             thing.name = "ore";
    //             thing.sprite = "colored_4";
    //             thing.fixedToGrid = true;
    //             thing.resource = true;
    //             thing.floor = true;
    //             thing.Hitpoints = 5;
    //             thing.storeable = true;
    //             thing.storeGroup = "resource";
    //             break;
    //         case TypeOfThing.Iron:
    //             thing.name = "iron";
    //             thing.description = "A resource used to build tools and machinery.";
    //             thing.sprite = "colored_transparent_721";
    //             thing.resource = true;
    //             thing.Hitpoints = 1;
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.requiredToCraft = new TypeOfThing[] { TypeOfThing.Ore };
    //             break;
    //         case TypeOfThing.CabbageSeed:
    //             thing.name = "cabbage seed";
    //             thing.sprite = "colored_transparent_817";
    //             thing.resource = true;
    //             thing.Hitpoints = 10;
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.storeable = true;
    //             thing.storeGroup = "seeds";
    //             break;
    //         case TypeOfThing.MudFloor:
    //             thing.name = "mud floor";
    //             thing.sprite = "colored_0";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             thing.playerBuiltFloor = true;
    //             thing.buildOn = true;
    //         break;
    //         case TypeOfThing.MudFloorBlueprint:
    //             thing.name = "mud floor";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.MudFloor, ConstructionGroup.Floors, TypeOfThing.None);
    //             break;
    //         case TypeOfThing.SoilFloor:
    //             thing.name = "soil";
    //             thing.sprite = "colored_1";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             thing.playerBuiltFloor = true;
    //             thing.buildOn = true;
    //         break;
    //         case TypeOfThing.SoilFloorBlueprint:
    //             thing.name = "soil";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.SoilFloor, ConstructionGroup.Farming, TypeOfThing.Hoe);
    //             break;
    //         case TypeOfThing.CabbageCrop:
    //         {
    //             var crop = new Crop(game, thing, 360, new string[] { "colored_transparent_204", "colored_transparent_205", "colored_transparent_206" });
    //             thing.name = "cabbage";
    //             thing.sprite = "colored_transparent_204";
    //             thing.playerBuiltFloor = true;
    //             thing.buildOn = false;
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(crop);
    //             thing.tileRule = new CropTile(thing, crop);
    //         }
    //         break;
    //         case TypeOfThing.CabbageCropBlueprint:
    //             thing.name = "cabbage";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, TypeOfThing.SoilFloor, TypeOfThing.CabbageCrop, ConstructionGroup.Farming, TypeOfThing.CabbageSeed);
    //             break;
    //         case TypeOfThing.WoodFloor:
    //             thing.name = "wood floor";
    //             thing.sprite = "colored_16";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             thing.playerBuiltFloor = true;
    //             thing.buildOn = true;
    //         break;
    //         case TypeOfThing.WoodFloorBlueprint:
    //             thing.name = "Wood";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.WoodFloor, ConstructionGroup.Floors, TypeOfThing.Wood);
    //             break;
    //         case TypeOfThing.WoodWall:
    //             thing.name = "wood wall";
    //             thing.sprite = "colored_98";
    //             thing.fixedToGrid = true;
    //             thing.pipe = true;
    //             thing.pathTag = "blocking";
    //             thing.tileRule = new TileRuleDefinition(
    //                 "colored_98", "colored_98", "colored_98", "colored_98", 
    //                 "colored_98", "colored_98", "colored_98", "colored_98",
    //                 "colored_98", "colored_98", "colored_98", "colored_98",
    //                 "colored_98", "colored_98", "colored_98");
    //         break;
    //         case TypeOfThing.WoodWallBlueprint:
    //             thing.name = "Wood";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.WoodWall, ConstructionGroup.Walls, TypeOfThing.Wood);
    //             thing.pipe = true;
    //             break;
    //         case TypeOfThing.ForagedWall:
    //             thing.name = "Foraged wall";
    //             thing.sprite = "colored_70";
    //             thing.fixedToGrid = true;
    //             thing.pipe = true;
    //             thing.blocksLight = true;
    //             thing.pathTag = "blocking";
    //             thing.blocksPath = true;
    //         break;
    //         case TypeOfThing.ForagedWallBlueprint:
    //             thing.name = "Foraged Wall";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.ForagedWall, ConstructionGroup.Walls, TypeOfThing.Wood);
    //             thing.pipe = true;
    //             break;
    //         case TypeOfThing.Fence:
    //             thing.name = "fence";
    //             thing.sprite = "colored_98";
    //             thing.fixedToGrid = true;
    //             thing.pipe = true;
    //             thing.pathTag = "blocking";
    //             thing.tileRule = new TileRuleDefinition(
    //                 "colored_98", "colored_98", "colored_98", "colored_98", 
    //                 "colored_98", "colored_98", "colored_98", "colored_98",
    //                 "colored_98", "colored_98", "colored_98", "colored_98",
    //                 "colored_98", "colored_98", "colored_98");
    //         break;
    //         case TypeOfThing.FenceBlueprint:
    //             thing.name = "fence";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.Fence, ConstructionGroup.Walls, TypeOfThing.Wood);
    //             thing.pipe = true;
    //             break;
    //         case TypeOfThing.StoneFloor:
    //             thing.name = "stone floor";
    //             thing.sprite = "colored_416";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             thing.playerBuiltFloor = true;
    //             thing.buildOn = true;
    //         break;
    //         case TypeOfThing.StoneFloorBlueprint:
    //             thing.name = "Stone";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.StoneFloor, ConstructionGroup.Floors, TypeOfThing.Stone);
    //             break;
    //         case TypeOfThing.StoneWall:
    //             thing.name = "stone wall";
    //             thing.sprite = "colored_580";
    //             thing.fixedToGrid = true;
    //             thing.pathTag = "blocking";
    //             thing.blocksLight = true;
    //             thing.pipe = true;
    //             thing.blocksPath = true;
    //         break;
    //         case TypeOfThing.StoneWallBlueprint:
    //             thing.name = "Stone";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.StoneWall, ConstructionGroup.Walls, TypeOfThing.Stone);
    //             thing.pipe = true;
    //             break;

    //         /*
    //             Furniture
    //         */
    //         case TypeOfThing.DoorBlueprint:
    //             thing.name = "Door";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(
    //                 game, thing, 
    //                 null, 
    //                 TypeOfThing.Door, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //             break;
    //         case TypeOfThing.Door:
    //             thing.name = "Door";
    //             thing.sprite = "colored_297";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             break;
    //         case TypeOfThing.ForagedBedBlueprint:
    //             thing.name = "Foraged Bed";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.ForagedBed, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //             break;
            
    //         case TypeOfThing.ForagedBed:
    //             thing.name = "Foraged Bed";
    //             thing.sprite = "colored_204";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             break;

    //         case TypeOfThing.BedBlueprint:
    //             thing.name = "Bed";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.Bed, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //             break;
            
    //         case TypeOfThing.Bed:
    //             thing.name = "Bed";
    //             thing.sprite = "colored_261";
    //             thing.fixedToGrid = true;
    //             thing.floor = true;
    //             break;
    //           case TypeOfThing.ClayForgeBlueprint:
    //             thing.name = "Clay Forge";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.ClayForge, ConstructionGroup.Furniture, TypeOfThing.Clay);
    //         break;
    //         case TypeOfThing.ClayForge:
    //             thing.name = "Clay Forge";
    //             thing.sprite = "colored_680";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.fixedToGrid = true;
    //             thing.assignToFamily = true;
    //             thing.AddTrait(new Factory(game, thing, new TypeOfThing[] { TypeOfThing.Iron }));
    //             thing.AddTrait(new Fire(game, thing, true));
    //         break;

    //         case TypeOfThing.WorkbenchBlueprint:
    //             thing.name = "Workbench";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.Workbench, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //         break;
    //         case TypeOfThing.Workbench:
    //             thing.name = "Workbench";
    //             thing.sprite = "colored_228";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.fixedToGrid = true;
    //             thing.assignToFamily = true;
    //             thing.AddTrait(new Factory(game, thing, new TypeOfThing[] { TypeOfThing.Axe, TypeOfThing.Hoe }));
    //         break;
    //         case TypeOfThing.Axe:
    //             thing.name = "axe";
    //             thing.description = "A tool used to chop down trees.";
    //             thing.sprite = "colored_transparent_937";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.requiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
    //             thing.storeable = true;
    //             thing.storeGroup = "tool";
    //             break;
    //         case TypeOfThing.Hoe:
    //             thing.name = "hoe";
    //             thing.description = "A tool used to till land for planting crops.";
    //             thing.sprite = "colored_transparent_770";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.requiredToCraft = new TypeOfThing[] { TypeOfThing.Iron };
    //             thing.storeable = true;
    //             thing.storeGroup = "tool";
    //             break;
    //         case TypeOfThing.Storage:
    //             thing.name = "storage";
    //             thing.description = "A storage container used to store resources";
    //             thing.sprite = "colored_200";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(new Storage(game, thing));
    //             thing.fixedToGrid = true;
    //             break;
    //         case TypeOfThing.StorageBlueprint:
    //             thing.name = "storage";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.Storage, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //         break;
    //         case TypeOfThing.Fire:
    //             thing.name = "fire";
    //             thing.description = "A fire to keep villagers warm";
    //             thing.sprite = "colored_334";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(new Fire(game, thing));
    //             break;
    //         case TypeOfThing.FireBlueprint:
    //             thing.name = "fire";
    //             thing.sprite = "colored_transparent_855";
    //             thing.floor = true;
    //             thing.sortingOrder = (int)SortingOrders.Blueprints;
    //             thing.construction = new Construction(game, thing, null, TypeOfThing.Fire, ConstructionGroup.Furniture, TypeOfThing.Wood);
    //         break;

    //         /*
    //             Objects
    //         */
    //         case TypeOfThing.Villager:
    //             thing.name = "Villager";
    //             thing.sprite = "colored_transparent_24";
    //             thing.color = new Color(235/255f, 155/255f, 200/255f);
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(new Inventory(thing));
    //             thing.AddTrait(new Villager(game, thing));
    //         break;
    //         case TypeOfThing.Hen:
    //             thing.name = "Chicken";
    //             thing.sprite = "colored_transparent_249";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(new Animal(game, thing));
    //         break;
    //         case TypeOfThing.Rooster:
    //             thing.name = "Cockerl";
    //             thing.sprite = "colored_transparent_249";
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.color = Color.red;
    //             thing.AddTrait(new Animal(game, thing));
    //         break;
    //         case TypeOfThing.Chick:
    //             thing.name = "Chick";
    //             thing.scale = Vector3.one * 0.5f;
    //             thing.sprite = "colored_transparent_248";
    //             thing.color = Color.yellow;
    //             thing.sortingOrder = (int)SortingOrders.Objects;
    //             thing.AddTrait(new Animal(game, thing));
    //         break;

    //         /*
    //             Misc
    //         */



    //         default:
    //             throw new System.Exception(string.Format("Unable to create tile {0}", thingType.ToString()));
    //     }
                
    //     return thing;
    // }
}

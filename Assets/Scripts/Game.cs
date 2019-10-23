using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public delegate void ThingAdded(Thing thing);
public delegate void ThingRemoved(Thing thing);

public class Game : MonoBehaviour
{
    public ZoneGraph ZoneGraph { get { return _zoneGraph; } }
    public Vector2Int MapSize = Vector2Int.one * 10;
    public AstarPath AstarPath;
    public Thing[,] Grid;
    public List<Thing> Things;
    public List<Thing> AllThings;
    public TypeOfThing? CurrentType;
    public WorldTime WorldTime;
    public ThingAdded OnThingAdded;
    public ThingRemoved OnThingRemoved;
    private GameCursor _cursor;
    private List<PositionalAudio> _positionalAudio;
    private Director _director;
    private ZoneGraph _zoneGraph;

    void Awake()
    {
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
            if(thing.GetTrait<Agent>() != null)
                thing.GetTrait<Agent>().PauseAgent();
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
        RandomMap();

        _zoneGraph.Start();
    }

    public void RandomMap()
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
                    CreateAndAddThing(TypeOfThing.BerryBush, x, y);
                }
                else if(noise > 0.65f)
                {
                    CreateAndAddThing(TypeOfThing.Tree, x, y);
                }
                else if(noise2 > 0.8f)
                {
                    CreateAndAddThing(TypeOfThing.Rock, x, y);
                }
                else if(noise3 > 0.85f)
                {
                    CreateAndAddThing(TypeOfThing.Mushroom, x, y);
                }
                else
                {
                    CreateAndAddThing(TypeOfThing.Grass, x, y); 
                }
                
            }
        }

        for(var y = 0; y < MapSize.y; y++)
        {
            var x = Mathf.FloorToInt(MapSize.x / 2);
            CreateAndAddThing(TypeOfThing.Path, x, y);
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

        for(var i = 0; i < 4; i++) 
        {
            var x = UnityEngine.Random.Range(0, MapSize.x);
            var y = UnityEngine.Random.Range(0, MapSize.y);

            CreateAndAddThing(TypeOfThing.Hen, x, y);
        }

        for(var i = 0; i < 10; i++) 
        {
            var x = UnityEngine.Random.Range(0, MapSize.x);
            var y = UnityEngine.Random.Range(0, MapSize.y);

            CreateAndAddThing(TypeOfThing.Chick, x, y);
        }

        for(var i = 0; i < 1; i++) 
        {
            var x = UnityEngine.Random.Range(0, MapSize.x);
            var y = UnityEngine.Random.Range(0, MapSize.y);

            CreateAndAddThing(TypeOfThing.Rooster, x, y);
        }

    }

    public GameObject InstantiateObj()
    {
        var obj = Instantiate(Assets.GetPrefab("Thing"));
        obj.transform.SetParent(transform);
        obj.SetActive(true);
        return obj;
    }

    /*
        Sprites
    */
    
    public Thing GetThingNotInScene(TypeOfThing type)
    {
        return AllThings.Where(t => t.type == type).FirstOrDefault();
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
        var obj = InstantiateObj();
        obj.transform.position = new Vector3(x, y, 0);
        obj.transform.name = thingType.ToString();
         
        return Assets.Create(this, obj, thingType, x, y);
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

    public void Clear()
    {
        foreach(var thing in Things.ToArray())
        {
            RemoveThing(thing);
        }
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

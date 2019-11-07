using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Village.Things;
using Village.AI;
using Village.Saving;

namespace Village
{
    public delegate void ThingAdded(Thing thing);
    public delegate void ThingRemoved(Thing thing);

    public class Game : ISave<GameSave>
    {
        public Vector2Int Size { get { return _size; } }
        public WorldTime WorldTime { get { return _worldTime; } }
        private List<Thing> _all;
        public ThingAdded OnThingAdded;
        public ThingRemoved OnThingRemoved;
        private List<PositionalAudio> _positionalAudio;
        private Director _director;
        private Thing[,] _grid;
        private List<Thing> _things;
        private WorldTime _worldTime;
        private Vector2Int _size = Vector2Int.one * 10;
        private AstarPath _aStarPath;
        private RegionManager _regionManager;

        public Game(AstarPath aStarPath, Vector2Int size)
        {
            _aStarPath = aStarPath;
            _size = size;
            _things = new List<Thing>();
            _grid = new Thing[_size.x, _size.y];
            _worldTime = new WorldTime(360, 5, 23);
            _director = new Director(this);
            _positionalAudio = new List<PositionalAudio>()
            {
                new PositionalAudio(this, "river", "running_water"),
                new PositionalAudio(this, "trees", "birds")
            };

            // setup all things
            _all = new List<Thing>();

            _regionManager = new RegionManager(this);

        }

        public void Start()
        {
            _all.Clear();
            
            foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
            {
                if(thingType == TypeOfThing.None)
                    continue;
                var thing = Create(thingType, -10, -10);
                if(thing.HasTrait<Agent>())
                    thing.GetTrait<Agent>().PauseAgent();
                _all.Add(thing);
            }
        }

        public void Generate()
        {
            for(var x = 0; x < _size.x; x++)
            {
                for(var y = 0; y < _size.y; y++)
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
                        CreateAndAddThing(TypeOfThing.MushroomGrowing, x, y);
                    }
                    else
                    {
                        CreateAndAddThing(TypeOfThing.Grass, x, y); 
                    }
                    
                }
            }

            for(var y = 0; y < _size.y; y++)
            {
                var x = Mathf.FloorToInt(_size.x / 2);
                CreateAndAddThing(TypeOfThing.Path, x, y);
            }
        
            var rand = 0;
            for(var y = 0; y < _size.y; y++)
            {
                var x = _size.x - 3;
                rand = UnityEngine.Random.Range(0, 4);
                AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x - 1, y));
                AddThing(Create(TypeOfThing.Stream, x, y));
                rand = UnityEngine.Random.Range(0, 2);
                AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x + 1, y));
            }

            for(var i = 0; i < 20; i++)
            {
                var x = UnityEngine.Random.Range(0, _size.x);
                var y = UnityEngine.Random.Range(0, _size.y);

                CreateAndAddThing(TypeOfThing.FallenWood, x, y);
            }
            
            return;

            for(var i = 0; i < 4; i++) 
            {
                var x = UnityEngine.Random.Range(0, _size.x);
                var y = UnityEngine.Random.Range(0, _size.y);

                CreateAndAddThing(TypeOfThing.Hen, x, y);
            }

            for(var i = 0; i < 10; i++) 
            {
                var x = UnityEngine.Random.Range(0, _size.x);
                var y = UnityEngine.Random.Range(0, _size.y);

                CreateAndAddThing(TypeOfThing.Chick, x, y);
            }

            for(var i = 0; i < 1; i++) 
            {
                var x = UnityEngine.Random.Range(0, _size.x);
                var y = UnityEngine.Random.Range(0, _size.y);

                CreateAndAddThing(TypeOfThing.Rooster, x, y);
            }

        }

        /*
            Sprites
        */
        
        public Thing GetThingNotInScene(TypeOfThing type)
        {
            return _all.Where(t => t.type == type).FirstOrDefault();
        }

        public Thing GetThingOnGrid(Vector2Int position)
        {
            return GetThingOnGrid(position.x, position.y);
        }

        public bool IsOnGrid(int x, int y)
        {
            return x >= 0 && x < _size.x && y >= 0 && y < _size.y;
        }

        public Thing GetThingOnGrid(int x, int y)
        {
            if(x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _grid[x, y];
            }

            return null;
        }

        /*
            Things
        */
        public IEnumerable<Thing> QueryThings()
        {
            return _things.Where(t => t.transform != null);
        }

        public IEnumerable<Thing> QueryThingsNotInScene()
        {
            return _all;
        }

        public Thing AddThing(Thing thing)
        {
            if(thing.fixedToGrid)
            {
                if(IsOnGrid(thing.position.x, thing.position.y))
                {
                    var existing = GetThingOnGrid(thing.position.x, thing.position.y);
                    if(existing != null)
                    {
                        RemoveThing(existing);
                    }

                    _grid[thing.position.x, thing.position.y] = thing;

                    _things.Add(thing);
                    thing.Setup();

                    if(OnThingAdded != null)
                        OnThingAdded(thing);
                }
            }
            else
            {
                _things.Add(thing);
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

        void RemoveThing(Thing thing)
        {
            if(thing == null)
                return;

            if(thing.fixedToGrid && thing.transform != null)
            {
                _grid[thing.position.x, thing.position.y] = null;
            }   

            // make sure this is called before
            // destroying the thing
            if(OnThingRemoved != null)
                OnThingRemoved(thing);

            _things.Remove(thing);

            thing.Destroy();
        }

        public Thing Create(TypeOfThing thingType)
        {
            return Create(thingType, 0, 0);
        }

        public Thing Create(TypeOfThing thingType, int x, int y)
        { 
            return Assets.Create(this, thingType, x, y);
        }

        public Thing CreateAndAddThing(TypeOfThing type, int x, int y)
        {
            return AddThing(Create(type, x, y));
        }

        public void Destroy(Thing thing)
        {
            RemoveThing(thing);
        }

        /*
            Pathfinding
        */
        public int TagFromString(string tag)
        {
            return System.Array.IndexOf(_aStarPath.GetTagNames(), tag);
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

            _aStarPath.UpdateGraphs(graphupdate);
        }

        public bool IsFloorForRegion(Vector2Int position)
        {
            return GetThingOnGrid(position) != null && !GetThingOnGrid(position).blocksPath;
        }

        public bool IsPathPossible(Vector2Int start, Vector2Int end)
        {
            return _regionManager.IsPathPossbile(start, end);
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
            foreach(var thing in _things.ToArray())
            {
                RemoveThing(thing);
            }
        }

        public void Update()
        {
            _director.Update();
            _regionManager.Update();

            WorldTime.Update();

            for(var i = 0; i < _things.Count; i++) 
            {
                _things[i].Update();
            }

            foreach(var positionalAudio in _positionalAudio)
            {
                positionalAudio.Update();
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                AddThing(Create(TypeOfThing.Villager, 
                    Mathf.FloorToInt(_size.x / 2), 
                    UnityEngine.Random.Range(0, _size.y)));
            }

            if(Input.GetKeyDown(KeyCode.O))
            {
                var random = _things[UnityEngine.Random.Range(0, _things.Count)];
                RemoveThing(random);
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                var toBuild = _things.Where(t => t.construction != null).ToArray();
                foreach(var thing in toBuild)
                {
                    thing.construction.Construct();
                    Destroy(thing);
                }
            }
        }

        public GameSave ToSaveObj()
        {
            return new GameSave()
            {
                Size = Size,
                Things = _things.Select(t => t.ToSaveObj()).ToArray()
            };
        }

        public void FromSaveObj(GameSave obj)
        {
            // clear game
            Clear();

            // load game
            foreach(var thingSave in obj.Things)
            {
                var thing = Create(thingSave.type);
                thing.FromSaveObj(thingSave);
                AddThing(thing);
            }
        }
        
        public void DrawGizmos()
        {
            if(!Application.isPlaying)
                return;

            for(var i = 0; i < _things.Count; i++) 
            {
                _things[i].DrawGizmos();
            }

            foreach(var positionalAudio in _positionalAudio)
            {
                positionalAudio.DrawGizmos();
            }

            _regionManager.DrawGizmos();
        }

    }

}
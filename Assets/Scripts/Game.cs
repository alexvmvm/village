﻿using System;
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

    public class Game : MonoBehaviour, ISave<GameSave>
    {
        public AstarPath AStarPath;
        public Vector2Int Size = Vector2Int.one * 50;
        public WorldTime WorldTime { get { return _worldTime; } }
        public List<Thing.ThingConfig> ThingConfigs { get; private set; }
        private List<Thing> _all;
        public ThingAdded OnThingAdded;
        public ThingRemoved OnThingRemoved;
        private List<PositionalAudio> _positionalAudio;
        private Director _director;
        private Thing[,] _grid;
        private List<Thing> _things;
        private WorldTime _worldTime;
        private RegionManager _regionManager;

        void Awake()
        {
            _things = new List<Thing>();
            _grid = new Thing[Size.x, Size.y];
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

        void Start()
        {
            _all.Clear();
            
            foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
            {
                if(thingType == TypeOfThing.None)
                    continue;
                var thing = Create(thingType, -10, -10);
                // if(thing.HasTrait<Agent>())
                //     thing.GetTrait<Agent>().PauseAgent();
                _all.Add(thing);
            }
        }

        public void Generate()
        {
            for(var x = 0; x < Size.x; x++)
            {
                for(var y = 0; y < Size.y; y++)
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

            for(var y = 0; y < Size.y; y++)
            {
                var x = Mathf.FloorToInt(Size.x / 2);
                CreateAndAddThing(TypeOfThing.Path, x, y);
            }
        
            var rand = 0;
            for(var y = 0; y < Size.y; y++)
            {
                var x = Size.x - 3;
                rand = UnityEngine.Random.Range(0, 4);
                AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x - 1, y));
                AddThing(Create(TypeOfThing.Stream, x, y));
                rand = UnityEngine.Random.Range(0, 2);
                AddThing(Create(rand == 0 ? TypeOfThing.Ore : TypeOfThing.Clay, x + 1, y));
            }

            for(var i = 0; i < 20; i++)
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAndAddThing(TypeOfThing.FallenWood, x, y);
            }
            
            return;

            for(var i = 0; i < 4; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAndAddThing(TypeOfThing.Hen, x, y);
            }

            for(var i = 0; i < 10; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAndAddThing(TypeOfThing.Chick, x, y);
            }

            for(var i = 0; i < 1; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAndAddThing(TypeOfThing.Rooster, x, y);
            }

        }

        /*
            Sprites
        */
        
        public Thing GetThingOnGrid(Vector2Int position)
        {
            return GetThingOnGrid(position.x, position.y);
        }

        public bool IsOnGrid(int x, int y)
        {
            return x >= 0 && x < Size.x && y >= 0 && y < Size.y;
        }

        public Thing GetThingOnGrid(int x, int y)
        {
            if(x >= 0 && x < Size.x && y >= 0 && y < Size.y)
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

        public Thing AddThing(Thing thing)
        {
            if(thing.Config.FixedToGrid)
            {
                if(IsOnGrid(thing.Position.x, thing.Position.y))
                {
                    var existing = GetThingOnGrid(thing.Position.x, thing.Position.y);
                    if(existing != null)
                    {
                        RemoveThing(existing);
                    }

                    _grid[thing.Position.x, thing.Position.y] = thing;

                    _things.Add(thing);
                    //thing.Setup();

                    OnThingAdded?.Invoke(thing);
                }
            }
            else
            {
                _things.Add(thing);
                //thing.Setup();

                if(OnThingAdded != null)
                        OnThingAdded(thing);
            }

            switch(thing.Config.TypeOfThing)
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

            if(thing.Config.FixedToGrid && thing.transform != null)
            {
                _grid[thing.Position.x, thing.Position.y] = null;
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
            return Assets.Create(thingType, x, y);
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
            return System.Array.IndexOf(AStarPath.GetTagNames(), tag);
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

            AStarPath.UpdateGraphs(graphupdate);
        }

        public bool IsFloorForRegion(Vector2Int position)
        {
            return GetThingOnGrid(position) != null && !GetThingOnGrid(position).Config.PathBlocking;
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
                    Mathf.FloorToInt(Size.x / 2), 
                    UnityEngine.Random.Range(0, Size.y)));
            }

            if(Input.GetKeyDown(KeyCode.O))
            {
                var random = _things[UnityEngine.Random.Range(0, _things.Count)];
                RemoveThing(random);
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                var toBuild = _things.Where(t => t.Construction != null).ToArray();
                foreach(var thing in toBuild)
                {
                    thing.Construction.Construct();
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
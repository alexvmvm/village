using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Pathfinding;
using Village.Things;
using Village.Things.Config;
using Village.AI;
using Village.Saving;
using UnityEngine.Profiling;

namespace Village
{
    public delegate void ThingAdded(Thing thing);
    public delegate void ThingRemoved(Thing thing);

    public class Game : MonoBehaviour, ISave<GameSave>
    {
        public AstarPath AStarPath;
        public Vector2Int Size = Vector2Int.one * 50;
        public WorldTime WorldTime { get { return _worldTime; } }
        public float TimeSinceLoaded { get { return _timeSinceLoaded; } }
        public List<ThingConfig> ThingConfigs { get; private set; }
        public ThingAdded OnThingAdded;
        public ThingRemoved OnThingRemoved;
        private List<PositionalAudio> _positionalAudio;
        private List<Thing> _things;
        private WorldTime _worldTime;
        private RegionManager _regionManager;
        private Dictionary<Vector2Int, Thing> _floor;
        private List<Thing> _loose;
        private Queue<GameObject> _delete;
        private float _timeSinceLoaded;
        private bool _loaded;

        void Awake()
        {
            _things = new List<Thing>();
            _worldTime = new WorldTime();
            _positionalAudio = new List<PositionalAudio>()
            {
                new PositionalAudio(this, "river", "running_water"),
                new PositionalAudio(this, "trees", "birds")
            };

            _regionManager = new RegionManager(this);
            
            _floor = new Dictionary<Vector2Int, Thing>();   
            _loose = new List<Thing>();         
            _delete = new Queue<GameObject>();
            
            ThingConfigs = Assets.GetAllThingConfigs().ToList();

            _timeSinceLoaded = 0f;
            _loaded = false;
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
                    
                    var position = new Vector2Int(x, y);

                    if(noise > 0.6f && noise < 0.65f)
                    {
                        CreateAtPosition(TypeOfThing.BerryBush, position);
                    }
                    else if(noise > 0.65f)
                    {
                        CreateAtPosition(TypeOfThing.Tree, position);
                    }
                    else if(noise2 > 0.8f)
                    {
                        CreateAtPosition(TypeOfThing.Rock, position);
                    }
                    else if(noise3 > 0.85f)
                    {
                        CreateAtPosition(TypeOfThing.MushroomGrowing, position);
                    }
                    else
                    {
                        CreateAtPosition(TypeOfThing.Grass, position); 
                    }
                    
                }
            }

            for(var y = 0; y < Size.y; y++)
            {
                var x = Mathf.FloorToInt(Size.x / 2);
                var p = new Vector2Int(x, y);
                CreateAtPosition(TypeOfThing.Path, p);
            }
        
            var rand = 0;
            for(var y = 0; y < Size.y; y++)
            {
                var x = Size.x - 3;
                rand = UnityEngine.Random.Range(0, 4);
                var p = new Vector2Int(x, y);
                CreateAtPosition(rand == 0 ? TypeOfThing.OreFloor : TypeOfThing.ClayFloor, p + Vector2Int.left);
                CreateAtPosition(TypeOfThing.Stream, p);
                rand = UnityEngine.Random.Range(0, 2);
                CreateAtPosition(rand == 0 ? TypeOfThing.OreFloor : TypeOfThing.ClayFloor, p + Vector2Int.right);
            }

            for(var i = 0; i < 20; i++)
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAtPosition(TypeOfThing.FallenWood, new Vector2Int(x, y));
            }

            return;

            for(var i = 0; i < 4; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAtPosition(TypeOfThing.Hen, new Vector2Int(x, y));
            }

            for(var i = 0; i < 10; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAtPosition(TypeOfThing.Chick, new Vector2Int(x, y));
            }

            for(var i = 0; i < 1; i++) 
            {
                var x = UnityEngine.Random.Range(0, Size.x);
                var y = UnityEngine.Random.Range(0, Size.y);

                CreateAtPosition(TypeOfThing.Rooster, new Vector2Int(x, y));
            }

        }



        /*
            Query
        */

        public bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x <= Size.x && position.y >= 0 && position.y <= Size.y;
        }

        public bool IsThingOnFloor(Vector2Int position)
        {
            return _floor.ContainsKey(position);
        }

        public Thing GetThingOnFloor(Vector2Int position)
        {
            return IsThingOnFloor(position) ? _floor[position] : null;
        }

        public bool IsLooseThing(Vector2Int position)
        {
            return QueryLooseThings().Any(t => t.transform != null && t.Position == position);
        }

        public Thing GetLooseThing(Vector2Int position)
        {
            return QueryLooseThings().FirstOrDefault(t => t.Position == position);
        }

        /*
            Things
        */

        public Thing CreateAtPosition(TypeOfThing thingType, Vector2Int position)
        {
            var thing = Assets.Create(thingType, position.x, position.y);

            if(thing.Config.FixedToFloor)
            {
                if(IsThingOnFloor(thing.Position))
                    Remove(GetThingOnFloor(thing.Position));

                _floor[thing.Position] = thing;
                _things.Add(thing);
            }
            else
            {
                if(IsLooseThing(thing.Position))
                    Remove(GetLooseThing(thing.Position));

                _loose.Add(thing);
                _things.Add(thing);
            }

            OnThingAdded?.Invoke(thing);

            switch(thing.Config.TypeOfThing)
            {
                case TypeOfThing.Villager:
                    EventManager.TriggerEvent(Constants.VILLAGER_ARRIVED);
                break;
            }

            thing.RefreshSprite();

            return thing;
        }

        public void Remove(Thing thing)
        {
            if(thing == null)
                return;

            if(thing.Config.FixedToFloor)
            {
                _floor.Remove(thing.Position);
            }   
            else 
            {
                _loose.Remove(thing);
            }

            // make sure this is called before
            // destroying the thing
            if(OnThingRemoved != null)
                OnThingRemoved(thing);

            if(thing.Config.FixedToFloor)
                thing.ResetPath();

            _things.Remove(thing);

            _delete.Enqueue(thing.gameObject);
        }

        public IEnumerable<Thing> QueryThings()
        {
            return _things;
        }

        public IEnumerable<Thing> QueryLooseThings()
        {
            return _loose;
        }

        private static Vector2Int[] _dropLocations = new Vector2Int[]
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up
        };

        public Vector2Int? FindNearestLoosePosition(Vector2Int position)
        {
            foreach(var p in _dropLocations)
            {
                var testPositon = position + p;
                if(!IsFloorForRegion(testPositon) || QueryLooseThings().Any(t => t.Position == testPositon))
                    continue;
                return testPositon;
            }   
            return null;
        }

        /*
            Construction
        */

        public bool IsPlaceableAt(ThingConfig config, Vector2Int position)
        {
            var current = GetThingOnFloor(position);
            if (current == null)
                return false;
            if (ConstructAtPosition(current.Position))
                return false;
            if (config.ConstructionConfig != null && config.ConstructionConfig.BuildOn.HasValue && config.ConstructionConfig.BuildOn != current.Config.TypeOfThing)
                return false;
            return current.Config.BuildSite;
        }
        
        // todo: make this more performant
        bool ConstructAtPosition( Vector2Int position)
        {
            return QueryThings().Any(t => t.Config.TypeOfThing == TypeOfThing.Blueprint && t.Position == position);
        }

        /*
            Pathfinding
        */
        public int TagFromString(string tag)
        {
            return System.Array.IndexOf(AStarPath.GetTagNames(), tag);
        }

        public void UpdateAstarPath(Vector2Int position, string pathTag)
        {
            var graphupdate = new GraphUpdateObject
            {
                setTag = TagFromString(pathTag),
                bounds = new Bounds(new Vector3(position.x, position.y, 0), Vector3.one),
                modifyTag = true
            };

            AStarPath.UpdateGraphs(graphupdate);
        }

        public bool IsFloorForRegion(Vector2Int position)
        {
            return GetThingOnFloor(position) != null && GetThingOnFloor(position).Config.PathTag != Movement.TAG_BLOCKING;
        }

        public bool IsPathPossible(Vector2Int start, Vector2Int end)
        {
            return _regionManager.IsPathPossbile(start, end);
        }

        public Thing IsPathPossibleToThing(Vector2Int start, Func<Thing, bool> filter)
        {
            return _regionManager.IsPathPossbileToThing(start, filter);
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

        public void Update()
        {

            Profiler.BeginSample("Game_Update_RegionManagerUpdate");   
            _regionManager.Update();
            Profiler.EndSample();

            Profiler.BeginSample("Game_Update_WorldTimeUpdate");   
            WorldTime.Update();
            Profiler.EndSample();

            foreach(var positionalAudio in _positionalAudio)
            {
                positionalAudio.Update();
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                var position = new Vector2Int(Mathf.FloorToInt(Size.x / 2), UnityEngine.Random.Range(0, Size.y));
                CreateAtPosition(TypeOfThing.Villager, position);
            }

            if(Input.GetKeyDown(KeyCode.O))
            {
                var random = _things[UnityEngine.Random.Range(0, _things.Count)];
                Remove(random);
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                var toBuild = _things.Where(t => t.Config.TypeOfThing == TypeOfThing.Blueprint).ToArray();
                foreach(var thing in toBuild)
                {
                    thing.Construct();
                }
            }

            if(_loaded)
            {
                _timeSinceLoaded += Time.deltaTime;
            }
        }

        void LateUpdate()
        {
            while(_delete.Count > 0)
            {
                Destroy(_delete.Dequeue());
            }
        }

        /*
            Saving/Loading
        */

        public IEnumerator LoadNewGame()
        {
            ResetTimeSinceLoad();
            yield return StartCoroutine(RemoveAll());
            Generate();
        }

        public IEnumerator LoadGame(GameSave obj)
        {
            Time.timeScale = 0;
            ResetTimeSinceLoad();
            Profiler.BeginSample("Game_LoadGame_RemoveAll");
            Debug.Log("Clearing Scene");
            yield return StartCoroutine(RemoveAll());
            Profiler.EndSample();
            Profiler.BeginSample("Game_LoadGame_FormSaveObj");
            Debug.Log($"Loading Game with {obj.Things.Length} entities...");
            yield return StartCoroutine(FromSaveObjC(obj));
            Debug.Log("Game Loaded!");
            Profiler.EndSample();
            Time.timeScale = 1f;

        }

        void ResetTimeSinceLoad()
        {
            _loaded = true;
            _timeSinceLoaded = 0f;
        }

        IEnumerator RemoveAll()
        {
            foreach(var thing in _things.ToArray())
            {
                Remove(thing);
            }
            
            while(_delete.Count > 0)
                yield return null;

            Debug.Log("All Removed");
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
            throw new NotImplementedException();
        }

        IEnumerator FromSaveObjC(GameSave obj)
        {
            // load game
            var count = 0;
            foreach(var thingSave in obj.Things)
            {
                var thing = CreateAtPosition(thingSave.type, thingSave.position);
                thing.FromSaveObj(thingSave);
                count++;
                if(count % 1000 == 0)
                {
                    yield return null;
                }
            }
        }
        
        public void OnDrawGizmos()
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
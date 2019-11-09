using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Village.Saving;

namespace Village.Things
{
    public class Thing : MonoBehaviour, ISave<ThingSave>
    {
        public enum AgentConfig
        {
            None,
            Villager,
            Animal
        }

        public class ThingConfig
        {
            public string Sprite;
            public string Name;
            public String Description;
            public int Hitpoints;
            public TypeOfThing TypeOfThing;
            public Color Color;
            public Vector3 Scale;
            public int SortingOrder;
            public bool FixedToGrid;
            public ITileRule TileRule;
            public int GridGroup;
            public bool Floor;
            public bool PathBlocking;
            public bool LightBlocking;
            public bool BuildSite;
            public bool Pipe;
            public bool Edible;
            public bool Storeable;
            public string StoreGroup;
            public bool Resource;
            public TypeOfThing Produces;
            public string PositionalAudioGroup;
            public string PathTag;
            public bool Walkable;
            public TypeOfThing[] RequiredToCraft;
            public bool AssignToFamily;
            public bool Inventory;
            public bool Fire;
            public bool Storage;
            public ConstructionConfig Construction;
            public FactoryConfig Factory;
            public CropConfig Crop;
            public AgentConfig Agent;
        }

        public class ConstructionConfig 
        {
            public TypeOfThing? BuildOn { get; protected set; }
            public TypeOfThing Builds { get; protected set; }
            public ConstructionGroup Group { get; protected set; }
            public TypeOfThing Requires { get; protected set; }

            public ConstructionConfig(
                TypeOfThing? buildOn,
                TypeOfThing builds,
                ConstructionGroup group,
                TypeOfThing requires
            )
            {
                BuildOn = buildOn;
                builds = Builds;
                group = Group;
                requires = Requires;
            }
        }

        public class FactoryConfig 
        {
            public TypeOfThing[] Produces { get; protected set; }

            public FactoryConfig(TypeOfThing[] produces)
            {
                Produces = produces;
            }
        }

        public class CropConfig
        {
            public float TimeToGrow { get; protected set; }
            public string[] Sprites { get; protected set; }

            public CropConfig(float timeToGrow, string[] sprites)
            {
                TimeToGrow = timeToGrow;
                Sprites = sprites;
            }
        }

        public ThingConfig Config { get; protected set; }
        public SpriteRenderer SpriteRenderer { get; protected set; }
        public int Hitpoints { get; set; }
        // public Vector3 Position { get { return transform.position; } }
        public Factory Factory { get; protected set; }
        public Storage Storage { get; protected set; }
        public Inventory Inventory { get; protected set; }
        public Construction Construction { get; protected set; }

        void Awake()
        {
            SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        void Start()
        {
            RefreshSprite();

            if (!string.IsNullOrEmpty(Config.PathTag))
            {
                game.UpdateAstarPath(transform.position.ToVector2IntFloor(), Config.PathTag, Config.Walkable);
            }


            if (Config.LightBlocking)
            {
                transform.gameObject.AddComponent<BoxCollider2D>();
                transform.gameObject.layer = LayerMask.NameToLayer("Blocks Light");
            }

            // foreach (var trait in _traits)
            //     trait.Setup();
        }

        public void Setup(ThingConfig config)
        {
            Config = config;
            Hitpoints = config.Hitpoints;
        }

        // properties    
        public string id;
        //public string sprite;
        //public string name;
        //public string description;
        public string belongsToFamily;
        // public bool assignToFamily;
        public string ownedBy;
        public Game game;
        //public int hitpoints = 100;
        //public TypeOfThing type;
        //public Transform transform;
        //public GameObject gameObject;
        //public SpriteRenderer spriteRenderer;
        //public Color color = Color.white;
        //public Vector3 scale = Vector3.one;
        //public int sortingOrder;
        //public bool fixedToGrid;
        //public ITileRule tileRule;
        //public int positionalGroup;
        //public bool floor;
        //public bool blocksPath;
        //ublic bool playerBuiltFloor;
        //public bool blocksLight;
        //public bool buildOn;
        //public bool pipe;
        //public bool edible;

        // public bool storeable;
        // public string storeGroup;
        // public bool resource;
        // public TypeOfThing produces;
        // public TypeOfThing requiredToGet;

        // public string positionalAudioGroup;
        // public string pathTag;
        // public bool walkable = true;
        //public Construction construction;
        private TextMesh _textMesh;
        private GameObject _labelObj;
        //public TypeOfThing[] requiredToCraft;
        //private List<ITrait> _traits;

        // public Thing(TypeOfThing type, Game game)
        // {
        //     this.game = game;

        //     this.id = Guid.NewGuid().ToString();
        //     this.type = type;
        //     this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        //     this.produces = type;
        //     // this._traits = new List<ITrait>();
        // }

        // public Vector2Int position
        // {
        //     get
        //     {
        //         return new Vector2Int(
        //             Mathf.FloorToInt(transform.position.x),
        //             Mathf.FloorToInt(transform.position.y));
        //     }
        // }

        //public bool Exists { get { return transform != null; } }

        /*
            Traits
        */
        // public void AddTrait(ITrait trait)
        // {
        //     _traits.Add(trait);
        // }

        // public T GetTrait<T>() where T : ITrait
        // {
        //     return (T)_traits.Where(t => typeof(T) == t.GetType()).FirstOrDefault();
        // }

        // public bool HasTrait<T>() where T : ITrait
        // {
        //     return GetTrait<T>() != null;
        // }   

        /*
            Label
        */
        public void SetLabel(string label)
        {
            if (_labelObj == null)
            {
                _labelObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Label"));
                _labelObj.transform.SetParent(transform);
                _labelObj.transform.localPosition = new Vector3(0, -0.8f, 0);
                _labelObj.GetComponentInChildren<MeshRenderer>().sortingOrder = (int)SortingOrders.Labels;
                _textMesh = _labelObj.GetComponentInChildren<TextMesh>();
            }

            _textMesh.text = label;
        }

        public void SetSprite()
        {
            var spriteName = Config.TileRule != null ? Config.TileRule.GetSprite(GetGridPositions()) : Config.Sprite;
            this.SpriteRenderer.sprite = Assets.GetSprite(spriteName);
            this.SpriteRenderer.sortingOrder = Config.SortingOrder;
            this.SpriteRenderer.color = Config.Color;
            this.transform.localScale = Config.Scale;
            this.transform.rotation = Assets.GetSpriteRotation(spriteName);
        }

        public void RefreshSprite()
        {
            SetSprite();

            if (!Config.FixedToGrid)
                return;

            var px = Mathf.FloorToInt(transform.position.x);
            var py = Mathf.FloorToInt(transform.position.y);

            for (var x = px - 1; x <= px + 1; x++)
            {
                for (var y = py - 1; y <= py + 1; y++)
                {
                    if (x == px && y == py)
                        continue;

                    var thing = game.GetThingOnGrid(x, y);
                    if (thing != null)
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

            for (var x = px - 1; x <= px + 1; x++)
            {
                for (var y = py - 1; y <= py + 1; y++)
                {
                    if (x == px && y == py)
                        continue;

                    var thing = game.GetThingOnGrid(x, y);
                    if (thing != null && thing.Config.GridGroup == Config.GridGroup)
                    {
                        var vector = new Vector2Int(x - px, y - py);
                        if (vector == Vector2Int.up)
                            position = position | Position.Top;
                        else if (vector == Vector2Int.down)
                            position = position | Position.Bottom;
                        else if (vector == Vector2Int.left)
                            position = position | Position.Left;
                        else if (vector == Vector2Int.right)
                            position = position | Position.Right;
                    }

                }
            }

            return position;
        }

        public bool CanBeSeletected()
        {
            return Config.AssignToFamily || Config.Factory != null || Config.Storage;
        }

        public bool IsInStorage()
        {
            return transform.parent != null;
        }

        public void Destroy()
        {
            if (!string.IsNullOrEmpty(Config.PathTag))
            {
                game.UpdateAstarPath(transform.position.ToVector2IntFloor(), "ground", true);
            }

            if (_labelObj != null)
                GameObject.Destroy(_labelObj);

            GameObject.Destroy(transform.gameObject);
        }

        public virtual void Update()
        {
            var label = "";
            if (Config.Resource)
                label += $"x{Hitpoints}\n";
            if (!string.IsNullOrEmpty(ownedBy))
                label += $"owner: {ownedBy}\n";

            if (!string.IsNullOrEmpty(label))
                SetLabel(label);
        }

        public ThingSave ToSaveObj()
        {
            return new ThingSave
            {
                id = id,
                position = transform.position,
                type = Config.TypeOfThing,
                hitpoints = Config.Hitpoints,
                ownedBy = ownedBy
            };
        }

        public void FromSaveObj(ThingSave save)
        {
            this.id = save.id;
            this.transform.position = save.position;
            this.Config.TypeOfThing = save.type;
            this.Hitpoints = save.hitpoints;
            this.ownedBy = save.ownedBy;
        }

        public void DrawGizmos()
        {
#if UNITY_EDITOR

            if (!string.IsNullOrEmpty(belongsToFamily))
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

}
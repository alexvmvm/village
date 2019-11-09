using UnityEngine;
using System;
using Village.Saving;
using SwordGC.AI.Goap;

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
            public int Hitpoints = 100;
            public TypeOfThing TypeOfThing;
            public Color Color = Color.white;
            public Vector3 Scale = Vector3.one;
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
        public Vector2Int Position { get { return transform.position.ToVector2IntFloor(); } }
        public Factory Factory { get; protected set; }
        public Storage Storage { get; protected set; }
        public Inventory Inventory { get; protected set; }
        public Construction Construction { get; protected set; }
        public GoapAgent Agent { get; protected set; }

        void Awake()
        {
            SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Game = FindObjectOfType<Game>();
        }
        
        public void Setup(ThingConfig config)
        {
            Config = config;
            Hitpoints = config.Hitpoints;

            RefreshSprite();

            if (!string.IsNullOrEmpty(Config.PathTag))
            {
                Game.UpdateAstarPath(transform.position.ToVector2IntFloor(), Config.PathTag, Config.Walkable);
            }


            if (Config.LightBlocking)
            {
                transform.gameObject.AddComponent<BoxCollider2D>();
                transform.gameObject.layer = LayerMask.NameToLayer("Blocks Light");
            }
        }

        public string id;
        public string belongsToFamily;
        public string ownedBy;
        public Game Game;
        private TextMesh _textMesh;
        private GameObject _labelObj;

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

                    var thing = Game.GetThingOnGrid(x, y);
                    if (thing != null)
                    {
                        thing.SetSprite();
                    }
                }
            }
        }

        public Positions GetGridPositions()
        {
            var position = Positions.None;

            var px = Mathf.FloorToInt(transform.position.x);
            var py = Mathf.FloorToInt(transform.position.y);

            for (var x = px - 1; x <= px + 1; x++)
            {
                for (var y = py - 1; y <= py + 1; y++)
                {
                    if (x == px && y == py)
                        continue;

                    var thing = Game.GetThingOnGrid(x, y);
                    if (thing != null && thing.Config.GridGroup == Config.GridGroup)
                    {
                        var vector = new Vector2Int(x - px, y - py);
                        if (vector == Vector2Int.up)
                            position = position | Positions.Top;
                        else if (vector == Vector2Int.down)
                            position = position | Positions.Bottom;
                        else if (vector == Vector2Int.left)
                            position = position | Positions.Left;
                        else if (vector == Vector2Int.right)
                            position = position | Positions.Right;
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
                Game.UpdateAstarPath(transform.position.ToVector2IntFloor(), "ground", true);
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
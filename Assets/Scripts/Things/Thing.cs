using UnityEngine;
using System;
using Village.Saving;
using Village.AI;

namespace Village.Things
{
    public delegate void ThingMoved(Thing thing, Vector2Int previous, Vector2Int current);

    public class Thing : MonoBehaviour, ISave<ThingSave>
    {
        public static ThingMoved OnThingMoved;

        public enum AgentConfig
        {
            None,
            Villager,
            Animal
        }

        [Serializable]    
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
            public bool FixedToFloor;
            public ITileRule TileRule;
            public int GridGroup;
            public bool Floor;
            public bool LightBlocking;
            public bool BuildSite;
            public bool Pipe;
            public bool Edible;
            public bool Storeable;
            public string StoreGroup;
            public bool Resource;
            public bool Tool;
            public TypeOfThing Produces;
            public TypeOfThing RequiredToProduce = TypeOfThing.None;
            public string PositionalAudioGroup;
            public string PathTag;
            public TypeOfThing[] RequiredToCraft;
            public bool AssignToFamily;
            public bool Inventory;
            public bool Fire;
            public bool Storage;
            public FactoryConfig Factory;
            public CropConfig Crop;
            public AgentConfig Agent;
            public ConstructionConfig Construction;

            public ThingConfig(TypeOfThing type)
            {
                TypeOfThing = type;
                Produces = type;
            }
        }

        [Serializable]    
        public class ConstructionConfig
        {
            public TypeOfThing? BuildOn { get; protected set; }
            public TypeOfThing Builds { get; protected set; }
            public Thing.ThingConfig BuildsConfig { get; private set; }
            public ConstructionGroup Group { get; protected set; }
            public TypeOfThing Requires { get; protected set; }

            public ConstructionConfig(
                TypeOfThing? buildOn,
                ConstructionGroup group,
                TypeOfThing requires
            )
            {
                BuildOn = buildOn;
                Group = group;
                Requires = requires;
            }
        }

        [Serializable]    
        public class FactoryConfig
        {
            public TypeOfThing[] Produces { get; protected set; }

            public FactoryConfig(TypeOfThing[] produces)
            {
                Produces = produces;
            }
        }

        [Serializable]    
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
        public Factory Factory { get; private set; }
        public Fire Fire { get; private set; }
        public Storage Storage { get; private set; }
        public Inventory Inventory { get; private set; }
        public Agent Agent { get; private set; }
        public TypeOfThing Builds { get; private set; }
        public TypeOfThing Requires { get; private set; }

        private Vector2Int _previousPosition { get; set; }

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
                Game.UpdateAstarPath(transform.position.ToVector2IntFloor(), Config.PathTag);
            }


            if (Config.LightBlocking)
            {
                gameObject.AddComponent<BoxCollider2D>();
                gameObject.layer = LayerMask.NameToLayer("Blocks Light");
            }

            if(Config.Inventory)
            {
                Inventory = gameObject.AddComponent<Inventory>();
            }
            
            if(Config.Factory != null)
            {
                Factory = gameObject.AddComponent<Factory>();
                Factory.Setup(Config.Factory);
            }

            if(Config.Fire)
            {
                Fire = gameObject.AddComponent<Fire>();
            }

            if(Config.Storage)
            {
                Storage = gameObject.AddComponent<Storage>();
            }

            switch(Config.Agent)
            {
                case AgentConfig.Villager:
                Agent = gameObject.AddComponent<Villager>();
                break;
                case AgentConfig.Animal:
                Agent = gameObject.AddComponent<Animal>();
                break;
            }
        }

        public string id;
        public string belongsToFamily;
        public string ownedBy;
        public Game Game;
        private TextMesh _textMesh;
        private GameObject _labelObj;

        /*
            Construction
        */
        public void SetBuilds(TypeOfThing type)
        {
            Builds = type;
            Requires = Assets.CreateThingConfig(type).Construction.Requires;
        }

        public void Construct()
        {
            if(Config.FixedToFloor)
            {
                var existing = Game.GetThingOnFloor(Position);
                if(existing != null)
                    Game.Remove(existing);
            }

            var thing = Game.CreateAtPosition(Builds, Position);
            Game.Remove(this);
        }

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

            if (!Config.FixedToFloor)
                return;

            var px = Mathf.FloorToInt(transform.position.x);
            var py = Mathf.FloorToInt(transform.position.y);

            for (var x = px - 1; x <= px + 1; x++)
            {
                for (var y = py - 1; y <= py + 1; y++)
                {
                    if (x == px && y == py)
                        continue;

                    var position = new Vector2Int(x, y);
                    var thing = Game.GetThingOnFloor(position);
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

            var px = Position.x;
            var py = Position.y;

            for (var x = px - 1; x <= px + 1; x++)
            {
                for (var y = py - 1; y <= py + 1; y++)
                {
                    if (x == px && y == py)
                        continue;

                    var p = new Vector2Int(x, y);
                    var thing = Game.GetThingOnFloor(p);
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
            return transform.parent != null && transform.parent.GetComponent<Storage>() != null;
        }

        public void ResetPath()
        {
            if (!string.IsNullOrEmpty(Config.PathTag))
            {
                Game.UpdateAstarPath(transform.position.ToVector2IntFloor(), Movement.TAG_GROUND);
            }
        }

        public virtual void Update()
        {
            if(_previousPosition != Position)
            {
                if(OnThingMoved != null)
                    OnThingMoved(this, _previousPosition, Position);
                _previousPosition = Position;
            }

            // var label = "";
            // if (Config.Resource)
            //     label += $"x{Hitpoints}\n";
            // if (!string.IsNullOrEmpty(ownedBy))
            //     label += $"owner: {ownedBy}\n";

            // if (!string.IsNullOrEmpty(label))
            //     SetLabel(label);
        }

        public ThingSave ToSaveObj()
        {
            return new ThingSave
            {
                id = id,
                position = transform.position.ToVector2IntFloor(),
                type = Config.TypeOfThing,
                hitpoints = Config.Hitpoints,
                ownedBy = ownedBy,
                builds = Builds,
                requires = Requires
            };
        }

        public void FromSaveObj(ThingSave save)
        {
            this.id = save.id;
            //this.transform.position = save.position.ToVector3();
            this.Config.TypeOfThing = save.type;
            this.Hitpoints = save.hitpoints;
            this.ownedBy = save.ownedBy;
            this.Builds = save.builds;
            this.Requires = save.requires;
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
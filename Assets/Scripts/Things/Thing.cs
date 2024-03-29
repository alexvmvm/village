﻿using UnityEngine;
using System;
using Village.Saving;
using Village.AI;
using Village.Things.Config;

namespace Village.Things
{
    public delegate void ThingMoved(Thing thing, Vector2Int previous, Vector2Int current);
    public delegate void ThingConstructed();

    public class Thing : MonoBehaviour, ISave<ThingSave>
    {
        public static ThingMoved OnThingMoved;
        public ThingConstructed OnThingConstructed;
        public ThingConfig Config { get; protected set; }
        public SpriteRenderer SpriteRenderer { get; protected set; }
        public bool LabelActive { get { return _labelObj != null && _labelObj.activeSelf; } }
        public int Hitpoints { get; set; }
        public Vector2Int Position { get { return transform.position.ToVector2IntFloor(); } }
        public Factory Factory { get; private set; }
        public Fire Fire { get; private set; }
        public Storage Storage { get; private set; }
        public Crop  Crop { get; private set; }
        public Inventory Inventory { get; private set; }
        public GOAPAgent Agent { get; private set; }
        public TypeOfThing Builds { get; private set; }
        public TypeOfThing Requires { get; private set; }
        public ITileRule TileRule { get; private set; }
        private Vector2Int _previousPosition { get; set; }

        void Awake()
        {
            SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            SpriteRenderer.material = Assets.GetMaterial("Sprite-Diffuse");
            
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

            // todo: must be a better way...
            if(config.TileRuleConfig != null)
            {
                var type = Type.GetType(config.TileRuleConfig.Type);
                var instance = (ITileRule)Activator.CreateInstance(type, new object[] { config.TileRuleConfig.Sprites });
                TileRule = instance;
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
            
            if(Config.FactoryConfig != null)
            {
                Factory = gameObject.AddComponent<Factory>();
                Factory.Setup(Config.FactoryConfig);
            }

            if(Config.Fire)
            {
                Fire = gameObject.AddComponent<Fire>();
            }

            if(Config.Storage)
            {
                Storage = gameObject.AddComponent<Storage>();
            }

            if(Config.CropConfig != null)
            {
                Crop = gameObject.AddComponent<Crop>();
                Crop.Setup(Config.CropConfig);
            }

            switch(Config.Agent)
            {
                case AgentConfig.Villager:
                Agent = gameObject.AddComponent<Villager>();
                break;
                case AgentConfig.Animal:
                //Agent = gameObject.AddComponent<Animal>();
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
            var config = Assets.GetThingConfig(type);
            
            Builds = config.ConstructionConfig.Builds;
            Requires = config.ConstructionConfig.Requires;
        }

        public void Construct()
        {
            if(Config.FixedToFloor)
            {
                var existing = Game.GetThingOnFloor(Position);
                if(existing != null)
                    Game.Remove(existing);
            }

            if(OnThingConstructed != null)
                OnThingConstructed();

            if(Builds != TypeOfThing.None)
            {
                var thing = Game.CreateAtPosition(Builds, Position);
            }

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

        /*
            Sprites
        */

        public void SetSprite()
        {
            var spriteName = TileRule != null ? TileRule.GetSprite(GetGridPositions()) : Config.Sprite;
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
            return Config.AssignToFamily || Config.FactoryConfig != null || Config.Storage;
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

            if(Config.Resource)
            {
                SetLabel($"x{Hitpoints}");
            }
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

            if(!Config.FixedToFloor)
            {
                var style = new GUIStyle();
                style.fontSize = 10;
                style.normal.textColor = Color.white;

                var label = $"Hitpoints: {Hitpoints}";

                // current actions
                var position = transform.position + Vector3.up;
                UnityEditor.Handles.Label(position, label, style);
            }
#endif
        }
    }

}
using UnityEngine;
using System;
using Village.Things;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Village.Things.Config
{
    public enum AgentConfig
    {
        None,
        Villager,
        Animal
    }

    public class TileRuleConfig
    {
        public string[] Sprites;
        public string Type;
    }

    [Serializable]
    public class ThingConfig
    {
        public string Sprite;
        public string Name;
        public String Description;
        public string Parent;
        public int Hitpoints = 100;
        public TypeOfThing TypeOfThing;
        public Color Color = UnityEngine.Color.white;
        public Vector3 Scale = Vector3.one;
        public int SortingOrder;
        public bool FixedToFloor;
        public TileRuleConfig TileRuleConfig;
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
    }

}
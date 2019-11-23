using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Village.Things;
using Village.AI;
using Village;
using System.Linq;
using Village.Things.Config;
using Village.Things.Serialization;

public class Assets 
{
    private static Dictionary<string, Sprite> _sprites;
    private static Dictionary<string, AudioClip> _audioClips;
    private static Dictionary<string, Material> _materials;
    private static Dictionary<string, GameObject> _prefabs;
    private static ThingSerialization _thingSerialization;
    private static ThingConfig[] _things;
    private static Dictionary<TypeOfThing, ThingConfig> _thingDictionary;

    public static Sprite GetSprite(string name)
    {
        if(_sprites == null)
        {
            _sprites = new Dictionary<string, Sprite>();
            foreach(var sprite in Resources.LoadAll<Sprite>(""))
                _sprites[sprite.name] = sprite;
        }

        if(string.IsNullOrEmpty(name))
            throw new System.Exception($"Sprite name is null");

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

    static void SetupThings()
    {
        _thingSerialization = new ThingSerialization();
        var xml = Resources.Load<TextAsset>("Config/Things").text;
        _things = _thingSerialization.LoadFromString(xml);
        _thingDictionary = _things.ToDictionary(t => t.TypeOfThing);
    }

    public static ThingConfig[] GetAllThingConfigs()
    {
        if(_things == null)
            SetupThings();
        return _things;
    }

    public static ThingConfig GetThingConfig(TypeOfThing thingType)
    {
        if(_thingDictionary == null)
            SetupThings();

        if(!_thingDictionary.ContainsKey(thingType))
            throw new Exception($"TypeOfThing {thingType} not found in asset dictionary");

        return _thingDictionary[thingType];
    }

    public static Thing Create(TypeOfThing typeOfThing, int x, int y)
    {
        var id = Guid.NewGuid().ToString();
        var config = GetThingConfig(typeOfThing);
        var gameObject = new GameObject($"{typeOfThing}_{id}");
        var thing = gameObject.AddComponent<Thing>();
        thing.transform.position = new Vector3(x, y);
        thing.id = id;
        thing.Setup(config);
        return thing;
    }
}

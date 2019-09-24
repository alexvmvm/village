﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    public Game Game;
    public ObjectPooler GroupPooler;
    public ObjectPooler ButtonPooler;
    
    private List<Thing> _things;

    void Awake()
    {
        GroupPooler.DeactivateAll();
        foreach(ConstructionGroup group in Enum.GetValues(typeof(ConstructionGroup)))
        {
            var obj = GroupPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = group.ToString().ToUppercaseFirst();
            obj.SetActive(true);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                SetupGroupButtons(group);
            });
        }

        _things = new List<Thing>();
        foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
        {
            _things.Add(Game.Create(thingType));
        }
    }

    public void SetupGroupButtons(ConstructionGroup group)
    {
        ButtonPooler.DeactivateAll();
        
        foreach(var thing in _things.Where(t => t.construction != null && t.construction.Group == group))
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.name.ToUppercaseFirst();
            obj.SetActive(true);

            var thingToBuild = _things.Where(t => t.type == thing.construction.BuildType).FirstOrDefault();
            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Game.GetSprite(thingToBuild.sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                Game.CurrentType = thing.type;
            });
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            gameObject.SetActive(false);
        }
    }
}

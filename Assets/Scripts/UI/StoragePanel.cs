﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Village.Things;

public class StoragePanel : MonoBehaviour
{
    public ObjectPooler StorageGroupPooler;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    IEnumerable<IGrouping<string, Thing>> GetStorableThings()
    {
        return _session.Game.QueryThingsNotInScene().Where(t => t.storeable).GroupBy(t => t.storeGroup);
    }

    public void Setup(Thing thing)
    {
        var storage = thing.GetTrait<Storage>();
        StorageGroupPooler.DeactivateAll();
        foreach(var group in GetStorableThings())
        {
            var obj = StorageGroupPooler.GetPooledObject();
            var storageGroup = obj.GetComponent<StorageGroup>();
            storageGroup.Heading.text = group.Key.ToUppercaseFirst();
            storageGroup.GroupToggle.onValueChanged.RemoveAllListeners();
            storageGroup.GroupToggle.isOn = group.Any(t => storage.IsAllowing(t.type));
            storageGroup.GroupToggle.onValueChanged.AddListener((value) => {
                foreach(var toggle in storageGroup.gameObject.GetComponentsInChildren<Toggle>())
                    toggle.isOn = value;
            });

            storageGroup.TogglePooler.DeactivateAll();
            foreach(var groupThing in group)
            {
                var toggleObj = storageGroup.TogglePooler.GetPooledObject();
                toggleObj.GetComponentInChildren<Text>().text = groupThing.name.ToUppercaseFirst();
                toggleObj.SetActive(true);

                var toggle = toggleObj.GetComponentInChildren<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.isOn = storage.IsAllowing(thing.type);
                toggle.onValueChanged.AddListener((value) => {
                    if(value)
                        thing.GetTrait<Storage>().Allow(groupThing.type);
                    else
                        thing.GetTrait<Storage>().Disallow(groupThing.type);
                });
            }

            obj.SetActive(true);

            // var obj = TogglePooler.GetPooledObject();
            // obj.GetComponentInChildren<Text>().text = item.name.ToUppercaseFirst();
            // var toggle = obj.GetComponentInChildren<Toggle>();
            // toggle.onValueChanged.RemoveAllListeners();
            // toggle.isOn = false;
            // toggle.onValueChanged.AddListener((value) => {
            //     if(value) 
            //         thing.GetTrait<Storage>().Allow(item.type);
            //     else
            //         thing.GetTrait<Storage>().Disallow(item.type);
            // });
            // obj.SetActive(true);
        }

        gameObject.SetActive(true);
    }
}

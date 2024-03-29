﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Village.Things;
using Village.Things.Config;

public class StoragePanel : MonoBehaviour
{
    public ObjectPooler StorageGroupPooler;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    IEnumerable<IGrouping<string, ThingConfig>> GetStorableThings()
    {
        return _session.Game.ThingConfigs.Where(t => t.Storeable).GroupBy(t => t.StoreGroup);
    }

    public void Setup(Thing thing)
    {
        var storage = thing.Storage;
        StorageGroupPooler.DeactivateAll();
        foreach(var group in GetStorableThings())
        {
            var obj = StorageGroupPooler.GetPooledObject();
            var storageGroup = obj.GetComponent<StorageGroup>();
            storageGroup.Heading.text = group.Key.ToUppercaseFirst();
            storageGroup.GroupToggle.onValueChanged.RemoveAllListeners();
            storageGroup.GroupToggle.isOn = group.Any(t => storage.IsAllowing(t.TypeOfThing));
            storageGroup.GroupToggle.onValueChanged.AddListener((value) => {
                foreach(var toggle in storageGroup.gameObject.GetComponentsInChildren<Toggle>())
                    toggle.isOn = value;
            });

            storageGroup.TogglePooler.DeactivateAll();
            foreach(var groupThing in group)
            {
                var toggleObj = storageGroup.TogglePooler.GetPooledObject();
                toggleObj.GetComponentInChildren<Text>().text = groupThing.Name.ToUppercaseFirst();
                toggleObj.SetActive(true);

                var toggle = toggleObj.GetComponentInChildren<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.isOn = storage.IsAllowing(groupThing.TypeOfThing);
                toggle.onValueChanged.AddListener((value) => {
                    if(value)
                        thing.Storage.Allow(groupThing.TypeOfThing);
                    else
                        thing.Storage.Disallow(groupThing.TypeOfThing);
                });
            }

            obj.SetActive(true);
        }

        gameObject.SetActive(true);
    }
}

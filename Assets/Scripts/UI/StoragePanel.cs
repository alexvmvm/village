using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Village.Things;
using Village;

public class StoragePanel : MonoBehaviour
{
    public ObjectPooler TogglePooler;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    public void Setup(Thing thing)
    {
        TogglePooler.DeactivateAll();

        foreach(var item in _session.Game.AllThings.Where(t => t.resource))
        {
            var obj = TogglePooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = item.name.ToUppercaseFirst();
            var toggle = obj.GetComponentInChildren<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((value) => {
                if(value) 
                    thing.GetTrait<Storage>().Allow(item.type);
                else
                    thing.GetTrait<Storage>().Disallow(item.type);
            });
        }

        gameObject.SetActive(true);
    }
}

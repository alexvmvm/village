using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StoragePanel : MonoBehaviour
{
    public Game Game;
    public ObjectPooler TogglePooler;

    public void Setup(Thing thing)
    {
        TogglePooler.DeactivateAll();

        foreach(var item in Game.AllThings.Where(t => t.resource))
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

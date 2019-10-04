using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ManualPlacementPanel : MonoBehaviour
{
    public ObjectPooler ButtonPooler;
    public Game Game;

    void Start()
    {
        SetupButtons();
    }

    public void SetupButtons()
    {
         ButtonPooler.DeactivateAll();
        
        foreach(var thing in Game.AllThings)
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.name.ToUppercaseFirst();
            obj.SetActive(true);

            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Game.GetSprite(thing.sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                Game.CurrentType = thing.type;
            });
        }
    }

}

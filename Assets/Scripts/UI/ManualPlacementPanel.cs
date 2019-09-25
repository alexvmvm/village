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

    private List<Thing> _things;

    void Start()
    {
        _things = new List<Thing>();
        foreach(TypeOfThing thingType in Enum.GetValues(typeof(TypeOfThing)))
        {
            _things.Add(Game.Create(thingType));
        }

        SetupButtons();
    }

    public void SetupButtons()
    {
         ButtonPooler.DeactivateAll();
        
        foreach(var thing in _things)
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

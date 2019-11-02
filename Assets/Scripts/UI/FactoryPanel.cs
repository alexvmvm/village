﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village;
using Village.Things;

public class FactoryPanel : MonoBehaviour
{
    public ObjectPooler FactoryLineItem;
    public ObjectPooler CostPooler;
    public GameObject DetailsPanel;
    public Text DetailsDescription;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    public void Setup(Thing thing)
    {
        var factory = thing.GetTrait<Factory>();

        FactoryLineItem.DeactivateAll();

        foreach(var type in thing.GetTrait<Factory>().Produces)
        {
            var lineItemThing = _session.Game.Create(type, 0, 0);
            var obj = FactoryLineItem.GetPooledObject();

            var lineItem = obj.GetComponent<FactoryLineItem>();

            lineItem.OnMouseEnter.RemoveAllListeners();
            lineItem.OnMouseEnter.AddListener(() => {
                OnMouseEnter(lineItemThing);
            });

            lineItem.OnMouseExit.RemoveAllListeners();
            lineItem.OnMouseExit.AddListener(() => {
                OnMouseExit();
            });

            lineItem.Name.text = lineItemThing.name.ToUppercaseFirst();
            lineItem.Image.sprite = Assets.GetSprite(lineItemThing.sprite);
            lineItem.Number.text = $"{factory.GetQueuedCount(type)}";

            lineItem.Increase.onClick.RemoveAllListeners();
            lineItem.Increase.onClick.AddListener(() => {
                factory.AddThingToProduce(type);
                lineItem.Number.text = $"{factory.GetQueuedCount(type)}";
            });

            lineItem.Decrease.onClick.RemoveAllListeners();
            lineItem.Decrease.onClick.AddListener(() => {
                factory.RemoveThingToProduce(type);
                lineItem.Number.text = $"{factory.GetQueuedCount(type)}";
            });

            obj.SetActive(true);
        }   

        gameObject.SetActive(true);
    }

    void OnMouseEnter(Thing thing)
    {
        CostPooler.DeactivateAll();

        foreach(var required in thing.requiredToCraft)
        {
            var obj = CostPooler.GetPooledObject();
            var lineItem = obj.GetComponent<CostLineItem>();
            var lineItemThing = _session.Game.GetThingNotInScene(required);
            lineItem.Name.text = lineItemThing.name.ToUppercaseFirst();
            lineItem.Amount.text = "x1";
            lineItem.Image.sprite = Assets.GetSprite(lineItemThing.sprite);
            obj.SetActive(true);
        }


        DetailsDescription.text = thing.description;

        DetailsPanel.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        DetailsPanel.gameObject.SetActive(false);
    }
}

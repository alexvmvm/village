using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Village;
using Village.Things;
using Village.Things.Config;

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
        var factory = thing.Factory;

        FactoryLineItem.DeactivateAll();

        foreach(var type in thing.Factory.Produces)
        {
            var lineItemThing = Assets.CreateThingConfig(type);
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

            lineItem.Name.text = lineItemThing.Name.ToUppercaseFirst();
            lineItem.Image.sprite = Assets.GetSprite(lineItemThing.Sprite);
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

    void OnMouseEnter(ThingConfig thing)
    {
        CostPooler.DeactivateAll();

        foreach(var required in thing.RequiredToCraft)
        {
            var obj = CostPooler.GetPooledObject();
            var lineItem = obj.GetComponent<CostLineItem>();
            var lineItemThing = _session.Game.ThingConfigs.FirstOrDefault(t => t.TypeOfThing == required);
            lineItem.Name.text = lineItemThing.Name.ToUppercaseFirst();
            lineItem.Amount.text = "x1";
            lineItem.Image.sprite = Assets.GetSprite(lineItemThing.Sprite);
            obj.SetActive(true);
        }


        DetailsDescription.text = thing.Description;

        DetailsPanel.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        DetailsPanel.gameObject.SetActive(false);
    }
}

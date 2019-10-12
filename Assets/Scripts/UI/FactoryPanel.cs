using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryPanel : MonoBehaviour
{
    public Game Game;
    public ObjectPooler FactoryLineItem;

    public void Setup(Thing thing)
    {
        var factory = thing.factory;

        FactoryLineItem.DeactivateAll();

        foreach(var type in thing.factory.Produces)
        {
            var lineItemThing = Game.Create(type);
            var obj = FactoryLineItem.GetPooledObject();
            var lineItem = obj.GetComponent<FactoryLineItem>();
            lineItem.Name.text = lineItemThing.name.ToUppercaseFirst();
            lineItem.Image.sprite = Game.GetSprite(lineItemThing.sprite);
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
}

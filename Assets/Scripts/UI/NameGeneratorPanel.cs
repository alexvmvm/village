using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameGeneratorPanel : MonoBehaviour
{
    public ObjectPooler ObjectPooler;

    void OnEnable()
    {
       Generate();
    }

    public void Generate()
    {
        ObjectPooler.DeactivateAll();

        for(var i = 0; i < 100; i++) 
        {
            var obj = ObjectPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = NameGenerator.GenerateCharacterName();
            obj.SetActive(true);
        }
    }
}

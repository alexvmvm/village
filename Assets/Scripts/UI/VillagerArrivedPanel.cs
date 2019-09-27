using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class VillagerArrivedPanel : MonoBehaviour
{
    public Game Game;
    public Text Text;

    void OnEnable()
    {
        transform.position = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
    }
    
    public void Activate(string name)
    {
        Text.text = string.Format("{0} has arrived in the village. Find them a house to live in.", name);
        gameObject.SetActive(true);
    }
}

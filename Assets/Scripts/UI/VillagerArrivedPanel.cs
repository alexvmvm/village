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
    
    public void ShowMessage(string message)
    {
        Text.text = message;
        gameObject.SetActive(true);
    }
}

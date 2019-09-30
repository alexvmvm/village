using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Text Name;
    public Text Hitpoints;

    public void Setup(Thing thing)
    {
        if(thing == null)
            return;

        Name.text = thing.name.ToUppercaseFirst();
        Hitpoints.text = string.Format("{0}/{1}", thing.hitpoints, 100);
    }
}

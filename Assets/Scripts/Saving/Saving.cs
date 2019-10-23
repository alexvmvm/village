using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

public class Saving : MonoBehaviour
{
    public Game Game;
  

    void Start()
    {
        StartCoroutine(LoadSave());
    }

    IEnumerator LoadSave()
    {
        yield return SaveFiles.LoadCurrentSaveGame(Game);
    }
}

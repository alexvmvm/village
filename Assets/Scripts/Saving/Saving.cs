using System.Collections;
using UnityEngine;
using Village;
using Village.Saving;

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

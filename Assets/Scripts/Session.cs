using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village;
using Village.Saving;

public class Session : MonoBehaviour
{
    public AstarPath AstarPath;
    public string SaveFileName;

    public Game Game;
    public GameCursor Cursor;


    void Start()
    {
        SetupNewGame();
        if(!string.IsNullOrEmpty(SaveFileName))
            LoadGame();
    }

    [BitStrap.Button]
    public void SetupNewGame()
    {
        StartCoroutine(SetupGame());
    }

    IEnumerator SetupGame()
    {
        yield return StartCoroutine(Game.LoadNewGame());
    }

    [BitStrap.Button]
    public void LoadGame()
    {
        StartCoroutine(LoadGameC());
    }

    IEnumerator LoadGameC()
    {
        var gameSave = SaveFiles.LoadGameFromName(SaveFileName);
        if(gameSave != null)
        {
            yield return StartCoroutine(Game.LoadGame(gameSave));
        }
        else
        {
            Debug.Log($"Unable to find save: ${SaveFileName}");
        }
    }

    [BitStrap.Button]
    public void SaveGame()
    {
        SaveFiles.SaveGameWithName(Game, SaveFileName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static bool IsInMacOS
    {
        get
        {
            return UnityEngine.SystemInfo.operatingSystem.IndexOf("MacOS") != -1;
        }
    }

    public static bool IsInWinOS
    {
        get
        {
            return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1;
        }
    }

    [BitStrap.Button]
    public void OpenDirectory()
    {
        if (IsInWinOS)
            System.Diagnostics.Process.Start ("explorer.exe", Application.persistentDataPath.Replace (@"/", @"\"));
        else if (IsInMacOS) 
        {
            System.Diagnostics.Process.Start("open", Application.persistentDataPath);
            Debug.Log (Application.persistentDataPath);
        }
            
    }
}

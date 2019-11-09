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
    public GameCursor Cursor { get { return _cursor; } }
    private GameCursor _cursor;

    public void Awake()
    {
        _cursor = new GameCursor(Game);
    }

    void Start()
    {
        SetupNewGame();
        if(!string.IsNullOrEmpty(SaveFileName))
            LoadGame();
    }

    [BitStrap.Button]
    public void SetupNewGame()
    {
        Game.Generate();
    }

    [BitStrap.Button]
    public void LoadGame()
    {
        var gameSave = SaveFiles.LoadGameFromName(SaveFileName);
        if(gameSave != null)
        {
            Game.Clear();
            Game.FromSaveObj(gameSave);
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

    void Update()
    {
        Game.Update();
        _cursor.Update();
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

    void OnDrawGizmos()
    {
        if(_cursor != null)
            _cursor.DrawGizmos();
        if(Game != null)
            Game.DrawGizmos();
    }
}

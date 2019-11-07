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

    public Game Game { get { return _game; } }    
    private Game _game;
    public GameCursor Cursor { get { return _cursor; } }
    private GameCursor _cursor;

    public void Awake()
    {
        _game = new Game(AstarPath, new Vector2Int(50, 50));
        _cursor = new GameCursor(_game);
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
        _game.Start();
        _game.Generate();
    }

    [BitStrap.Button]
    public void LoadGame()
    {
        var gameSave = SaveFiles.LoadGameFromName(SaveFileName);
        if(gameSave != null)
        {
            _game.Clear();
            _game.Start();
            _game.FromSaveObj(gameSave);
        }
        else
        {
            Debug.Log($"Unable to find save: ${SaveFileName}");
        }
    }

    [BitStrap.Button]
    public void SaveGame()
    {
        SaveFiles.SaveGameWithName(_game, SaveFileName);
    }

    void Update()
    {
        _game.Update();
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
        if(_game != null)
            _game.DrawGizmos();
    }
}

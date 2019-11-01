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
    public Text LoadingText;
    public Text ErrorText;
    public GameObject LoadingPanel;

    public Game Game { get { return _game; } }    
    private Game _game;
    public GameCursor Cursor { get { return _cursor; } }
    private GameCursor _cursor;
    public ZoneGraph ZoneGraph { get { return _zoneGraph; } }    
    private ZoneGraph _zoneGraph;

    public void Awake()
    {
        _game = new Game(AstarPath, new Vector2Int(50, 50), CreateGameObject);
        _zoneGraph = new ZoneGraph(_game);
        _cursor = new GameCursor(_game);
    }

    void Start()
    {
        _game.Start();

        if(SaveFiles.IsNewGame())
        {
            Debug.Log("New Game, generating...");
            _game.Generate();
            LoadingPanel.SetActive(false);
        } 
        else
        {
            Debug.Log("Loading from save...");

            try 
            {
                _game.FromSaveObj(SaveFiles.GetCurrentSetGameSave());
            } 
            catch(Exception error) 
            {
                Debug.Log("Failed to load save");

                LoadingText.text = "Failed to load save";
                ErrorText.text = error.Message;
                
                SaveFiles.ClearSetSave();
            }

        }

        _zoneGraph.Start();
    }

    GameObject CreateGameObject()
    {
        var obj = MonoBehaviour.Instantiate(Assets.GetPrefab("Thing"));
        obj.SetActive(true);
        return obj;
    }

    [BitStrap.Button]
    public void Save()
    {
        SaveFiles.SaveGame(_game, @"C:\Users\Alex\AppData\LocalLow\Unity\save.xml");
    }

    [BitStrap.Button]
    public void Load()
    {
        var saveObject = SaveFiles.LoadGame(@"C:\Users\Alex\AppData\LocalLow\Unity\save.xml");
        _game.FromSaveObj(saveObject);
    }

    void Update()
    {
        _game.Update();
        _zoneGraph.Update();
        _cursor.Update();
    }

    void OnDrawGizmos()
    {
        if(_cursor != null)
            _cursor.DrawGizmos();
        if(_zoneGraph != null)
            _zoneGraph.DrawGizmos();
    }
}

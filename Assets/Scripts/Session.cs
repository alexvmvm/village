using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour
{
    public AstarPath AstarPath;
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
        _zoneGraph.Start();
    }

    GameObject CreateGameObject()
    {
        var obj = MonoBehaviour.Instantiate(Assets.GetPrefab("Thing"));
        obj.SetActive(true);
        return obj;
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

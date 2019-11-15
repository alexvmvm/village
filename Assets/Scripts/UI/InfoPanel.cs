using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village.Things;
using Village;
using System.Linq;

public class InfoPanel : MonoBehaviour
{
    public ObjectPooler TextPooler;
    private Game _game;
    private GameCursor _gameCursor;

    void Awake()
    {
        _game = FindObjectOfType<Game>();
        _gameCursor = FindObjectOfType<GameCursor>();
    }

    void OnEnable()
    {
        _gameCursor.OnCursorMoved += Setup;
    }

    void OnDisable()
    {
        _gameCursor.OnCursorMoved -= Setup;
    }

    public void Setup(Vector2Int position)
    {
        TextPooler.DeactivateAll();

        var loose = _game.QueryLooseThings().Where(t => t.Position == position).FirstOrDefault();
        if(loose != null)
        {
            BuildList(loose);
            return;
        }

        var floor = _game.GetThingOnFloor(position);
        if(floor != null)
        {
            BuildList(floor);
        }
    }

    void BuildList(Thing thing)
    {
        AddTextLine(thing.Config.Name.ToUppercaseFirst());
        AddTextLine($"{thing.Hitpoints}");

        switch(thing.Config.TypeOfThing)
        {
            case TypeOfThing.Blueprint:
                AddTextLine("<color=black>Blueprint</color>");
                AddTextLine($"Builds: {thing.Builds.ToString()}");
                AddTextLine($"Requires: {thing.Requires.ToString()}");
            break;
            case TypeOfThing.ClayForge:
            case TypeOfThing.Workbench:
                AddTextLine("<color=black>Factory</color>");
                AddTextLine($"Currently Producing: {thing.Factory.CurrentlyProducing()}");         
            break;
            case TypeOfThing.Storage:
                AddTextLine("<color=black>Storage</color>");
                foreach(var stored in thing.Storage.Stored)
                {
                    AddTextLine($"{stored.Config.TypeOfThing}: {stored.Hitpoints}");   
                }
            break;
        }
    }

    void AddTextLine(string text)
    {
        var obj = TextPooler.GetPooledObject();
        obj.GetComponent<Text>().text = text;
        obj.SetActive(true);
    }
}

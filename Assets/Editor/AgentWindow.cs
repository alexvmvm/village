using UnityEngine;
using UnityEditor;
using Village.Things;
using Village.Things.Config;
using System.Linq;
using Village.AI;
using System.Collections.Generic;

public class AgentWindow : EditorWindow
{
    [MenuItem("Window/Agents")]
    public static void ShowWindow()
    {
        GetWindow<AgentWindow>(false, "Agent Window", true);
    }

    private Vector2 _actionsScrollView = Vector2.zero;
    private GameObject _selected;
    private GOAPAction _selectedAction;
    private bool _selectedActionPossible;
    private GUIStyle _header;

    void OnGUI()
    {
        var agents = GameObject.FindObjectsOfType<Thing>().Where(t => t.Config.Agent != AgentConfig.None).ToArray();
        var buttonWidth = 200;
        
        if(_selected == null && agents.Count() > 0)
        {
            _selected = agents.First().gameObject;
        }

        for(var i = 0; i < agents.Length; i++)
        {
            if(GUI.Button(new Rect(10 + i * buttonWidth, 10, buttonWidth, 20), agents[i].name))
            {
                _selected = agents[i].gameObject;
                _selectedAction = null;
            }
        }

        var yOffset = 40;

        if(_selected != null)
        {
            DrawAgent(10, yOffset, _selected);
        }

        if(_selectedAction != null)
        {
            DrawActionDetail(520, yOffset, _selectedAction);
        }
    }

    void SetSelectedAction(GOAPAction action)
    {
        _selectedAction = action;
        _selectedActionPossible = action.IsPossibleToPerform();
    }

    void DrawAgent(float x, float y, GameObject gameObject)
    {
        var agent = gameObject.GetComponent<Agent>();
        var actions = agent.GetActions().OrderBy(a => a.ToString()).ToArray();
        var currentY = y;

        // world state
        var worldStateRect = DrawConditions(x, y, 500, agent.WorldState, "World State");
        currentY += worldStateRect.height + 10;
        
        // goals
        var goalStateRect = DrawConditions(x, currentY, 500, agent.GoalState, "Goal State");
        currentY += goalStateRect.height + 10;

        // current action
        if(agent.CurentAction != null)
        {
            var currentRect = DrawAction(x, currentY, agent.CurentAction);
            currentY += currentRect.height + 10;
        }

        // current queued
        var queued = agent.Queued;
        for(var i = 0; i < queued.Length; i++)
        {
            GUI.Label(new Rect(x, currentY, 500, 20), $"-> {queued[i].ToString()}");
            currentY += 20;
        }        

        _actionsScrollView = GUI.BeginScrollView(
            new Rect(10, currentY, 500, position.height - (currentY - y) - 20), 
            _actionsScrollView, 
            new Rect(10, currentY, 10000, 15000));

        for(var i = 0; i < actions.Count(); i++)
        {
            var action = actions[i];
            var rect = DrawAction(x, currentY, action);
            currentY += rect.height;
        }

        GUI.EndScrollView();
    }

    Rect DrawActionDetail(float x, float y, GOAPAction selectedAction)
    {
        var rect = new Rect(x, y, 500, 20);

        GUI.Box(rect, selectedAction.ToString());

        if(GUI.Button(new Rect(x, y + 20, 500, 20), "IsPossibleToPerform"))
        {
            _selectedActionPossible = selectedAction.IsPossibleToPerform();
        }
        
        GUI.Box(new Rect(x, y + 40, 500, 20), _selectedActionPossible ? "yes ✔" : "no ✘");

        return rect;
    }

    Rect DrawAction(float x, float y, GOAPAction action)
    {
        //var rect = new Rect(x, y, 600, 1000);
        
        var yOffset = 20;
        var rect = new Rect(x, y, 500, yOffset);

        GUI.Box(rect, $"{action.ToString()}");

        // draw preconditions
        var preconditons = DrawConditions(rect.x, rect.y + yOffset, rect.width/2, action.Preconditions, "Preconditions");
        var effects = DrawConditions(rect.x + preconditons.width, rect.y + yOffset, rect.width/2, action.Effects, "Effects");

        rect.height = Mathf.Max(preconditons.height, effects.height) + yOffset;

        if(GUI.Button(new Rect(x, y + rect.height, 500, 20), "check"))
        {
            SetSelectedAction(action);
        }
        rect.height += 20;

        return rect;
    }

    Rect DrawConditions(float x, float y, float width, Dictionary<string, object> conditions, string name)
    {
        var conditionHeight = 20;
        var totalHeight = conditions.Count() * conditionHeight;
        var rect = new Rect(x, y, width, totalHeight + 20);
        GUI.Box(rect, name);

        var yPos = y + 20;
        for(var i = 0; i < conditions.Count(); i++)
        {
            var actionRect = new Rect(rect.x, yPos, width, conditionHeight);
            var p = conditions.ElementAt(i);
            GUI.Box(actionRect, $"{p.Key}: {p.Value}");
            yPos += conditionHeight;
        }

        return rect;
    }

    // Rect DrawCropView(float x, float y, ThingConfig[] crops)
    // {
    //     var thingHeight = 20;
    //     var thingWidth = 100;
    //     var offsetTop = 30;

    //     var rect = new Rect(x, y, thingWidth * 3, crops.Length * thingHeight + offsetTop);
       

    //     GUI.Box(rect, "Crop View\n Requires -> Crop -> Produces");

    //     for(var i = 0; i < crops.Length; i++)
    //     {
    //         var yPos = y + i * thingHeight + offsetTop;
    //         DrawThing(x, yPos, thingWidth, thingHeight, crops[i].ConstructionConfig != null ? crops[i].ConstructionConfig.Requires : TypeOfThing.None);
    //         DrawThing(x + thingWidth, yPos, thingWidth, thingHeight, crops[i]);
    //         DrawThing(x +  2 * thingWidth, yPos, thingWidth, thingHeight, crops[i].CropConfig.Produces);
            
    //     }

    //     return rect;
    // }

    // Rect DrawCollection(float x, float y, ThingConfig[] things, string name)
    // {
    //     var margin = 10;
    //     var thingWidth = 100;
    //     var thingHeight = 20;
    //     var height = things.Count() * (thingHeight + margin) + 2 * margin;
    //     var cWidth = thingWidth + 2 * margin;
    //     DrawColumn(x, y, cWidth, height, name);
    //     for(var i = 0; i < things.Length; i++)
    //     {
    //         var yPos = y + i * (thingHeight + margin) + margin * 2;
    //         DrawThing(x + margin, yPos, thingWidth, thingHeight, things[i]);
    //     }

    //     return new Rect(x, y, cWidth, height);
    // }

    // void DrawThing(float x, float y, float width, float height, ThingConfig thing)
    // {
    //     DrawThing(x, y, width, height, thing.TypeOfThing);
    // }

    // void DrawThing(float x, float y, float width, float height, TypeOfThing thing)
    // {
    //     GUI.Box(new Rect(x, y, width, height), thing.ToString());
    // }

    // void DrawColumn(float x, float y, float width, float height, string name)
    // {
    //     var style = new GUIStyle();
    //     GUI.Box(new Rect(x, y, width, height), name);
    // }
}
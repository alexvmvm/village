using UnityEngine;
using UnityEditor;
using Village.Things.Config;
using System.Linq;

public class GameWindow : EditorWindow
{
    [MenuItem("Window/Game Tree")]
    public static void ShowWindow()
    {
        GetWindow<GameWindow>(false, "Game Window", true);
    }

    private Vector2 _scrollPosition = Vector2.zero;

    void OnGUI()
    {
   
        var configs = Assets.GetAllThingConfigs();
        var resourcesRect = DrawCollection(10, 10, configs.Where(t => t.Resource).ToArray(), "Resources");
        var cropRect = DrawCollection(resourcesRect.xMax + 10, 10, configs.Where(t => t.CropConfig != null).ToArray(), "Crops");
        var factoriesRect = DrawCollection(cropRect.xMax + 10, 10, configs.Where(t => t.FactoryConfig != null).ToArray(), "Factories");
        var toolsRect = DrawCollection(factoriesRect.xMax + 10, 10, configs.Where(t => t.Tool).ToArray(), "Tools");
        var blueprintsRect = DrawCollection(toolsRect.xMax + 10, 10, configs.Where(t => t.ConstructionConfig != null).ToArray(), "Blueprints");
        var agentsRect = DrawCollection(blueprintsRect.xMax + 10, 10, configs.Where(t => t.Agent != AgentConfig.None).ToArray(), "Agents");

        var cropView = DrawCropView(agentsRect.xMax + 10, 10, configs.Where(t => t.CropConfig != null).ToArray());

 
    }

    Rect DrawCropView(float x, float y, ThingConfig[] crops)
    {
        var thingHeight = 20;
        var thingWidth = 100;
        var offsetTop = 30;

        var rect = new Rect(x, y, thingWidth * 3, crops.Length * thingHeight + offsetTop);
       

        GUI.Box(rect, "Crop View\n Requires -> Crop -> Produces");

        for(var i = 0; i < crops.Length; i++)
        {
            var yPos = y + i * thingHeight + offsetTop;
            DrawThing(x, yPos, thingWidth, thingHeight, crops[i].ConstructionConfig != null ? crops[i].ConstructionConfig.Requires : TypeOfThing.None);
            DrawThing(x + thingWidth, yPos, thingWidth, thingHeight, crops[i]);
            DrawThing(x +  2 * thingWidth, yPos, thingWidth, thingHeight, crops[i].CropConfig.Produces);
            
        }

        return rect;
    }

    Rect DrawCollection(float x, float y, ThingConfig[] things, string name)
    {
        var margin = 10;
        var thingWidth = 100;
        var thingHeight = 20;
        var height = things.Count() * (thingHeight + margin) + 2 * margin;
        var cWidth = thingWidth + 2 * margin;
        DrawColumn(x, y, cWidth, height, name);
        for(var i = 0; i < things.Length; i++)
        {
            var yPos = y + i * (thingHeight + margin) + margin * 2;
            DrawThing(x + margin, yPos, thingWidth, thingHeight, things[i]);
        }

        return new Rect(x, y, cWidth, height);
    }

    void DrawThing(float x, float y, float width, float height, ThingConfig thing)
    {
        DrawThing(x, y, width, height, thing.TypeOfThing);
    }

    void DrawThing(float x, float y, float width, float height, TypeOfThing thing)
    {
        GUI.Box(new Rect(x, y, width, height), thing.ToString());
    }

    void DrawColumn(float x, float y, float width, float height, string name)
    {
        var style = new GUIStyle();
        GUI.Box(new Rect(x, y, width, height), name);
    }
}
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Village;
using Village.Things;

public class BuildingPanel : MonoBehaviour
{
    public ObjectPooler GroupPooler;
    public ObjectPooler ButtonPooler;
    private Game _game;    
    private GameCursor _cursor;
    void Awake()
    {
        _game = FindObjectOfType<Game>();
        _cursor = FindObjectOfType<GameCursor>();

        GroupPooler.DeactivateAll();
        foreach(ConstructionGroup group in Enum.GetValues(typeof(ConstructionGroup)))
        {
            var obj = GroupPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = group.ToString().ToUppercaseFirst();
            obj.SetActive(true);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                SetupGroupButtons(group);
            });
        }
    }   

    public void SetupGroupButtons(ConstructionGroup group)
    {
        ButtonPooler.DeactivateAll();
        
        var things = _game.ThingConfigs.Where(t => t.ConstructionConfig != null && t.ConstructionConfig.Group == group).ToList();

        foreach(var thing in things)
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.Name.ToUppercaseFirst();
            obj.SetActive(true);

            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Assets.GetSprite(thing.Sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                _cursor.SetCursor(thing.TypeOfThing, true);
            });
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            gameObject.SetActive(false);
        }
    }
}

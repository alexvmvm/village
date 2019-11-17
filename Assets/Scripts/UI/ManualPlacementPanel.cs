using UnityEngine;
using UnityEngine.UI;
using Village;
using System.Linq;

public class ManualPlacementPanel : MonoBehaviour
{
    public ObjectPooler ButtonPooler;
    private Game _game;
    private GameCursor _cursor;

    void Awake()
    {
        _game = FindObjectOfType<Game>();
        _cursor = FindObjectOfType<GameCursor>();
    }

    void Start()
    {
        SetupButtons();
    }

    public void SetupButtons()
    {
         ButtonPooler.DeactivateAll();
        
        foreach(var thing in _game.ThingConfigs.OrderBy(t => t.TypeOfThing.ToString()))
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.Name.ToUppercaseFirst();
            obj.SetActive(true);

            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Assets.GetSprite(thing.Sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                _cursor.SetCursor(thing.TypeOfThing, false);
            });
        }
    }

}

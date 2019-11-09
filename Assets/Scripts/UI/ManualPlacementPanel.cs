using UnityEngine;
using UnityEngine.UI;

public class ManualPlacementPanel : MonoBehaviour
{
    public ObjectPooler ButtonPooler;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    void Start()
    {
        SetupButtons();
    }

    public void SetupButtons()
    {
         ButtonPooler.DeactivateAll();
        
        foreach(var thing in _session.Game.ThingConfigs)
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.Name.ToUppercaseFirst();
            obj.SetActive(true);

            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Assets.GetSprite(thing.Sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                _session.Cursor.CurrentType = thing.TypeOfThing;
            });
        }
    }

}

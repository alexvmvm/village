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
        
        foreach(var thing in _session.Game.QueryThingsNotInScene())
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.name.ToUppercaseFirst();
            obj.SetActive(true);

            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Assets.GetSprite(thing.sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                _session.Cursor.CurrentType = thing.Config.TypeOfThing;
            });
        }
    }

}

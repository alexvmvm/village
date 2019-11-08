using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Village.Things;

public class BuildingPanel : MonoBehaviour
{
    public ObjectPooler GroupPooler;
    public ObjectPooler ButtonPooler;
    private Session _session;
    
    void Awake()
    {
        _session = FindObjectOfType<Session>();

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
        
        foreach(var thing in _session.Game.QueryThingsNotInScene().Where(t => t.construction != null && t.construction.Group == group))
        {
            var obj = ButtonPooler.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = thing.name.ToUppercaseFirst();
            obj.SetActive(true);

            var thingToBuild = _session.Game.QueryThingsNotInScene().Where(t => t.Config.TypeOfThing == thing.construction.BuildType).FirstOrDefault();
            obj.transform.GetComponentInChildrenExcludingParent<Image>().sprite = Assets.GetSprite(thingToBuild.sprite);

            var button = obj.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                _session.Cursor.CurrentType = thing.Config.TypeOfThing;
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

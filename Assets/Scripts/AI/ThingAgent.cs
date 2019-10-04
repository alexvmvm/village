using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThingAgent : Agent
{
    protected Thing _thing;
    private TextMesh _textMesh;
    private GameObject _labelObj;

    public ThingAgent(Game game, Thing thing) : base(game)
    {
        _thing = thing;

        /*
            Name Label 
        */
        _labelObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Label"));    
        _labelObj.transform.SetParent(_thing.transform);
        _labelObj.transform.localPosition = new Vector3(0, -0.8f, 0);
        _labelObj.GetComponentInChildren<MeshRenderer>().sortingOrder = (int)SortingOrders.Labels;
        

        _textMesh = _labelObj.GetComponentInChildren<TextMesh>();
    }

    public void SetLabel(string label)
    {
        _textMesh.text = label;
    }

    public void ShowLabel(bool show)
    {
        _labelObj.SetActive(show);
    }

    public override abstract void ActionCompleted(GOAPAction action);
    public override abstract Dictionary<string, object> GetWorldState();
    public override abstract Dictionary<string, object> GetGoal();

    public override void Update()
    {
        base.Update();

        if(_labelObj.activeSelf)
            _textMesh.transform.rotation = Quaternion.identity;
    }
}

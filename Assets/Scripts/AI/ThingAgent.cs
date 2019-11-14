using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Village.Things;

namespace Village.AI
{

    public abstract class ThingAgent : Agent
    {
        private TextMesh _textMesh;
        private GameObject _labelObj;

        public override void Awake()
        {
            base.Awake();


            /*
                Name Label 
            */
            _labelObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Label"));
            _labelObj.transform.SetParent(transform);
            _labelObj.transform.localPosition = new Vector3(0, -0.8f, 0);
            _labelObj.GetComponentInChildren<MeshRenderer>().sortingOrder = (int)SortingOrders.Labels;


            _textMesh = _labelObj.GetComponentInChildren<TextMesh>();
            _textMesh.alignment = TextAlignment.Center;
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
        public override abstract void GetWorldState(Dictionary<string, object> world);
        public override abstract void GetGoalState(Dictionary<string, object> goal);

        public override void Update()
        {
            base.Update();

            if (_labelObj.activeSelf)
                _textMesh.transform.rotation = Quaternion.identity;
        }
    }

}

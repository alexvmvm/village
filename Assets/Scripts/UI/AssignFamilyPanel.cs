using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AssignFamilyPanel : MonoBehaviour
{
    public Dropdown Dropdown;

    private VillageManager _villageManager;
    private Thing _thing;
    private List<string> _names;

    void Awake()
    {
        _villageManager = FindObjectOfType<VillageManager>();
    }

    public void Setup(Thing thing)
    {
        _thing = thing;
        _names = _villageManager.GetDistinctSurnames().ToList();
        _names.Add("None");
        _names.Reverse();

        Dropdown.ClearOptions();
        Dropdown.AddOptions(_names.Select(n => n == "None" ? n : n + " Family").ToList());
        var index = !string.IsNullOrEmpty(_thing.belongsToFamily) && _names.IndexOf(_thing.belongsToFamily) > 0 ?
            _names.IndexOf(_thing.belongsToFamily) : 0;
            
        Dropdown.value = index;

        Dropdown.onValueChanged.AddListener((value) => {
            _thing.belongsToFamily = value > 0 ? _names[value] : "";
        });


        gameObject.SetActive(true);
    }
}

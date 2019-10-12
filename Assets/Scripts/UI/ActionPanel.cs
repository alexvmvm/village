using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button AssignFamily;
    public Button FactoryButton;

    [Header("Panels")]
    public AssignFamilyPanel AssignFamilyPanel;
    public FactoryPanel FactoryPanel;

    public void Setup(Thing thing)
    {
        Clear();

        if(thing == null)
            return;

        if(thing.assignToFamily)
        {
            AssignFamily.gameObject.SetActive(true);
            AssignFamily.onClick.RemoveAllListeners();
            AssignFamily.onClick.AddListener(() => {
                AssignFamilyPanel.Setup(thing);
            });
        }

        if(thing.factory != null)
        {
            FactoryButton.gameObject.SetActive(true);
            FactoryButton.onClick.RemoveAllListeners();
            FactoryButton.onClick.AddListener(() => {
                FactoryPanel.Setup(thing);
            });
        }
    }

    public void Clear()
    {
        AssignFamily.onClick.RemoveAllListeners();
        FactoryButton.onClick.RemoveAllListeners();

        AssignFamily.gameObject.SetActive(false);
        FactoryButton.gameObject.SetActive(false);
    }
}

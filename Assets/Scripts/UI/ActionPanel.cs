﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village.Things;

public class ActionPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button AssignFamily;
    public Button FactoryButton;
    public Button StorageButton;

    [Header("Panels")]
    public AssignFamilyPanel AssignFamilyPanel;
    public FactoryPanel FactoryPanel;
    public StoragePanel StoragePanel;

    public void Setup(Thing thing)
    {
        Clear();

        if(thing == null)
            return;

        if(thing.Config.AssignToFamily)
        {
            AssignFamily.gameObject.SetActive(true);
            AssignFamily.onClick.RemoveAllListeners();
            AssignFamily.onClick.AddListener(() => {
                AssignFamilyPanel.Setup(thing);
            });
        }

        if(thing.Factory != null)
        {
            FactoryButton.gameObject.SetActive(true);
            FactoryButton.onClick.RemoveAllListeners();
            FactoryButton.onClick.AddListener(() => {
                FactoryPanel.Setup(thing);
            });
        }

        if(thing.Storage != null)
        {
            StorageButton.gameObject.SetActive(true);
            StorageButton.onClick.RemoveAllListeners();
            StorageButton.onClick.AddListener(() => {
                StoragePanel.Setup(thing);
            });
        }
    }

    public void Clear()
    {
        AssignFamily.onClick.RemoveAllListeners();
        FactoryButton.onClick.RemoveAllListeners();
        StorageButton.onClick.RemoveAllListeners();

        AssignFamily.gameObject.SetActive(false);
        FactoryButton.gameObject.SetActive(false);
        StorageButton.gameObject.SetActive(false);
    }
}

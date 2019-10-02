﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button AssignFamily;

    [Header("Panels")]
    public AssignFamilyPanel AssignFamilyPanel;

    public void Setup(Thing thing)
    {
        Clear();

        if(thing == null)
            return;

        switch(thing.type)
        {
            case TypeOfThing.FamilyChest:
                AssignFamily.gameObject.SetActive(true);
                AssignFamily.onClick.RemoveAllListeners();
                AssignFamily.onClick.AddListener(() => {
                    AssignFamilyPanel.Setup(thing);
                });
            break;
        }
    }

    public void Clear()
    {
        AssignFamily.onClick.RemoveAllListeners();

        AssignFamily.gameObject.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public ObjectPooler LoadPool;
    public Button Load;
    public Button Delete;

    void OnEnable()
    {
        Load.interactable = false;
        Delete.interactable = false;
        RefreshSaveFiles();
    }

    void RefreshSaveFiles()
    {
        LoadPool.DeactivateAll();
        foreach(var save in SaveFiles.GetSaves())
        {
            var obj = LoadPool.GetPooledObject();
            var text = obj.GetComponentInChildren<Text>();
            text.text = save.FileNameWithoutExt;
            var btn = obj.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                SelectSave(save);
            });
            obj.SetActive(true);
        }
    }

    void SelectSave(SaveFile saveFile)
    {
        Load.interactable = true;
        Delete.interactable = true;

        Delete.onClick.RemoveAllListeners();
        Delete.onClick.AddListener(() => {
            DeleteSave(saveFile);
        });
    }

    void DeleteSave(SaveFile save)
    {
        SaveFiles.DeleteSave(save);
        RefreshSaveFiles();
    }

    void OnDisable()
    {

    }
}

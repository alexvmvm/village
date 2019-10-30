using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public ObjectPooler LoadPool;

    void OnEnable()
    {
        RefreshSaveFiles();
        PlayerPrefs.SetString("SaveFilePath", "");
    }

    void ResetAllButtonColors()
    {
        foreach(var obj in LoadPool.GetActiveObjects())
        {
            obj.GetComponentInChildren<Image>().color = Color.white;
        }
    }

    void RefreshSaveFiles()
    {
        LoadPool.DeactivateAll();
        foreach(var save in SaveFiles.GetSaves())
        {
            var obj = LoadPool.GetPooledObject();
            var loadingLineItem = obj.GetComponent<LoadingLineItem>();

            loadingLineItem.Text.text = save.FileNameWithoutExt;

            loadingLineItem.Load.onClick.RemoveAllListeners();
            loadingLineItem.Load.onClick.AddListener(() => {
                Debug.Log("Load");
            });

            loadingLineItem.Delete.onClick.RemoveAllListeners();
            loadingLineItem.Delete.onClick.AddListener(() => {
                Debug.Log("Delete");
            });

            obj.SetActive(true);
        }
    }

    void SelectSave(SaveFile saveFile)
    {
        // Load.interactable = true;
        // Delete.interactable = true;
        
        // Load.onClick.RemoveAllListeners();
        // Load.onClick.AddListener(() => {
        //     SaveFiles.LoadSave(saveFile);
        // });

        // Delete.onClick.RemoveAllListeners();
        // Delete.onClick.AddListener(() => {
        //     DeleteSave(saveFile);
        // });
    }

    void LoadSave(string saveFilePath)
    {
        PlayerPrefs.SetString("SaveFilePath", saveFilePath);
        PlayerPrefs.SetInt("NewGame", 0);
        SceneManager.LoadScene("main");
    }

    void DeleteSave(SaveFile save)
    {
        SaveFiles.DeleteSave(save);
        RefreshSaveFiles();
        ResetAllButtonColors();
    }

    void OnDisable()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Village.Saving;

public class GameMenuPanel : MonoBehaviour
{
    
    public Button Save;
    public InputField Input;
    public ObjectPooler Load;
    public ObjectPooler Delete;
    private Session _session;

    void Awake()
    {
        _session = FindObjectOfType<Session>();
    }

    void Start()
    {
        Save.onClick.AddListener(() => {
            if(!string.IsNullOrEmpty(Input.text))
            {
                _session.SaveFileName = Input.text;
                _session.SaveGame();
                RefreshLoadSaves();
                RefreshDeleteSaves();
            }
        });
    }

    void OnEnable()
    {
        Refresh();
        Input.text = _session.SaveFileName;
    }

    void Refresh()
    {
        RefreshLoadSaves();
        RefreshDeleteSaves();
    }

    void RefreshLoadSaves()
    {
        Load.DeactivateAll();
        foreach(var saveFile in SaveFiles.GetSaves())
        {
            var obj = Load.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = saveFile.FileNameWithoutExt;
            var btn = obj.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                _session.SaveFileName = saveFile.FileNameWithoutExt;
                _session.LoadGame();
            });
            obj.SetActive(true);
        }
    }

    void RefreshDeleteSaves()
    {
        Delete.DeactivateAll();
        foreach(var saveFile in SaveFiles.GetSaves())
        {
            var obj = Delete.GetPooledObject();
            obj.GetComponentInChildren<Text>().text = saveFile.FileNameWithoutExt;
            var btn = obj.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                SaveFiles.DeleteSave(saveFile);
                Refresh();
            });
            obj.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGamePanel : MonoBehaviour
{
    public InputField InputField;
    public Button Create;

    public void CreateGame()
    {
        if(!string.IsNullOrEmpty(InputField.text) && !SaveFiles.SaveExists(InputField.text))
        {
            var saveFile = SaveFiles.CreateNewSave(InputField.text);
            SaveFiles.LoadSave(saveFile);
        }
    }
}

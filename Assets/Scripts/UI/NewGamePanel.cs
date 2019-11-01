using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Village.Saving;

public class NewGamePanel : MonoBehaviour
{
    public InputField InputField;
    public Button Create;

    public void CreateGame()
    {
        InputField.gameObject.SetActive(false);
        InputField.gameObject.SetActive(true);

        var text = InputField.text;

        if(!string.IsNullOrEmpty(text))
        {   
            SaveFiles.SetSaveAndLoad(InputField.text, true);
        }
        
    }
}

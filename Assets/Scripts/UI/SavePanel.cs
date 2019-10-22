using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePanel : MonoBehaviour
{
    private Saving _saving;

    void Awake()
    {
        _saving = FindObjectOfType<Saving>();
    }

    public void Save()
    {
        if(_saving != null)
        {
            _saving.SaveCurrentGame();
        }
    }
}

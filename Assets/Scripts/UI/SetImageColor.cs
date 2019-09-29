using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImageColor : MonoBehaviour
{
    public Color Normal = Color.white;
    public Color Selected = Color.green;
    public bool SelectedOnStart;
    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Start()
    {
        if(SelectedOnStart)
            SetSelected(true);
    }

    public void SetSelected(bool selected)
    {
        _image.color = selected ? Selected : Normal;
    }
}

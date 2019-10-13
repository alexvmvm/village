using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FactoryLineItem : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    public Button Increase;
    public Button Decrease;

    public Text Number;
    public Text Name;
    public Image Image;
    public UnityEvent OnMouseEnter;
    public UnityEvent OnMouseExit;

    private Image _row;

    void Awake()
    {
        _row = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit.Invoke();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOverUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool MouseOverElement;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOverElement = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverElement = false;
    }
}

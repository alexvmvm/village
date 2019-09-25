using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _dragging;
    private Vector2 _offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
        
        _offset = transform.position - Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _dragging = false;
    }

    void Update()
    {
        if (_dragging) {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + _offset;
        }
    }
}

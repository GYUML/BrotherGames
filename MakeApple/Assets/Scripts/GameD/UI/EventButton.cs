using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent onClick;
    public UnityEvent onDown;
    public UnityEvent onUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Action<PointerEventData> onPointerDownEvent;
    Action<PointerEventData> onPointerUpEvent;

    public void SetPointerDownEvent(Action<PointerEventData> onEvent)
    {
        onPointerDownEvent = onEvent;
    }

    public void SetPointerUpEvent(Action<PointerEventData> onEvent)
    {
        onPointerUpEvent = onEvent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownEvent?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpEvent?.Invoke(eventData);
    }
}

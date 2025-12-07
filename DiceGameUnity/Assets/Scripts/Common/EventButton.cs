using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image image;

    Action<PointerEventData> onPointerDownEvent;
    Action<PointerEventData> onPointerUpEvent;

    public bool Enable
    {
        get { return enable; }
        set 
        { 
            enable = value;
            var color = image.color;
            color.a = enable ? 1f : 0.5f;
            image.color = color;
        }
    }

    bool enable;

    private void Start()
    {
        Enable = true;
    }

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
        if (enable)
            onPointerDownEvent?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (enable)
            onPointerUpEvent?.Invoke(eventData);
    }
}

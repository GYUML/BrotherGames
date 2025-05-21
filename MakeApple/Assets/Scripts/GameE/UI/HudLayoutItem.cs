using System;
using UnityEngine;

public class HudLayoutItem : MonoBehaviour
{
    Camera mainCamera;
    RectTransform rectTransform;
    RectTransform parentRectTransform;

    Transform target;
    Vector3 offset = Vector3.up;
    Action<HudLayoutItem> onTargetRemoved;

    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (target != null && target.gameObject.activeInHierarchy)
            rectTransform.anchoredPosition = WorldToAnchored(target.position + offset, parentRectTransform);
        else
            onTargetRemoved?.Invoke(this);
    }

    public void Init(Transform target, float offsetY, Action<HudLayoutItem> onTargetRemoved)
    {
        this.target = target;
        offset.y = offsetY;
        this.onTargetRemoved = onTargetRemoved;
    }

    public Transform GetTarget()
    {
        return target;
    }

    Vector2 WorldToAnchored(Vector3 worldPos, RectTransform parentRect)
    {
        var screenPoint = mainCamera.WorldToScreenPoint(worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out var anchoredPos))
            return anchoredPos;

        return Vector2.zero;
    }
}

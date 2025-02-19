using System;
using UnityEngine;

public class RectSizeListener : MonoBehaviour
{
    Action onSizeChange;

    private void OnRectTransformDimensionsChange()
    {
        onSizeChange?.Invoke();
    }

    public void AddSizeChangeCallback(Action onSizeChange)
    {
        this.onSizeChange += onSizeChange;
    }

    public void RemoveSizeChangeCallback(Action onSizeChange)
    {
        this.onSizeChange -= onSizeChange;
    }
}

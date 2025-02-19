using UnityEngine;
using UnityEngine.UI;

public class ProperRatioFitter : MonoBehaviour
{
    public CanvasScaler scaler;

    public Mode ratioMode;
    public float minRatio; // width / height
    public float maxRatio; // width / height

    Rect rect
    {
        get
        {
            if (mRectTransform == null)
                mRectTransform = GetComponent<RectTransform>();
            return mRectTransform.rect;
        }
    }

    RectTransform mRectTransform;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UpdateLayout();
#endif
    }

    private void OnRectTransformDimensionsChange()
    {
#if !UNITY_EDITOR
        UpdateLayout();
#endif
    }

    void UpdateLayout()
    {
        if (scaler == null)
            return;

        var ratio = rect.width / rect.height;

        if (ratioMode == Mode.Min)
        {
            if (ratio < minRatio)
                scaler.matchWidthOrHeight = 0f;
            else
                scaler.matchWidthOrHeight = 1f;
        }
        else if (ratioMode == Mode.Max)
        {
            if (maxRatio > 0 && ratio > maxRatio)
                scaler.matchWidthOrHeight = 1f;
            else
                scaler.matchWidthOrHeight = 0f;
        }
        else if (ratioMode == Mode.Both)
        {
            if (ratio < minRatio)
                scaler.matchWidthOrHeight = 0f;
            else if (maxRatio > 0 && ratio > maxRatio)
                scaler.matchWidthOrHeight = 1f;
        }
    }

    public enum Mode
    {
        None,
        Min,
        Max,
        Both
    }
}

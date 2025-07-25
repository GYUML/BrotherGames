using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtil
{
    public static void ApplySafeAreaAnchor(ref RectTransform rt, bool topSafe = false, bool bottomSafe = false, bool leftSafe = false, bool rightSafe = false)
    {
        var safeArea = Screen.safeArea;
        var topOffset = topSafe ? Screen.height - (safeArea.position.y + safeArea.height) : 0f;
        var bottomOffset = bottomSafe ? safeArea.position.y : 0f;
        var leftOffset = leftSafe ? safeArea.position.x : 0f;
        var rightOffset = rightSafe ? Screen.width - (safeArea.position.x + safeArea.width) : 0f;
        var scale = 1 / rt.lossyScale.x;

        rt.offsetMin = new Vector2(leftOffset, bottomOffset) * scale;
        rt.offsetMax = new Vector2(-rightOffset, -topOffset) * scale;
    }

    // ratio : width / height
    public static void ApplyPreserveRatio(Rect rect, CanvasScaler scaler, float minRatio = 0f, float maxRatio = 0f)
    {
        var ratio = rect.width / rect.height;

        if (minRatio > 0 && maxRatio > 0)
        {
            if (ratio < minRatio)
                scaler.matchWidthOrHeight = 0f;
            else if (maxRatio > 0 && ratio > maxRatio)
                scaler.matchWidthOrHeight = 1f;
        }
        else if (minRatio > 0)
        {
            if (ratio < minRatio)
                scaler.matchWidthOrHeight = 0f;
            else
                scaler.matchWidthOrHeight = 1f;
        }
        else if (maxRatio > 0)
        {
            if (maxRatio > 0 && ratio > maxRatio)
                scaler.matchWidthOrHeight = 1f;
            else
                scaler.matchWidthOrHeight = 0f;
        }
    }
}

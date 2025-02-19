using UnityEngine;

public class AspectRatioScaleFitter : MonoBehaviour
{
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

    void OnRectTransformDimensionsChange()
    {
        UpdateLayout();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UpdateLayout();
#endif
    }

    void UpdateLayout()
    {
        var parentRectTransform = transform.parent as RectTransform;
        var parentRect = parentRectTransform.rect;

        if (parentRectTransform == null)
            return;

        if (rect.width == 0 || transform.localScale.x == 0
            || rect.height == 0 || transform.localScale.y == 0)
            return;

        var needScaleX = parentRect.width / (rect.width * transform.localScale.x);
        var needScaleY = parentRect.height / (rect.height * transform.localScale.y);

        var newScale = needScaleX < needScaleY ? transform.localScale.x * needScaleX : transform.localScale.y * needScaleY;
        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }
}

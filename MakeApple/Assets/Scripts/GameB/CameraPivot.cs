using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public float minRatio;
    public float scaler;

    float prevRatio;

    Camera targetCamera
    {
        get
        {
            if (mTargetCamera == null)
                mTargetCamera = GetComponent<Camera>();

            return mTargetCamera;
        }
    }

    Camera mTargetCamera;

    private void Update()
    {
        UpdatePivot();
    }

    void UpdatePivot()
    {
        if (targetCamera != null)
        {
            var nowRatio = (float)Screen.width / Screen.height;
            if (nowRatio != prevRatio && nowRatio < minRatio)
            {
                var newPosition = targetCamera.transform.position;
                newPosition.y = (minRatio - nowRatio) * scaler;
                targetCamera.transform.position = newPosition;

                prevRatio = nowRatio;
            }
        }
    }
}

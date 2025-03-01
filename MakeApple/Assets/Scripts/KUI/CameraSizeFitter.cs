using UnityEngine;

public class CameraSizeFitter : MonoBehaviour
{
    public float minRatio = 0.5625f;
    public float minScaler = 1f;

    float baseSize;
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

    private void Awake()
    {
        if (targetCamera != null)
            baseSize = targetCamera.orthographicSize;
    }

    private void Update()
    {
        UpdateSize();
    }

    void UpdateSize()
    {
        if (targetCamera != null)
        {
            var nowRatio = (float)Screen.width / Screen.height;
            if (nowRatio != prevRatio)
            {
                if (nowRatio < minRatio)
                    targetCamera.orthographicSize = baseSize + (minRatio - nowRatio) * minScaler;
                else
                    targetCamera.orthographicSize = baseSize;

                prevRatio = nowRatio;
            }
        }
    }
}

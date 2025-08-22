using UnityEngine;
using UnityEngine.EventSystems;

public class DragCamera : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Camera mainCamera; // 인스펙터에서 메인 카메라를 연결

    public Vector2 maxCameraPos;
    public Vector2 minCameraPos;

    private Vector3 dragOrigin;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var pointerData = eventData;
        dragOrigin = mainCamera.ScreenToWorldPoint(pointerData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1) return; // 멀티 터치 시 무시

        var pointerData = eventData;
        Vector3 currentPosition = mainCamera.ScreenToWorldPoint(pointerData.position);
        Vector3 difference = dragOrigin - currentPosition;

        var newPos = mainCamera.transform.position + difference;
        newPos.x = Mathf.Clamp(newPos.x, minCameraPos.x, maxCameraPos.x);
        newPos.y = Mathf.Clamp(newPos.y, minCameraPos.y, maxCameraPos.y);
        mainCamera.transform.position = newPos;
    }
}

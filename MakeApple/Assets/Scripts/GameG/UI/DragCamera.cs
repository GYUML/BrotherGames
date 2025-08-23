using UnityEngine;
using UnityEngine.EventSystems;

namespace GameG
{
    public class DragCamera : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Camera mainCamera; // 인스펙터에서 메인 카메라를 연결

        public Vector2 maxCameraPos;
        public Vector2 minCameraPos;

        private Vector3 dragOrigin;

        public bool Locked { get; set; }
        public bool IsDragging { get; set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Locked)
                return;

            IsDragging = true;

            var pointerData = eventData;
            dragOrigin = mainCamera.ScreenToWorldPoint(pointerData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 멀티 터치 시 무시
            if (Input.touchCount > 1) 
                return;

            if (Locked)
                return;

            var pointerData = eventData;
            Vector3 currentPosition = mainCamera.ScreenToWorldPoint(pointerData.position);
            Vector3 difference = dragOrigin - currentPosition;

            var newPos = mainCamera.transform.position + difference;
            newPos.x = Mathf.Clamp(newPos.x, minCameraPos.x, maxCameraPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minCameraPos.y, maxCameraPos.y);
            mainCamera.transform.position = newPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
        }
    }
}

using UnityEngine;

namespace FireMage
{
    public class PlayerMoveInput : MonoBehaviour
    {
        public GameObject moveTarget;
        public Animator animator;

        public Vector2 targetMinRange;
        public Vector2 targetMaxRange;

        // mouse
        public float dragHeight;

        // side
        public float sideArea;
        public float sideMoveSpeed;

        Camera mainCam;
        
        bool isDragging;
        Vector3 prevPos;

        private void OnEnable()
        {
            mainCam = Camera.main;
            animator.SetBool("Move", true);
        }

        private void Update()
        {
            DragMove();
        }

        void DragMove()
        {
            if (!isDragging && Input.GetMouseButtonDown(0))
            {
                isDragging = true;

                if (GetMousePoint(out var hitPos))
                    prevPos = hitPos;
            }

            if (!Input.GetMouseButton(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                if (IsTargetLeftSideOut())
                {
                    mainCam.transform.position += new Vector3(-sideMoveSpeed * Time.deltaTime, 0f, 0f);
                }
                else if (IsTargetRightSideOut())
                {
                    mainCam.transform.position += new Vector3(sideMoveSpeed * Time.deltaTime, 0f, 0f);
                }

                if (GetMousePoint(out var hitPos))
                {
                    var moveVector = hitPos - prevPos;
                    moveVector.y = 0;

                    var newPos = moveTarget.transform.position + moveVector;
                    newPos.x = Mathf.Clamp(newPos.x, targetMinRange.x, targetMaxRange.x);
                    newPos.z = Mathf.Clamp(newPos.z, targetMinRange.y, targetMaxRange.y);
                    moveTarget.transform.position = newPos;

                    prevPos = hitPos;
                }
            }
        }

        // 마우스와 평면 만나는 위치
        bool GetMousePoint(out Vector3 hitPoint)
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0)); // y = dragHeight 평면

            if (dragPlane.Raycast(ray, out var enter))
            {
                hitPoint = ray.GetPoint(enter);
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }

        bool IsTargetLeftSideOut()
        {
            var leftBoundary = Screen.width * sideArea;
            var screenPos = mainCam.WorldToScreenPoint(moveTarget.transform.position);

            return screenPos.x < leftBoundary;
        }

        bool IsTargetRightSideOut()
        {
            var rightBoundary = Screen.width * (1f - sideArea);
            var screenPos = mainCam.WorldToScreenPoint(moveTarget.transform.position);

            return screenPos.x > rightBoundary;
        }
    }
}

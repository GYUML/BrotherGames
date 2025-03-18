using UnityEngine;
using UnityEngine.InputSystem;

namespace GameB
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rigid;
        public float speed;
        public Vector2 minRange;
        public Vector2 maxRange;

        Vector2 moveInput;
        float stunEndTime;

        private void Update()
        {
            if (moveInput.x > 0)
                transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            else if (moveInput.x < 0)
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        private void FixedUpdate()
        {
            if (rigid.position.x < minRange.x)
                rigid.MovePosition(new Vector2(minRange.x, rigid.position.y));
            else if (rigid.position.x > maxRange.x)
                rigid.MovePosition(new Vector2(maxRange.x, rigid.position.y));

            if (Time.time > stunEndTime)
                rigid.AddForce(moveInput * speed);

            var factorY = (3f - transform.position.y) / 4f;
            rigid.AddForce(Vector2.up * factorY * speed);
            Managers.GameLogic.SetFallingFactor(factorY);
        }

        public void OnMove(InputValue context)
        {
            moveInput = context.Get<Vector2>().normalized;
        }

        public void OnStun(float stunTime)
        {
            stunEndTime = Time.time + stunTime;
        }
    }
}


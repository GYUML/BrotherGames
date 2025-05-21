using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

namespace GameE
{
    public class PlayerController : MonoBehaviour
    {
        public LayerMask groundLayer;
        public Vector2 groundCheckPoint;
        public Vector2 groundCheckSize;

        public Rigidbody2D playerRb;
        public Collider2D playerCol;
        public Animator playerAnim;

        public float moveSpeed;
        public int maxJumpCount = 2;
        public float jumpPower = 4f;
        public float doubleJumpPower = 1.5f;

        bool isGrounded;
        bool isHangOn;
        float jumpingMoveX;
        bool isLeft;
        int nowJumpCount;
        Collider2D nowGroundCol;
        Collider2D ignoreGroundCol;

        private void Start()
        {
            groundLayer = LayerMask.GetMask("Ground");
        }

        private void FixedUpdate()
        {
            if (playerRb.linearVelocityY > 0)
            {
                isGrounded = false;
            }
            else if (playerRb.linearVelocityY <= 0f)
            {
                if (!IsGrounded())
                {
                    nowGroundCol = Physics2D.OverlapBox(transform.TransformPoint(groundCheckPoint), transform.lossyScale * groundCheckSize, 0f, groundLayer);
                    if (nowGroundCol != ignoreGroundCol)
                        isGrounded = nowGroundCol != null && transform.position.y > nowGroundCol.bounds.center.y;
                }
            }
            
            if (IsGrounded())
            {
                jumpingMoveX = 0f;
                nowJumpCount = 0;
            }
            else
                JumpMoveX();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.TransformPoint(groundCheckPoint), transform.lossyScale * groundCheckSize);
            Gizmos.DrawRay(playerRb.position, Vector2.down * 4f);
        }

        public void MoveX(float inputX)
        {
            if (jumpingMoveX != 0f)
            {
                if (inputX * jumpingMoveX >= 0f)
                    return;
                else
                    jumpingMoveX = 0f;
            }

            playerRb.linearVelocityX = Mathf.Clamp(inputX, -1f, 1f) * moveSpeed;
            Flip(inputX);
            playerAnim.SetBool("1_Move", inputX != 0f);
        }

        public void Jump(float inputX)
        {
            if (IsGrounded())
            {
                nowJumpCount++;
                jumpingMoveX = Mathf.Clamp(inputX, -1f, 1f) * moveSpeed;
                AddForceUp(jumpPower);
            }
        }

        public void DoubleJump(float inputX, Action<Vector2, bool> onJump = null)
        {
            if (!IsGrounded() && nowJumpCount < maxJumpCount)
            {
                nowJumpCount++;

                if (inputX == 0)
                {
                    if (jumpingMoveX != 0f || nowJumpCount > 2)
                        AddForceUp(doubleJumpPower);
                    else
                        AddForceUp(doubleJumpPower * 2f);
                }
                else
                {
                    jumpingMoveX = Mathf.Clamp(inputX, -1f, 1f) * moveSpeed * 2f;
                    AddForceUp(doubleJumpPower);
                }
                
                onJump?.Invoke(transform.position, IsLeft());
            }
        }

        public void DownJump()
        {
            var rayHits = Physics2D.RaycastAll(playerRb.position, Vector2.down, 4f, groundLayer);
            if (rayHits.Length > 1)
            {
                Debug.Log("DownJump");
                StartCoroutine(IgnoreColliderCo());
            }

            IEnumerator IgnoreColliderCo()
            {
                var ignoreTimeout = Time.fixedTime + 0.5f;
                ignoreGroundCol = nowGroundCol;
                isGrounded = false;
                Physics2D.IgnoreCollision(playerCol, ignoreGroundCol, true);

                while (Time.fixedTime < ignoreTimeout && IsGrounded() == false)
                    yield return new WaitForFixedUpdate();

                Physics2D.IgnoreCollision(playerCol, ignoreGroundCol, false);
                ignoreGroundCol = null;
            }
        }

        public void Attack()
        {
            playerAnim.SetTrigger("2_Attack");
        }

        public bool IsLeft()
        {
            return isLeft;
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsHandOn()
        {
            return isHangOn;
        }

        void AddForceUp(float jumpPower)
        {
            playerRb.linearVelocityY = 0f;
            playerRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        void JumpMoveX()
        {
            playerRb.linearVelocityX = jumpingMoveX;
            Flip(jumpingMoveX);
        }

        void Flip(float inputX)
        {
            if (inputX < 0f)
            {
                isLeft = true;
                transform.localRotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
            }
            else if (inputX > 0f)
            {
                isLeft = false;
                transform.localRotation = Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z);
            }
        }
    }
}

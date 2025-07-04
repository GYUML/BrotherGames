using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        public BaseHitBox hitBox;

        public float moveSpeed;
        public int maxDoubleJumpCount = 2;
        public float jumpPower = 4f;
        public float doubleJumpPower = 1.5f;

        bool isGrounded;
        bool isHangOn;
        float jumpingMoveX;
        bool isLeft;
        int nowDoubleJumpCount;
        float downJumpTimeLimit;
        float stunEndTime;
        float stunIgnoreEnd;

        Collider2D nowGroundCol;
        Collider2D ignoreGroundCol;

        readonly float LinearVelocityThreshHold = 0.01f;
        readonly float StunIgnoreTime = 0.5f;

        private void Start()
        {
            groundLayer = LayerMask.GetMask("Ground");
            hitBox.SetOnAttackedEvent(OnAttacked);
        }

        private void FixedUpdate()
        {
            if (playerRb.linearVelocityY > LinearVelocityThreshHold)
            {
                isGrounded = false;
            }
            else
            {
                if (!IsGrounded())
                {
                    nowGroundCol = Physics2D.OverlapBox(transform.TransformPoint(groundCheckPoint), transform.lossyScale * groundCheckSize, 0f, groundLayer);
                    if (nowGroundCol != ignoreGroundCol)
                        isGrounded = nowGroundCol != null;
                }
            }

            if (Time.time > stunEndTime)
            {
                if (IsGrounded())
                {
                    jumpingMoveX = 0f;
                    nowDoubleJumpCount = 0;
                }
                else
                {
                    JumpMoveX();
                }
            }   
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.TransformPoint(groundCheckPoint), transform.lossyScale * groundCheckSize);
            Gizmos.DrawRay(playerRb.position, Vector2.down * 4f);
        }

        public void MoveX(float inputX)
        {
            if (Time.time < stunEndTime)
                return;

            if (!IsGrounded())
            {
                if (jumpingMoveX != 0 && inputX * jumpingMoveX >= 0f)
                    return;
                else
                    jumpingMoveX = inputX * moveSpeed;
            }

            playerRb.linearVelocityX = Mathf.Clamp(inputX, -1f, 1f) * moveSpeed;
            Flip(inputX);
            playerAnim.SetBool("1_Move", inputX != 0f);
        }

        public void Jump(float inputX)
        {
            if (IsGrounded())
            {
                jumpingMoveX = Mathf.Clamp(inputX, -1f, 1f) * moveSpeed;
                AddForceUp(jumpPower);
            }
        }

        public void DoubleJump(float inputX, Action<Vector2, bool> onJump = null)
        {
            if (!IsGrounded() && nowDoubleJumpCount < maxDoubleJumpCount)
            {
                nowDoubleJumpCount++;

                if (inputX == 0)
                {
                    if (jumpingMoveX != 0f || nowDoubleJumpCount > 1)
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
            if (IsGrounded())
            {
                if (Time.fixedTime < downJumpTimeLimit)
                {
                    if (GetUnderGroundCount() > 1)
                        StartCoroutine(IgnoreColliderCo());

                    downJumpTimeLimit = 0f;
                }
                else
                {
                    downJumpTimeLimit = Time.fixedTime + 0.2f;
                }
            }

            int GetUnderGroundCount()
            {
                var rayHits = Physics2D.RaycastAll(playerRb.position, Vector2.down, 4f, groundLayer);
                var groundCount = 0;

                if (rayHits.Length > 0)
                {
                    foreach (var rayHit in rayHits)
                    {
                        var tileMap = rayHit.collider.GetComponent<Tilemap>();
                        if (tileMap != null)
                        {
                            var underneathCount = GameUtil.GetTilesUnderneathCount(tileMap, transform.position, 10);
                            groundCount += underneathCount;
                        }
                    }
                }

                return groundCount;
            }

            IEnumerator IgnoreColliderCo()
            {
                var ignoreTimeout = Time.fixedTime + 0.2f;
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

        public void Respawn()
        {
            playerRb.linearVelocity = Vector2.zero;
            jumpingMoveX = 0f;
            nowDoubleJumpCount = 0;
        }

        void OnAttacked(Vector2 direction, float stunTime)
        {
            Debug.Log("OnAttacked");

            if (Time.time < stunIgnoreEnd)
                return;

            stunEndTime = Time.time + stunTime;
            stunIgnoreEnd = Time.time + StunIgnoreTime;
            jumpingMoveX = 0f;

            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(direction, ForceMode2D.Impulse);
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

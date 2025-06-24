using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameE
{
    public class EnemyUnit : MonoBehaviour
    {
        public Rigidbody2D rigid;
        public GameObject root;

        public float knockbackPower;
        public float knockbackTime;
        public float moveSpeed;
        public float chaseError;

        public int Id
        {
            get { return id; }
            private set { id = value; }
        }

        int id;
        Transform chaseTarget;
        float knockbackEndTime;
        float moveEndTime;
        float nowVelocityX;

        public void Init(int id)
        {
            Id = id;
            chaseTarget = null;
        }

        public void OnAttacked(bool pushToLeft)
        {
            Flip(pushToLeft ? 1f : -1f);
            knockbackEndTime = Time.time + knockbackTime;
            rigid.linearVelocityX = 0f;
            rigid.AddForceX(pushToLeft ? -knockbackPower : knockbackPower, ForceMode2D.Impulse);
        }

        public void SetChaseTarget(Transform target)
        {
            this.chaseTarget = target;
        }

        private void FixedUpdate()
        {
            if (Time.time > knockbackEndTime)
            {
                if (chaseTarget != null && MathF.Abs(transform.position.x - chaseTarget.position.x) > chaseError)
                {
                    if (transform.position.x < chaseTarget.transform.position.x)
                        nowVelocityX = moveSpeed;
                    else
                        nowVelocityX = -moveSpeed;
                }
                else if (Time.time > moveEndTime)
                {
                    moveEndTime = Time.time + Random.Range(5f, 10f);
                    nowVelocityX = Random.Range(-1, 2) * moveSpeed;
                }

                rigid.linearVelocityX = nowVelocityX;
                Flip(rigid.linearVelocityX);
            }
        }

        void Flip(float inputX)
        {
            if (root != null)
            {
                var localScale = root.transform.localScale;
                if (inputX < 0)
                    localScale.x = Mathf.Abs(localScale.x);
                else
                    localScale.x = Mathf.Abs(localScale.x) * -1f;
                root.transform.localScale = localScale;
            }
            
            //if (inputX < 0f)
            //    transform.localRotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
            //else if (inputX > 0f)
            //    transform.localRotation = Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z);
        }
    }
}

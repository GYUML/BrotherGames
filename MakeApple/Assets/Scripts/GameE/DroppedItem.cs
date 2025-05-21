using DG.Tweening;
using System;
using UnityEngine;

namespace GameE
{
    public class DroppedItem : MonoBehaviour
    {
        public SpriteRenderer renderer;
        public Rigidbody2D rb;

        public float moveRangeY;
        public float moveDuration;

        Action onDestroy;
        float acquireMoveTime;
        float acquireMoveError;

        Transform acquirerer;
        float timeCounter;

        private void Start()
        {
            renderer.transform.DOLocalMoveY(moveRangeY, moveDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
        }

        private void Update()
        {
            if (acquirerer != null)
            {
                var desPos = acquirerer.position;
                desPos.y += 0.2f;
                if (Vector3.Magnitude(desPos - transform.position) < acquireMoveError)
                {
                    gameObject.SetActive(false);
                    onDestroy?.Invoke();
                }
                else
                {
                    timeCounter += Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, desPos, timeCounter / acquireMoveTime);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                rb.simulated = false;
                acquirerer = collision.transform;
                timeCounter = 0f;
            }
        }

        public void Drop(Vector2 dropVector, float acquireMoveTime, float acquireMoveError, Action onDestroy)
        {
            rb.simulated = true;
            rb.AddForce(dropVector, ForceMode2D.Impulse);
            this.acquireMoveTime = acquireMoveTime;
            this.acquireMoveError = acquireMoveError;
            this.onDestroy = onDestroy;
        }
    }

}

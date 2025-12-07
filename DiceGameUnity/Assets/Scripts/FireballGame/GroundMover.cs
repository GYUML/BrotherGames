using System.Collections.Generic;
using UnityEngine;

namespace FireMage
{
    public class GroundMover : MonoBehaviour
    {
        public GameObject groundPrefab;

        public float moveSpeed;
        public int maxCount;
        public Vector3 startPos;
        public Vector3 offsetPos;
        public Vector3 minBoundary;
        public Vector3 maxBoundary;

        Queue<GameObject> pool = new Queue<GameObject>();

        private void Start()
        {
            groundPrefab.SetActive(false);

            for (int i = 0; i < maxCount; i++)
            {
                var newGround = Instantiate(groundPrefab);
                newGround.transform.position = startPos + offsetPos * i;
                newGround.gameObject.SetActive(true);
                pool.Enqueue(newGround);
            }
        }

        private void Update()
        {
            var first = pool.Peek();
            if (first != null)
            {
                if (first.transform.position.z < minBoundary.z)
                {
                    var now = pool.Dequeue();
                    now.transform.position = new Vector3(now.transform.position.x, now.transform.position.y, maxBoundary.z);
                    pool.Enqueue(now);
                }
            }

            foreach (var item in pool)
            {
                var nextPos = item.transform.position;
                nextPos.z -= moveSpeed * Time.deltaTime;
                item.transform.position = nextPos;
            }
        }
    }
}

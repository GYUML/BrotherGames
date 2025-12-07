using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireMage
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyMover enemyPrefab;

        public float moveSpeed;
        public float spawnMinPosX;
        public float spawnMaxPosX;
        public float spawnPosY;
        public float spawnPosZ;

        // Test
        public float spwanDelay;

        Stack<EnemyMover> pool = new Stack<EnemyMover>();
        Coroutine spawnCo;

        private void Start()
        {
            enemyPrefab.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            if (spawnCo != null)
                StopCoroutine(spawnCo);
            spawnCo = StartCoroutine(SpawnRandom());
        }

        private void OnDisable()
        {
            if (spawnCo != null)
                StopCoroutine(spawnCo);
        }

        IEnumerator SpawnRandom()
        {
            while (true)
            {
                var spawnPosition = new Vector3(Random.Range(spawnMinPosX, spawnMaxPosX), spawnPosY, spawnPosZ);
                SpawnEnemy(spawnPosition);
                yield return new WaitForSeconds(spwanDelay);
            }
        }

        public void SpawnEnemy(Vector3 position)
        {
            var spawned = pool.Count > 0 ? pool.Pop() : Instantiate(enemyPrefab);
            spawned.gameObject.SetActive(true);
            spawned.transform.position = position;
            spawned.moveSpeed = moveSpeed;
            spawned.enemySpawner = this;
            spawned.hp = 10;
        }

        public void ReturnPool(EnemyMover item)
        {
            pool.Push(item);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace FireMage
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public GameObject projectile;
        public GameObject shooter;

        public float spawnDelay;
        public float moveSpeed;
        public float maxProjectile;
        public Vector3 spawnOffset;

        public Queue<GameObject> pool = new Queue<GameObject>();

        float nextSpawnTime;

        private void Update()
        {
            if (Time.time > nextSpawnTime && shooter != null)
            {
                if (pool.Count < maxProjectile)
                {
                    var ins = Instantiate(projectile);
                    ins.SetActive(true);
                    pool.Enqueue(ins);
                    ins.transform.position = shooter.transform.position + spawnOffset;
                }
                else
                {
                    var ins = pool.Dequeue();
                    pool.Enqueue(ins);
                    ins.transform.position = shooter.transform.position + spawnOffset;
                }

                nextSpawnTime = Time.time + spawnDelay;
            }

            foreach (var ins in pool)
            {
                ins.transform.position += new Vector3(0f, 0f, moveSpeed * Time.deltaTime);
            }
        }
    }

}

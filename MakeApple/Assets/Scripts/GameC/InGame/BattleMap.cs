using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameC
{
    public class BattleMap : MonoBehaviour
    {
        public GameObject groundPrefab;

        public GameObject player;
        public GameObject enemy;

        public float moveSpeed;
        public float mapMinX;

        public Vector3 groundStartPos;
        public float groundSpace;

        List<GameObject> groundList = new List<GameObject>();

        CoroutinePlayer coroutinePlayer = new CoroutinePlayer();

        private void Start()
        {
            StartCoroutine(coroutinePlayer.MainCoroutine());
            SpawnMap();
        }

        public void EnemyDie()
        {
            coroutinePlayer.AddCoroutine(EnemyDieProc());
        }

        public void Move()
        {
            coroutinePlayer.AddCoroutine(MoveProc());
        }

        void SpawnMap()
        {
            for (int i = 0; i < 10; i++)
            {
                var instantiated = Instantiate(groundPrefab);
                groundList.Add(instantiated);
                instantiated.transform.position = groundStartPos + Vector3.right * groundSpace * i;
                instantiated.gameObject.SetActive(true);
            }
        }

        IEnumerator MoveProc()
        {
            var moveDistance = 12f;
            var moveCounter = 0f;

            player.GetComponent<Animator>().SetBool("1_Move", true);
            enemy.transform.position += Vector3.right * 12f;
            enemy.gameObject.SetActive(true);

            while (moveCounter < moveDistance)
            {
                moveCounter += moveSpeed * Time.deltaTime;

                foreach (var ground in groundList)
                {
                    ground.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                    if (ground.transform.position.x < mapMinX)
                    {
                        var newPosition = ground.transform.position;
                        newPosition.x = mapMinX + groundSpace * groundList.Count;
                        ground.transform.position = newPosition;
                    }
                }

                enemy.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

                yield return null;
            }

            player.GetComponent<Animator>().SetBool("1_Move", false);
        }

        IEnumerator EnemyDieProc()
        {
            enemy.GetComponentInChildren<Animator>().SetTrigger("4_Death");
            yield return new WaitForSeconds(0.667f);
            enemy.gameObject.SetActive(false);
        }
    }
}


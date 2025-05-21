using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameD
{
    public class BattleMap : MonoBehaviour
    {
        public GameObject player;
        public GameObject enemyPrefab;

        CoroutinePlayer coroutinePlayer = new CoroutinePlayer();
        Stack<GameObject> enemyPool = new Stack<GameObject>();

        GameObject nowEnemy;

        private void Start()
        {
            StartCoroutine(coroutinePlayer.MainCoroutine());
            enemyPrefab.gameObject.SetActive(false);
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            coroutinePlayer.AddCoroutine(SpawnEnemyCo());
        }

        public void PlayerAttack(bool isSuccess)
        {
            coroutinePlayer.AddCoroutine(PlayerAttackCo(isSuccess));
        }

        IEnumerator PlayerAttackCo(bool isSuccess)
        {
            player.GetComponent<Animator>().SetTrigger("2_Attack");
            yield return new WaitForSeconds(0.1f);
            if (isSuccess && nowEnemy != null)
            {
                StartCoroutine(EnemyDeadCo());
                SpawnEnemy();
            }
        }

        IEnumerator SpawnEnemyCo()
        {
            yield return new WaitForSeconds(0.1f);

            nowEnemy = enemyPool.Count > 0 ? enemyPool.Pop() : Instantiate(enemyPrefab);
            nowEnemy.SetActive(true);
            nowEnemy.transform.position = new Vector3(0.5f, -0.64f, 0f);
        }

        IEnumerator EnemyDeadCo()
        {
            var enemy = nowEnemy;
            nowEnemy = null;
            if (enemy != null)
            {
                enemy.transform.DOMove(new Vector3(3.5f, 4.5f, 0f), 1f);
                yield return new WaitForSeconds(1f);
                enemy.gameObject.SetActive(false);
                enemyPool.Push(enemy);
            }
            else
            {
                Debug.LogError("EnemyDeadCo() enemy is null");
            }
        }
    }
}

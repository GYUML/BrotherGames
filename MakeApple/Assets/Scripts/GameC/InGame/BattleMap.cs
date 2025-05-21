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
        public GameObject fountain;
        public GameObject blessingStatue;

        public float moveSpeed;
        public float mapMinX;

        public Vector3 groundStartPos;
        public float groundSpace;

        readonly float SPAWN_POS_X = 6.5f;
        readonly float MOVE_DISTANCE_X = 6f;

        List<GameObject> groundList = new List<GameObject>();

        CoroutinePlayer coroutinePlayer = new CoroutinePlayer();

        private void Start()
        {
            enemy.SetActive(false);
            StartCoroutine(coroutinePlayer.MainCoroutine());
            SpawnMap();
        }

        public void SetStage(StageType stageType)
        {
            coroutinePlayer.AddCoroutine(SetStageProc(stageType));
        }

        public void EnemyDie()
        {
            coroutinePlayer.AddCoroutine(EnemyDieProc());
        }

        public void Move()
        {
            coroutinePlayer.AddCoroutine(MoveProc());
        }

        public void PlayerAttack()
        {
            player.GetComponent<Animator>().SetTrigger("2_Attack");
        }

        public void EnemyAttack()
        {
            enemy.GetComponentInChildren<Animator>().SetTrigger("2_Attack");
        }

        public void OnDamaged(long maxHp, long nowHp, long damage)
        {
            Managers.UI.GetLayout<BattleHudLayout>().SpawnDamageText(damage, enemy.transform.position);
            Managers.UI.GetLayout<BattleHudLayout>().UpdateHpBar(enemy.transform, maxHp, nowHp);
        }

        public void SpawnUnit(long maxHp, long nowHp)
        {
            coroutinePlayer.AddCoroutine(SpawnUnitProc(maxHp, nowHp, new Vector3(SPAWN_POS_X, 0f, 0f)));
        }

        public void SpawnPlayer(long maxHp, long nowHp)
        {
            Managers.UI.GetLayout<BattleHudLayout>().RegisterHpBarTarget(player.transform);
            Managers.UI.GetLayout<BattleHudLayout>().UpdateHpBar(player.transform, maxHp, nowHp);
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

        IEnumerator SpawnUnitProc(long maxHp, long nowHp, Vector3 position)
        {
            enemy.GetComponentInChildren<Animator>().SetBool("isDeath", false);
            enemy.transform.position = position;
            enemy.gameObject.SetActive(true);
            Managers.UI.GetLayout<BattleHudLayout>().RegisterHpBarTarget(enemy.transform);
            Managers.UI.GetLayout<BattleHudLayout>().UpdateHpBar(enemy.transform, maxHp, nowHp);

            yield return null;
        }

        IEnumerator SetStageProc(StageType stageType)
        {
            if (stageType == StageType.Recovery)
            {
                fountain.transform.position = new Vector3(SPAWN_POS_X, 1f, 0f);
                fountain.gameObject.SetActive(true);
            }
            else if (stageType == StageType.Blessing)
            {
                blessingStatue.transform.position = new Vector3(SPAWN_POS_X, 1f, 0f);
                blessingStatue.gameObject.SetActive(true);
            }

            yield return null;
        }

        IEnumerator MoveProc()
        {
            var moveDistance = MOVE_DISTANCE_X;
            var moveCounter = 0f;

            player.GetComponent<Animator>().SetBool("1_Move", true);

            while (moveCounter < moveDistance)
            {
                var moveVector = moveSpeed * Time.deltaTime;
                moveCounter += moveVector;

                foreach (var ground in groundList)
                {
                    ground.transform.position += Vector3.left * moveVector;
                    if (ground.transform.position.x < mapMinX)
                    {
                        var newPosition = ground.transform.position;
                        newPosition.x = mapMinX + groundSpace * groundList.Count;
                        ground.transform.position = newPosition;
                    }
                }

                enemy.transform.position += Vector3.left * moveVector;

                fountain.transform.position += Vector3.left * moveVector;
                blessingStatue.transform.position += Vector3.left * moveVector;

                yield return null;
            }

            player.GetComponent<Animator>().SetBool("1_Move", false);
        }

        IEnumerator EnemyDieProc()
        {
            enemy.GetComponentInChildren<Animator>().SetBool("isDeath", true);
            enemy.GetComponentInChildren<Animator>().SetTrigger("4_Death");
            Managers.UI.GetLayout<BattleHudLayout>().DeleteHpBarTarget(enemy.transform);

            yield return new WaitForSeconds(0.7f);

            enemy.gameObject.SetActive(false);
        }
    }
}


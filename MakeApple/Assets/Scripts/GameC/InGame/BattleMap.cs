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

        List<GameObject> groundList = new List<GameObject>();
        Dictionary<int, GameObject> unitDic = new Dictionary<int, GameObject>();

        CoroutinePlayer coroutinePlayer = new CoroutinePlayer();

        private void Start()
        {
            StartCoroutine(coroutinePlayer.MainCoroutine());
            SpawnMap();
        }

        public void SetStage(StageType stageType)
        {
            coroutinePlayer.AddCoroutine(SetStageProc(stageType));
        }

        public void EnemyDie(int unitId)
        {
            coroutinePlayer.AddCoroutine(EnemyDieProc(unitId));
        }

        public void Move()
        {
            coroutinePlayer.AddCoroutine(MoveProc());
        }

        public void PlayerAttack()
        {
            player.GetComponent<Animator>().SetTrigger("2_Attack");
        }

        public void EnemyAttack(int unitId)
        {
            if (unitDic.TryGetValue(unitId, out var unit))
            {
                unit.GetComponentInChildren<Animator>().SetTrigger("2_Attack");
            }
        }

        public void OnDamaged(int unitId, long maxHp, long nowHp, long damage)
        {
            if (unitDic.TryGetValue(unitId, out var unit))
            {
                Managers.UI.GetLayout<BattleHudLayout>().SpawnDamageText(damage, unit.transform.position);
                Managers.UI.GetLayout<BattleHudLayout>().UpdateHpBar(unit.transform, maxHp, nowHp);
            }
            else
            {
                Debug.LogError($"[BattleMap] OnDamaged() Not fount unit. unitId={unitId}");
            }
        }

        public void SpawnUnits(List<int> unitIds)
        {
            coroutinePlayer.AddCoroutine(SpawnUnitsProc(unitIds));
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

        void SpawnUnit(int unitId, long maxHp, long nowHp)
        {
            if (!unitDic.ContainsKey(unitId))
            {
                var unit = enemy;
                unitDic.Add(unitId, unit);

                unit.GetComponentInChildren<Animator>().SetBool("isDeath", false);
                unit.transform.position = new Vector3(12.5f, 0f, 0f);
                unit.gameObject.SetActive(true);

                Managers.UI.GetLayout<BattleHudLayout>().RegisterHpBarTarget(unit.transform);
                Managers.UI.GetLayout<BattleHudLayout>().UpdateHpBar(unit.transform, maxHp, nowHp);
            }
            else
            {
                Debug.LogError($"[BattleMap] Already Added. unitId={unitId}");
            }
        }

        IEnumerator SpawnUnitsProc(List<int> unitIds)
        {
            foreach (var unitId in unitIds)
                SpawnUnit(unitId, 100, 100);

            yield return null;
        }

        IEnumerator SetStageProc(StageType stageType)
        {
            if (stageType == StageType.Recovery)
            {
                fountain.transform.position = new Vector3(13.15f, 1f, 0f);
                fountain.gameObject.SetActive(true);
            }
            else if (stageType == StageType.Blessing)
            {
                blessingStatue.transform.position = new Vector3(13.15f, 1f, 0f);
                blessingStatue.gameObject.SetActive(true);
            }

            yield return null;
        }

        IEnumerator MoveProc()
        {
            var moveDistance = 12f;
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

                foreach (var unit in unitDic.Values)
                    unit.transform.position += Vector3.left * moveVector;

                fountain.transform.position += Vector3.left * moveVector;
                blessingStatue.transform.position += Vector3.left * moveVector;

                yield return null;
            }

            player.GetComponent<Animator>().SetBool("1_Move", false);
        }

        IEnumerator EnemyDieProc(int unitId)
        {
            if (unitDic.TryGetValue(unitId, out var unit))
            {
                unit.GetComponentInChildren<Animator>().SetBool("isDeath", true);
                unit.GetComponentInChildren<Animator>().SetTrigger("4_Death");
                yield return new WaitForSeconds(0.7f);
                unit.gameObject.SetActive(false);
                unitDic.Remove(unitId);

                Managers.UI.GetLayout<BattleHudLayout>().DeleteHpBarTarget(unit.transform);
            }
        }
    }
}


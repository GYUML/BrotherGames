using System.Collections.Generic;
using UnityEngine;

public class BattleLogic : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float enemySpeed;
    public float spawnInterval;

    Dictionary<int, long> enemyStatusDic = new Dictionary<int, long>();

    List<GameObject> enemyList = new List<GameObject>();
    Stack<GameObject> enemyPool = new Stack<GameObject>();
    Dictionary<int, GameObject> enemyDic = new Dictionary<int, GameObject>();

    float nextSpawnTime;
    int unitIdCounter;

    void FixedUpdate()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        foreach (var enemy in enemyList)
        {
            if (enemy.transform.position.z < -5f)
            {
                var unit = enemy.GetComponent<EnemyUnit>();
                if (unit != null)
                    RemoveEnemy(unit.Id);
            }
        }
    }

    public void AttackEnemy(int id)
    {
        if (!enemyStatusDic.ContainsKey(id))
            return;

        enemyStatusDic[id] -= 1;
        Debug.Log(enemyStatusDic[id]);

        if (enemyStatusDic[id] <= 0)
            RemoveEnemy(id);
    }

    public void RemoveEnemy(int id)
    {
        if (enemyStatusDic.ContainsKey(id))
            enemyStatusDic.Remove(id);

        if (enemyDic.TryGetValue(id, out var enemy))
        {
            enemy.SetActive(false);
            enemyDic.Remove(id);
            enemyPool.Push(enemy);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy;
        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Pop();
        }
        else
        {
            enemy = Instantiate(enemyPrefab);
            enemyList.Add(enemy);
        }

        enemy.GetComponent<Rigidbody>().MovePosition(new Vector3(Random.Range(-5f, 5f), -4f, 90f));
        enemy.GetComponent<EnemyUnit>().Id = unitIdCounter;
        enemy.GetComponent<EnemyUnit>().moveSpeed = enemySpeed;
        enemy.SetActive(true);

        enemyDic.Add(unitIdCounter, enemy);
        enemyStatusDic.Add(unitIdCounter, 10);
        unitIdCounter++;
    }
}

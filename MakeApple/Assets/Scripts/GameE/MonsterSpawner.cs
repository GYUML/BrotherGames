using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameE
{
    public class MonsterSpawner : MonoBehaviour
    {
        public EnemyUnit monsterPrefab;

        Stack<EnemyUnit> monsterPool = new Stack<EnemyUnit>();
        Dictionary<int, EnemyUnit> monsterDic = new Dictionary<int, EnemyUnit>();

        private void Start()
        {
            monsterPrefab.gameObject.SetActive(false);
        }

        public void Spawn(int id)
        {
            var pos = new Vector2(Random.Range(-10f, 10f), 0.5f);
            var monster = monsterPool.Count > 0 ? monsterPool.Pop() : Instantiate(monsterPrefab);
            monster.transform.position = pos;
            monster.gameObject.SetActive(true);
            monster.Init(id);
            monsterDic.Add(id, monster);
        }

        public void Despawn(int id)
        {
            if (TryGetMonster(id, out var monster))
            {
                monster.gameObject.SetActive(false);
                monsterPool.Push(monster);
                monsterDic.Remove(id);
            }
            else
            {
                Debug.LogError($"OnDeadMonster() Failed to find Monster. id={id}");
            }
        }

        public bool TryGetMonster(int id, out EnemyUnit monster)
        {
            return monsterDic.TryGetValue(id, out monster);
        }
    }
}

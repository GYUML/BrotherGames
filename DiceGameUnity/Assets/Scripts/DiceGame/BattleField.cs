using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    public EffectManager effectManager;

    public Animator player;
    public List<GameObject> enemyList = new List<GameObject>();

    public IEnumerator AttackEnemyCo(int index)
    {
        if (index >= enemyList.Count)
            yield break;

        var enemy = enemyList[index];
        player.Play("Attack");
        yield return new WaitForSeconds(0.5f);

        effectManager.ShowEffect(3, enemy.transform.position + new Vector3(0f, 0.5f, 0f), 0.5f);
    }
}

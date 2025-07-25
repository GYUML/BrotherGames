using UnityEngine;

public class CannonAttacker : MonoBehaviour
{
    public float damageInterval;

    float attackPossibleTime;

    private void OnParticleCollision(GameObject other)
    {
        if (Time.time < attackPossibleTime)
            return;

        var enemy = other.GetComponent<EnemyUnit>();
        if (enemy != null)
        {
            enemy.OnAttacked();
            attackPossibleTime = Time.time + damageInterval;
        }
    }
}

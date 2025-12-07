using FireMage;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public float moveSpeed;
    public long hp;

    private void Update()
    {
        var newPos = transform.position;
        newPos.z -= moveSpeed * Time.deltaTime;
        transform.position = newPos;
    }

    public void OnAttacked(long damage)
    {
        hp -= damage;

        if (hp <= 0)
            ReturnPool();
    }

    void ReturnPool()
    {
        gameObject.SetActive(false);
        enemySpawner.ReturnPool(this);
    }
}

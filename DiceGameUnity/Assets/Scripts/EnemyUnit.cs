using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Rigidbody rigidBody;
    public BattleLogic battleLogic;

    public int Id;
    public float moveSpeed;
    public float stunTime;

    float stunEndTime;

    private void FixedUpdate()
    {
        if (Time.time < stunEndTime)
            return;

        rigidBody.linearVelocity = new Vector3(0f, 0f, -moveSpeed);
    }

    public void OnAttacked()
    {
        battleLogic.AttackEnemy(Id);
        stunEndTime = Time.time + stunTime;
        rigidBody.linearVelocity = Vector3.zero;
    }
}

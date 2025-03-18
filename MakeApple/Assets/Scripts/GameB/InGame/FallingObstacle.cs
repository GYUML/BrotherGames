using System;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    Rigidbody2D rigid;

    public float fallingSpeed;
    public bool successBonus;

    Action<bool> onSuccessBonus;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigid.position += Vector2.up * fallingSpeed * Time.deltaTime;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBonus"))
        {
            Debug.Log("OnTrigger");
            onSuccessBonus?.Invoke(successBonus);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            successBonus = false;
        }
    }

    public void SetFallingSpeed(float fallingSpeed)
    {
        this.fallingSpeed = fallingSpeed;
    }

    public void Set(Action<bool> onSuccess)
    {
        onSuccessBonus = onSuccess;
        successBonus = true;
    }
}

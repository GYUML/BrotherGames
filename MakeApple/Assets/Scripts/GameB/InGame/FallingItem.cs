using System;
using UnityEngine;

public class FallingItem : MonoBehaviour
{
    int code;
    float fallingSpeed;
    Vector2 minRange;
    Vector2 maxRange;
    Action<GameObject> returnToPool;
    Action<int> onEnterPlayer;

    private void Update()
    {
        transform.position += Vector3.up * fallingSpeed * Time.deltaTime;
        if (transform.position.y < minRange.y || transform.position.y > maxRange.y)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ReturnToPool();
            onEnterPlayer?.Invoke(code);
        }
    }

    public void Set(int code, float fallingSpeed, Vector2 minRange, Vector2 maxRange, Action<GameObject> returnToPool, Action<int> onEnterPlayer)
    {
        this.code = code;
        this.fallingSpeed = fallingSpeed;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.returnToPool = returnToPool;
        this.onEnterPlayer = onEnterPlayer;
    }

    public void SetFallingSpeed(float fallingSpeed)
    {
        this.fallingSpeed = fallingSpeed;
    }

    public void ReturnToPool()
    {
        returnToPool?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}

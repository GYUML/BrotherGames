using System;
using UnityEngine;

public class BaseHitBox : MonoBehaviour
{
    Action<Vector2, float> onAttacked;

    public void SetOnAttackedEvent(Action<Vector2, float> onAttacked)
    {
        this.onAttacked = onAttacked;
    }

    public void OnAttacked(Vector2 direction, float stunTime)
    {
        onAttacked?.Invoke(direction, stunTime);
    }
}

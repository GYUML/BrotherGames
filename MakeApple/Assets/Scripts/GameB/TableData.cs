using UnityEngine;

public class TableData : MonoBehaviour
{
    public long GetAttackPerLevel(int level)
    {
        return 10L + level * 5;
    }

    public float GetBonusAttackPer(float startHeight, float normalFallingSpeed, float recordTime)
    {
        if (normalFallingSpeed == 0)
        {
            Debug.LogError("[TableData] GetBonusAttackPer() normalFalling speed can't be zero.");
            return 0f;
        }

        var bestTime = startHeight / (normalFallingSpeed * 2);
        var normalTime = startHeight / normalFallingSpeed;

        if (normalTime == bestTime)
            return 0f;

        if (recordTime <= bestTime)
            return 1f;
        else if (recordTime < normalTime)
            return 1f - (recordTime - bestTime) / (normalTime - bestTime);

        return 0f;
    }
}

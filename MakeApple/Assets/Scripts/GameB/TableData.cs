using UnityEngine;

namespace GameB
{
    public class TableData : MonoBehaviour
    {
        public long GetAttackPerLevel(int level)
        {
            return 10L + level * 5;
        }

        public double GetBonusAttackPer(float startHeight, float normalFallingSpeed, float recordTime)
        {
            if (normalFallingSpeed == 0)
            {
                Debug.LogError("[TableData] GetBonusAttackPer() normalFalling speed can't be zero.");
                return 0d;
            }

            var bestTime = startHeight / (normalFallingSpeed * 2);
            var normalTime = startHeight / normalFallingSpeed;

            if (normalTime == bestTime)
                return 0d;

            if (recordTime <= bestTime)
                return 1d;
            else if (recordTime < normalTime)
                return 1d - (recordTime - bestTime) / (normalTime - bestTime);

            return 0d;
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace GameC
{
    public class TableData : MonoBehaviour
    {
        StageType[] stageInfo = new StageType[] { StageType.None, StageType.Battle, StageType.Battle, StageType.Blessing, StageType.Battle, StageType.Battle, StageType.Battle,
            StageType.Battle, StageType.Battle, StageType.Recovery, StageType.Battle, StageType.Battle, StageType.Battle, StageType.Battle, StageType.Battle, StageType.Battle};


        public StageType GetStageType(int stage)
        {
            return stageInfo[stage];
        }

        public bool IsLastStage(int stage)
        {
            return stage >= stageInfo.Length;
        }
    }
}

using TMPro;
using UnityEngine;

namespace GameE
{
    public class StateLayout : MonoBehaviour
    {
        public GuageBar expGuage;
        public TMP_Text levelText;

        public void SetExpGuage(long maxExp, long nowExp)
        {
            expGuage.SetGuage(maxExp, nowExp);
        }

        public void SetLevel(int level)
        {
            levelText.text = level.ToString();
        }
    }
}

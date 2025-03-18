using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameB
{
    public class FallingGameLayout : UILayout
    {
        public TMP_Text heightText;
        public TMP_Text coinText;
        public TMP_Text speedText;
        public TMP_Text attackText;
        public Image expGuage;
        public TMP_Text expText;
        public TMP_Text bonusAttack;

        public void SetHeight(double height)
        {
            heightText.text = $"{height * 40f:F1}m";
        }

        public void SetCoin(int coin)
        {
            coinText.text = $"{coin}";
        }

        public void SetSpeed(double speed)
        {
            speedText.text = $"{speed * 40f:F1}m/s";
        }

        public void SetAttack(long attack)
        {
            attackText.text = $"{attack}";
        }

        public void SetExp(long max, long now)
        {
            expGuage.fillAmount = (float)now / max;
            expText.text = $"{now.ToFormat()}/{max.ToFormat()}";
        }

        public void SetBonusAttack(double bonusAttack)
        {
            this.bonusAttack.text = $"{bonusAttack * 100:F1}%";
        }
    }

}

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameC
{
    public class BattleLayout : UILayout
    {
        public Image mpGuage;
        public Image myHpGuage;
        public Image enemyHpGuage;
        public Image expGuage;
        public TMP_Text damageText;
        public TMP_Text levelText;
        public TMP_Text attackText;

        public KButton chargeButton;
        public KButton nextButton;
        public KButton startButton;

        private void Start()
        {
            nextButton.onClick.AddListener(() => Managers.Logic.GetComponent<GameLogic>().NextStage());
            startButton.onClick.AddListener(() => Managers.Logic.GetComponent<GameLogic>().StartGame());
        }

        public void SetChargeGuage(float nowCharge, float maxCharge)
        {
            if (maxCharge == 0)
                return;

            mpGuage.fillAmount = nowCharge / maxCharge;
        }

        public void SetChargeGuageColor(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
                mpGuage.color = color;
        }

        public void SetExpGuage(long nowExp, long maxExp)
        {
            if (maxExp == 0)
                return;

            expGuage.fillAmount = (float)nowExp / maxExp;
        }

        public void SetMyHpGuage(long nowHp, long maxHp)
        {
            if (maxHp == 0)
                return;

            myHpGuage.fillAmount = (float)nowHp / maxHp;
        }

        public void SetEnemyHpGuage(long nowHp, long maxHp)
        {
            if (maxHp == 0)
                return;

            enemyHpGuage.fillAmount = (float)nowHp / maxHp;
        }

        public void ShowDamage(long damage)
        {
            Debug.Log($"ShowDamage() damage={damage}");
        }

        public void ShowChargeButton()
        {
            startButton.gameObject.SetActive(false);
            chargeButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        public void ShowNextButton()
        {
            startButton.gameObject.SetActive(false);
            chargeButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }

        public void ShowStartButton()
        {
            startButton.gameObject.SetActive(true);
            chargeButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }

        public void HideButtons()
        {
            startButton.gameObject.SetActive(false);
            chargeButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }

        public void SetLevelText(int level)
        {
            levelText.text = $"Lv.{level}";
        }

        public void SetAttackText(long attack)
        {
            attackText.text = $"{attack}";
        }
    }
}

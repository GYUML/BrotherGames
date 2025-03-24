using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameC
{
    public class BattleLayout : UILayout
    {
        public Image mpGuage;
        public Image myHpGuage;
        public Image enemyHpGuage;
        public Image expGuage;
        public TMP_Text levelText;
        public TMP_Text attackText;

        public KButton defaultButton;

        public ScrollRect logScrollRect;
        public TMP_Text logText;

        private void Start()
        {
            logText.gameObject.SetActive(false);
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

        public void SetButton(string buttonText, UnityAction onClick)
        {
            defaultButton.GetComponentInChildren<TMP_Text>().text = buttonText;
            defaultButton.onClick.RemoveAllListeners();
            if (onClick != null)
                defaultButton.onClick.AddListener(onClick);
        }

        public void SetButtonEnable(bool enable)
        {
            defaultButton.enabled = enable;
        }

        public void SetLevelText(int level)
        {
            levelText.text = $"Lv.{level}";
        }

        public void SetAttackText(long attack)
        {
            attackText.text = $"{attack}";
        }

        public void AddLogText(string log)
        {
            var newLog = Instantiate(logText, logText.transform.parent);
            newLog.text = log;
            newLog.gameObject.SetActive(true);
            logScrollRect.normalizedPosition = Vector2.zero;
        }
    }
}

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
        public Image expGuage;
        public TMP_Text levelText;
        public TMP_Text attackText;
        public TMP_Text chargedText;

        public EventButton defaultButton;

        public ScrollRect logScrollRect;
        public TMP_Text logText;

        private void Start()
        {
            defaultButton.onDown.AddListener(() => Managers.Logic.GetComponent<GameLogic>().OnCharging(true));
            defaultButton.onUp.AddListener(() => Managers.Logic.GetComponent<GameLogic>().OnCharging(false));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                defaultButton.onDown?.Invoke();
            if (Input.GetKeyUp(KeyCode.Space))
                defaultButton.onUp?.Invoke();
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

        public void SetChargeText(float charge)
        {
            chargedText.text = $"{charge:F1}%";
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
            logText.text = logText.text + "\n" + log;
            logScrollRect.normalizedPosition = Vector2.zero;
        }
    }
}

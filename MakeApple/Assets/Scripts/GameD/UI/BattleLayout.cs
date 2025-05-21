using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameD
{
    public class BattleLayout : UILayout
    {
        public Image mpGuage;
        public TMP_Text enemyName;
        public TMP_Text successPossibility;
        public TMP_Text money;
        public TMP_Text rewardMoney;

        public KButton attackButton;
        public KButton exitButton;
        public KButton upgradeButton;

        public GameLogic gameLogic;

        private void Start()
        {
            attackButton.GetComponent<EventButton>().onDown.AddListener(() => gameLogic.OnChargeButton(true));
            attackButton.GetComponent<EventButton>().onUp.AddListener(() => gameLogic.OnChargeButton(false));
            exitButton.onClick.AddListener(() => gameLogic.ExitBattle());
            upgradeButton.onClick.AddListener(() => gameLogic.UpgradeLevel());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                attackButton.GetComponent<EventButton>().onDown?.Invoke();
            else if (Input.GetKeyUp(KeyCode.Space))
                attackButton.GetComponent<EventButton>().onUp?.Invoke();
        }

        public void SetChargeGuage(float nowCharge, float maxCharge)
        {
            if (maxCharge == 0)
                return;

            mpGuage.fillAmount = nowCharge / maxCharge;
        }

        public void SetEnemyName(string name)
        {
            enemyName.text = name;
        }

        public void SetSuccessPossibility(float minPossibility, float maxPossibility)
        {
            successPossibility.text = $"{minPossibility * 100f:F1}% ~ {maxPossibility * 100f:F1}%";
        }

        public void SetMoney(double money)
        {
            this.money.text = money.ToBig();
        }

        public void SetRewardMoney(double rewardMoney)
        {
            this.rewardMoney.text = rewardMoney.ToBig();
        }

        public void SetButton(string buttonText, UnityAction onClick)
        {
            attackButton.GetComponentInChildren<TMP_Text>().text = buttonText;
            attackButton.onClick.RemoveAllListeners();
            if (onClick != null)
                attackButton.onClick.AddListener(onClick);
        }

        public void SetButtonEnable(bool enable)
        {
            attackButton.enabled = enable;
        }
    }
}

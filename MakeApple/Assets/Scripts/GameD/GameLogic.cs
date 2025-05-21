using UnityEngine;

namespace GameD
{
    public class GameLogic : MonoBehaviour
    {
        public BattleLayout battleLayout;
        public BattleMap battleMap;

        float maxChargeTime = 0.5f;
        float nowChargeTime = 0f;
        float charged = 0f;
        float maxBonusPossibility = 0.1f;

        int enemyLevel;
        double money;
        int playerLevel;
        bool isChargeKeyDown;

        private void Start()
        {
            Application.targetFrameRate = 60;

            battleLayout.SetEnemyName($"Lv.{enemyLevel}\nTung Tung Tung Sahur");
            battleLayout.SetSuccessPossibility(GetSuccessPossibility(enemyLevel, playerLevel), GetMaxSuccessPossibility(enemyLevel, playerLevel));
            battleLayout.SetMoney(money);
            battleLayout.SetRewardMoney(GetRewardMoney(enemyLevel));
        }

        private void Update()
        {
            OnUpdateBattle();
        }

        public void OnChargeButton(bool isKeyDown)
        {
            isChargeKeyDown = isKeyDown;
        }

        public void ExitBattle()
        {
            money += GetRewardMoney(enemyLevel);
            enemyLevel = 0;
            battleLayout.SetEnemyName($"Lv.{enemyLevel}\nTung Tung Tung Sahur");
            battleLayout.SetSuccessPossibility(GetSuccessPossibility(enemyLevel, playerLevel), GetMaxSuccessPossibility(enemyLevel, playerLevel));
            battleLayout.SetMoney(money);
            battleLayout.SetRewardMoney(GetRewardMoney(enemyLevel));
        }

        public void UpgradeLevel()
        {
            var upgradeCost = Mathf.Pow(2f, playerLevel) * 1000f;
            if (money > upgradeCost)
            {
                money -= upgradeCost;
                playerLevel++;
                battleLayout.SetSuccessPossibility(GetSuccessPossibility(enemyLevel, playerLevel), GetMaxSuccessPossibility(enemyLevel, playerLevel));
                battleLayout.SetMoney(money);
                battleLayout.SetRewardMoney(GetRewardMoney(enemyLevel));
            }
        }

        void OnUpdateBattle()
        {
            if (isChargeKeyDown)
            {
                if (maxChargeTime == 0)
                    return;

                nowChargeTime += Time.deltaTime;

                if (nowChargeTime < maxChargeTime)
                {
                    charged = nowChargeTime / maxChargeTime;
                }
                else if (nowChargeTime < 2 * maxChargeTime)
                {
                    charged = (2 * maxChargeTime - nowChargeTime) / maxChargeTime;
                }
                else
                {
                    nowChargeTime -= 2 * maxChargeTime;
                    charged = nowChargeTime / maxChargeTime;
                }
            }
            else
            {
                if (charged > 0f)
                {
                    Debug.Log($"Update() charged = {charged}");
                    battleLayout.SetChargeGuage(charged, 1f);
                    AttackEnemy(charged);
                    nowChargeTime = 0f;
                    charged = 0f;

                    return;
                }
            }

            battleLayout.SetChargeGuage(charged, 1f);
        }

        void AttackEnemy(float charged)
        {
            var defaultPossibility = GetSuccessPossibility(enemyLevel, playerLevel);
            var bonusPossibility = charged * maxBonusPossibility;
            var rand = Random.Range(0f, 1f);
            var isSuccess = rand < defaultPossibility + bonusPossibility;

            if (isSuccess)
            {
                Debug.Log($"AttackEenemy() success");
                enemyLevel++;
            }
            else
            {
                Debug.Log($"AttackEenemy() failed");
                enemyLevel = 0;
            }

            battleMap.PlayerAttack(isSuccess);
            battleLayout.SetEnemyName($"Lv.{enemyLevel}\nTung Tung Tung Sahur");
            battleLayout.SetSuccessPossibility(GetSuccessPossibility(enemyLevel, playerLevel), GetMaxSuccessPossibility(enemyLevel, playerLevel));
            battleLayout.SetMoney(money);
            battleLayout.SetRewardMoney(GetRewardMoney(enemyLevel));
        }

        float GetSuccessPossibility(int enemyLevel, int playerLevel)
        {
            return Mathf.Pow(0.9f, enemyLevel) * (1f + playerLevel * 0.2f);
        }

        float GetMaxSuccessPossibility(int enemyLevel, int playerLevel)
        {
            return GetSuccessPossibility(enemyLevel, playerLevel) + maxBonusPossibility;
        }

        double GetRewardMoney(int enemyLevel)
        {
            return enemyLevel > 0 ? 1000d * Mathf.Pow(2, enemyLevel) : 0;
        }
    }
}

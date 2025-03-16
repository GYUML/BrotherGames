using UnityEngine;

namespace GameC
{
    public class GameLogic : MonoBehaviour
    {
        public float maxMp;
        public float nowMp;

        public float mpCost;
        public float mpRecovery;

        public BattleMap battleMap;

        bool isCharging;
        bool chargeLock;
        float chargedMp;
        float enemyAttackTimer;

        long enemyMaxHp = 50;
        long enemyHp = 50;
        long myMaxHp = 500;
        long myHp = 500;
        long myExp = 0;
        int myLevel = 1;

        float movingEndTime;

        int stage = 1;

        GameState nowState;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetState(GameState.GameReady);
        }

        private void Update()
        {
            if (nowState == GameState.Moving)
                OnUpdateMovingState();
            else if (nowState == GameState.Battle)
                OnUpdateBattleState();
            else if (nowState == GameState.BattleResult)
                OnUpdateBattleResultState();
        }

        public void StartGame()
        {
            stage = 0;
            NextStage();
        }

        public void SetState(GameState state)
        {
            nowState = state;

            if (state == GameState.GameReady)
            {
                Managers.UI.GetLayout<BattleLayout>().ShowStartButton();
            }
            else if (state == GameState.Moving)
            {
                movingEndTime = Time.time + 3f;
                battleMap.Move();
            }
            else if (state == GameState.Battle)
            {
                enemyMaxHp = stage * 50;
                enemyHp = enemyMaxHp;

                Managers.UI.GetLayout<BattleLayout>().ShowChargeButton();
            }
            else if (state == GameState.BattleResult)
            {
                Managers.UI.GetLayout<BattleLayout>().ShowNextButton();
            }
        }

        public void NextStage()
        {
            var lastStage = 50;
            if (stage < lastStage)
            {
                stage++;
                SetState(GameState.Moving);
            }
            else
            {
                Debug.Log("Game End");
            }
        }

        void OnUpdateBattleState()
        {
            isCharging = Input.GetKey(KeyCode.Space);

            if (!chargeLock && isCharging)
            {
                nowMp -= mpCost * Time.deltaTime;
                chargedMp += mpCost * Time.deltaTime;
            }
            else
            {
                if (chargedMp > 0)
                {
                    var chargingRate = chargedMp / maxMp;
                    var damage = (long)(chargingRate * chargingRate * chargingRate * 100f);
                    enemyHp -= damage;

                    Managers.UI.GetLayout<BattleLayout>().ShowDamage(damage);
                    //player.SetTrigger("2_Attack");

                    if (enemyHp <= 0)
                    {
                        myExp += 10;

                        while (myExp >= myLevel * 50)
                        {
                            myExp -= myLevel * 50;
                            myLevel++;
                        }

                        battleMap.EnemyDie();
                        Managers.UI.GetLayout<BattleLayout>().SetExpGuage(myExp, myLevel * 50);
                        Managers.UI.GetLayout<BattleLayout>().SetLevelText(myLevel);
                        SetState(GameState.BattleResult);
                    }
                }

                nowMp += mpRecovery * Time.deltaTime;
                chargedMp = 0;
            }

            if (nowMp <= 0)
            {
                chargeLock = true;
                nowMp = 0;
                chargedMp = 0;

                Managers.UI.GetLayout<BattleLayout>().SetChargeGuageColor("#8A8A8A");
            }
            else if (nowMp >= maxMp)
            {
                chargeLock = false;
                nowMp = maxMp;

                Managers.UI.GetLayout<BattleLayout>().SetChargeGuageColor("#E25656");
            }

            if (enemyHp > 0 && Time.time > enemyAttackTimer)
            {
                enemyAttackTimer = Time.time + 1f;
                myHp -= 10;
                //enemy.SetTrigger("2_Attack");
            }

            Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(nowMp, maxMp);
            Managers.UI.GetLayout<BattleLayout>().SetMyHpGuage(myHp, myMaxHp);
            Managers.UI.GetLayout<BattleLayout>().SetEnemyHpGuage(enemyHp, enemyMaxHp);
        }

        void OnUpdateMovingState()
        {
            if (Time.time > movingEndTime)
            {
                SetState(GameState.Battle);
            }
        }

        void OnUpdateBattleResultState()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                NextStage();
        }

        public enum GameState
        {
            None,
            GameReady,
            Moving,
            Battle,
            BattleResult,
        }
    }
}

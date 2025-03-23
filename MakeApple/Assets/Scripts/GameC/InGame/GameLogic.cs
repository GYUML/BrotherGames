using System.Collections;
using System.Collections.Generic;
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
        public TableData tableData;

        bool isCharging;
        bool chargeLock;
        float chargedMp;

        long enemyMaxHp = 50;
        long enemyHp = 50;
        long myMaxHp = 500;
        long myHp = 500;
        long myExp = 0;
        int myLevel = 1;
        long myAttack = 10;

        int nowStage = 1;
        List<Coroutine> nowProcList = new List<Coroutine>();

        GameState nowState;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetState(GameState.GameReady);
        }

        public void StartGame()
        {
            nowStage = 0;
            NextStage();
        }

        public void NextStage()
        {
            nowStage++;

            if (tableData.IsLastStage(nowStage))
            {
                Debug.Log("Game End");
            }
            else
            {
                battleMap.SetStage(tableData.GetStageType(nowStage));
                SetState(GameState.Moving);
            }
        }

        public void SetState(GameState state)
        {
            nowState = state;

            foreach (var proc in nowProcList)
            {
                if (proc != null)
                    StopCoroutine(proc);
            }

            nowProcList.Clear();

            if (state == GameState.GameReady)
                nowProcList.Add(StartCoroutine(GameReadyProc()));
            else if (state == GameState.Moving)
                nowProcList.Add(StartCoroutine(MovingProc()));
            else if (state == GameState.StageProgress)
                nowProcList.Add(StartCoroutine(StageProgressProc()));
            else if (state == GameState.StageResult)
                nowProcList.Add(StartCoroutine(StageResultProc()));
        }

        IEnumerator GameReadyProc()
        {
            Managers.UI.GetLayout<BattleLayout>().SetButton("Start", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartGame();
                    yield break;
                }

                yield return null;
            }
        }

        IEnumerator MovingProc()
        {
            battleMap.Move();
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(false);

            yield return new WaitForSeconds(3f);

            SetState(GameState.StageProgress);
        }

        IEnumerator StageProgressProc()
        {
            var stageType = tableData.GetStageType(nowStage);

            if (stageType == StageType.Battle)
            {
                nowProcList.Add(StartCoroutine(BattleProc()));
                nowProcList.Add(StartCoroutine(BattleEnemyAttackProc()));
            }
            else if (stageType == StageType.Recovery)
            {
                nowProcList.Add(StartCoroutine(RecoveryProc()));
            }
            else if (stageType == StageType.Blessing)
            {
                nowProcList.Add(StartCoroutine(BlessingProc()));
            }

            yield break;
        }

        IEnumerator StageResultProc()
        {
            nowMp = maxMp;
            chargedMp = 0;

            Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(nowMp, maxMp);
            Managers.UI.GetLayout<BattleLayout>().SetButton("Next", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NextStage();
                    yield break;
                }
                
                yield return null;
            }
        }

        IEnumerator BattleProc()
        {
            enemyMaxHp = nowStage * 50;
            enemyHp = enemyMaxHp;

            Managers.UI.GetLayout<BattleLayout>().SetButton("Charge", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
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
                        var damage = (long)(chargingRate * chargingRate * 100f);
                        enemyHp -= damage;

                        Managers.UI.GetLayout<BattleHudLayout>().SpawnDamageText(damage, new Vector3(0.5f, 0.5f, 0f));
                        battleMap.PlayerAttack();

                        if (enemyHp <= 0)
                        {
                            myExp += (10 + nowStage);

                            while (myExp >= myLevel * 50)
                            {
                                myExp -= myLevel * 50;
                                myLevel++;
                                myAttack += 2;
                            }

                            battleMap.EnemyDie();
                            Managers.UI.GetLayout<BattleLayout>().SetExpGuage(myExp, myLevel * 50);
                            Managers.UI.GetLayout<BattleLayout>().SetLevelText(myLevel);
                            Managers.UI.GetLayout<BattleLayout>().SetAttackText(myAttack);
                            SetState(GameState.StageResult);
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

                Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(nowMp, maxMp);
                Managers.UI.GetLayout<BattleLayout>().SetMyHpGuage(myHp, myMaxHp);
                Managers.UI.GetLayout<BattleLayout>().SetEnemyHpGuage(enemyHp, enemyMaxHp);

                yield return null;
            }
        }

        IEnumerator BattleEnemyAttackProc()
        {
            while (enemyHp > 0)
            {
                myHp -= 10;
                battleMap.EnemyAttack();

                yield return new WaitForSeconds(1f);
            }
        }

        IEnumerator RecoveryProc()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    myHp += 300;
                    SetState(GameState.StageResult);
                    Managers.UI.GetLayout<BattleLayout>().SetMyHpGuage(myHp, myMaxHp);

                    yield break;
                }

                yield return null;
            }
        }

        IEnumerator BlessingProc()
        {
            var success = 0;

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    var rand = Random.Range(0f, 1f);
                    Debug.Log(rand);
                    if (rand < Mathf.Max(1f - success * 0.2f, 0.2f))
                        success++;
                    else
                    {
                        success = 0;
                        break;
                    }
                        
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    break;
                }

                yield return null;
            }

            myAttack += success * 2;
            SetState(GameState.StageResult);
        }

        public enum GameState
        {
            None,
            GameReady,
            Moving,
            StageProgress,
            StageResult
        }
    }
}

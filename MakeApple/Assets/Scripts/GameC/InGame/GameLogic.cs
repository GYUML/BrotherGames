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
        StageType nowStageType;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetState(GameState.GameReady);
            battleMap.SpawnPlayer(myMaxHp, myHp);
        }

        public void StartGame()
        {
            nowStage = 0;
            NextStage();
        }

        public void NextStage()
        {
            nowStage++;

            //if (tableData.IsLastStage(nowStage))
            if (nowStage > 50)
            {
                Debug.Log("Game End");
            }
            else
            {
                var rand = Random.Range(0f, 1f);
                if (rand < 0.8f) nowStageType = StageType.Battle;
                else if (rand < 0.9f) nowStageType = StageType.Blessing;
                else nowStageType = StageType.Recovery;

                if (nowStageType == StageType.Battle)
                {
                    enemyMaxHp = nowStage * 200;
                    enemyHp = enemyMaxHp;
                    battleMap.SpawnUnit(enemyMaxHp, enemyHp);
                }

                battleMap.SetStage(nowStageType);
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

        public void OnCharging(bool isKeyDown)
        {
            isCharging = isKeyDown;
        }

        IEnumerator GameReadyProc()
        {
            Managers.UI.GetLayout<BattleLayout>().SetButton("Start", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(0f, 1f);
            Managers.UI.GetLayout<BattleLayout>().SetChargeText(0f);

            while (true)
            {
                if (isCharging)
                {
                    isCharging = false;
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

            yield return new WaitForSeconds(1f);

            SetState(GameState.StageProgress);
        }

        IEnumerator StageProgressProc()
        {
            //var stageType = tableData.GetStageType(nowStage);
            var stageType = nowStageType;

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
            nowMp = 0f;
            chargedMp = 0f;

            Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(nowMp, maxMp);
            Managers.UI.GetLayout<BattleLayout>().SetButton("Next", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
            {
                if (isCharging)
                {
                    isCharging = false;
                    NextStage();
                    yield break;
                }
                
                yield return null;
            }
        }

        IEnumerator BattleProc()
        {
            Managers.UI.GetLayout<BattleLayout>().SetButton("Charge", null);
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);
            Managers.UI.GetLayout<BattleLayout>().AddLogText("Meet enemy.");

            while (true)
            {
                if (isCharging)
                {
                    chargedMp += mpCost * Time.deltaTime;
                    chargedMp %= 2 * maxMp;
                    nowMp = chargedMp > maxMp ? 2 * maxMp - chargedMp : chargedMp;
                }
                else
                {
                    if (chargedMp > 0)
                    {
                        var chargingRate = nowMp / maxMp;
                        var damage = (long)(chargingRate * chargingRate * 100f);
                        enemyHp -= damage;

                        battleMap.PlayerAttack();
                        battleMap.OnDamaged(enemyMaxHp, enemyHp, damage);

                        if (enemyHp <= 0)
                        {
                            myExp += (10 + nowStage * 5);

                            while (myExp >= myLevel * 30)
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

                    chargedMp = 0;
                    nowMp = 0;
                }

                Managers.UI.GetLayout<BattleLayout>().SetChargeGuage(nowMp, maxMp);
                Managers.UI.GetLayout<BattleLayout>().SetMyHpGuage(myHp, myMaxHp);
                if (isCharging)
                    Managers.UI.GetLayout<BattleLayout>().SetChargeText(nowMp / maxMp * 100f);

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
            Managers.UI.GetLayout<BattleLayout>().AddLogText("Meet fountain of recovery.");
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
            {
                if (isCharging)
                {
                    isCharging = false;
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
            Managers.UI.GetLayout<BattleLayout>().AddLogText("Meet statue of goddess.");
            Managers.UI.GetLayout<BattleLayout>().SetButtonEnable(true);

            while (true)
            {
                if (isCharging)
                {
                    isCharging = false;
                    var rand = Random.Range(0f, 1f);
                    Debug.Log(rand);
                    if (rand < Mathf.Max(1f - success * 0.2f, 0.2f))
                    {
                        success++;
                        Managers.UI.GetLayout<BattleLayout>().AddLogText($"The prayer was successful.\n You can get +{success * 2} ATK");
                    }
                    else
                    {
                        success = 0;
                        Managers.UI.GetLayout<BattleLayout>().AddLogText("The goddess was angry with greed.");
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

        public struct Stat
        {
            public long hp;
            public long attack;
        }
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameC
{
    public class BattleLayout : MonoBehaviour
    {
        public Image mpGuage;
        public Image myHpGuage;
        public Image enemyHpGuage;
        public Image expGuage;
        public TMP_Text damageText;

        public Animator player;
        public Animator enemy;

        public float maxMp;
        public float nowMp;

        public float mpCost;
        public float mpRecovery;
        public float mpCostFactor;

        bool isCharging;
        bool chargeLock;
        float chargingTime;

        long enemyMaxHp = 500;
        long enemyHp = 500;
        long myMaxHp = 500;
        long myHp = 500;
        long myExp = 0;
        int myLevel = 1;

        float enemyAttackTimer;

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            isCharging = Input.GetKey(KeyCode.Space);

            if (!chargeLock && isCharging)
            {
                nowMp -= mpCost * Time.deltaTime;
                chargingTime += Time.deltaTime;
            }
            else
            {
                nowMp += mpRecovery * Time.deltaTime;

                if (chargingTime > 0)
                {
                    var chargingRate = chargingTime * 2f;
                    var damage = (long)(chargingRate * chargingRate * chargingRate * 100f);
                    enemyHp -= damage;
                    chargingTime = 0;

                    damageText.text = damage.ToString();
                    damageText.rectTransform.DOKill();
                    damageText.rectTransform.anchoredPosition = new Vector2(damageText.rectTransform.anchoredPosition.x, -227f);
                    damageText.rectTransform.DOAnchorPosY(0f, 1f);
                    player.SetTrigger("2_Attack");

                    if (enemyHp <= 0)
                    {
                        myExp += 10;

                        while (myExp >= myLevel * 50)
                        {
                            myExp -= myLevel * 50;
                            myExp = 0;
                            myLevel++;
                            expGuage.fillAmount = (float)myExp / (myLevel * 50);
                        }

                        enemyHp = enemyMaxHp;
                    }
                }
            }

            if (enemyHp > 0 && Time.time > enemyAttackTimer)
            {
                enemyAttackTimer = Time.time + 1f;
                myHp -= 10;
                enemy.SetTrigger("2_Attack");
            }

            if (nowMp <= 0)
            {
                //Debug.Log(chargingTime);
                chargeLock = true;
                chargingTime = 0;

                if (ColorUtility.TryParseHtmlString("#8A8A8A", out var color))
                    mpGuage.color = color;
            }
            else if (nowMp >= maxMp)
            {
                chargeLock = false;

                if (ColorUtility.TryParseHtmlString("#E25656", out var color))
                    mpGuage.color = color;
            }
            
            nowMp = Mathf.Clamp(nowMp, 0, maxMp);
            mpGuage.fillAmount = nowMp / maxMp;
            myHpGuage.fillAmount = (float)myHp / myMaxHp;
            enemyHpGuage.fillAmount = (float)enemyHp / enemyMaxHp;
        }
    }
}

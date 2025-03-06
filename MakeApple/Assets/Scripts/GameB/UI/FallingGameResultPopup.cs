using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameB
{
    public class FallingGameResultPopup : UIPopup
    {
        public Image hpGuage;
        public TMP_Text damageText;
        public KButton homeButton;

        public void StartEndEffect(long max, long now)
        {
            StartCoroutine(ReduceHpGuage(max, now));
        }

        IEnumerator ReduceHpGuage(long max, long now)
        {
            var reduceTime = 1f;
            var reduceTimer = reduceTime;
            var endValue = (float)now / max;

            damageText.gameObject.SetActive(false);
            homeButton.gameObject.SetActive(false);

            while (reduceTimer > 0)
            {
                hpGuage.fillAmount = Mathf.Lerp(endValue, 1f, reduceTimer / reduceTime);
                yield return null;
                reduceTimer -= Time.deltaTime;
            }

            hpGuage.fillAmount = endValue;

            damageText.gameObject.SetActive(true);
            damageText.text = $"{max - now}";
            damageText.rectTransform.anchoredPosition = new Vector2(0f, -140f);
            damageText.rectTransform.DOAnchorPosY(410f, 0.3f).OnComplete(() => homeButton.gameObject.SetActive(true));
        }
    }
}


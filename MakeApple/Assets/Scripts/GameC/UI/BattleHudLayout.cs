using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameC
{
    public class BattleHudLayout : UILayout
    {
        public TMP_Text damageTextPrefab;

        Stack<TMP_Text> damageTextPool = new Stack<TMP_Text>();

        Camera mainCamera;
        RectTransform rectTransform;

        private void Awake()
        {
            mainCamera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        public void SpawnDamageText(long damage, Vector3 position)
        {
            if (damageTextPool.Count == 0)
                damageTextPool.Push(Instantiate(damageTextPrefab, damageTextPrefab.transform.parent));

            var anchoredPosition = WorldToAnchored(position, rectTransform);

            var damageText = damageTextPool.Pop();
            damageText.gameObject.SetActive(true);
            damageText.text = damage.ToString();
            damageText.alpha = 1f;
            damageText.rectTransform.anchoredPosition = anchoredPosition;
            damageText.rectTransform.DOAnchorPosY(anchoredPosition.y + 150f, 0.5f);
            damageText.DOFade(0f, 0.5f)
                .SetEase(Ease.InCirc)
                .OnComplete(() => damageTextPool.Push(damageText));
        }

        Vector2 WorldToAnchored(Vector3 worldPos, RectTransform parentRect)
        {
            var screenPoint = mainCamera.WorldToScreenPoint(worldPos);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out var anchoredPos))
                return anchoredPos;

            return Vector2.zero;
        }
    }
}

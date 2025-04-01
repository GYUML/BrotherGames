using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameC
{
    public class BattleHudLayout : UILayout
    {
        public TMP_Text damageTextPrefab;
        public GuageBar hpGuagePrefab;

        Stack<TMP_Text> damageTextPool = new Stack<TMP_Text>();
        Stack<GuageBar> hpGuagePool = new Stack<GuageBar>();

        Dictionary<Transform, GuageBar> hpGuageTargetDic = new Dictionary<Transform, GuageBar>();

        Camera mainCamera;
        RectTransform rectTransform;

        private void Awake()
        {
            mainCamera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            foreach (var entry in hpGuageTargetDic)
            {
                var target = entry.Key;
                var hpGuage = entry.Value;
                var anchoredPosition = WorldToAnchored(target.transform.position, rectTransform);
                anchoredPosition.y += 100;
                hpGuage.SetPosition(anchoredPosition);
            }
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

        public void RegisterHpBarTarget(Transform targetTransform)
        {
            if (!hpGuageTargetDic.ContainsKey(targetTransform))
            {
                var hpGuage = hpGuagePool.Count > 0 ? hpGuagePool.Pop() : Instantiate(hpGuagePrefab, hpGuagePrefab.transform.parent);
                hpGuage.gameObject.SetActive(true);
                hpGuageTargetDic.Add(targetTransform, hpGuage);
            }
            else
            {
                Debug.LogError($"[BattleHudLayout] Aready registerd. targetTransform={targetTransform}");
            }
        }

        public void DeleteHpBarTarget(Transform targetTransform)
        {
            if (hpGuageTargetDic.ContainsKey(targetTransform))
            {
                var hpGuage = hpGuageTargetDic[targetTransform];
                hpGuage.gameObject.SetActive(false);
                hpGuagePool.Push(hpGuage);
                hpGuageTargetDic.Remove(targetTransform);
            }
            else
            {
                Debug.LogError($"[BattleHudLayout] Not found target. targetTransform={targetTransform}");
            }
        }

        public void UpdateHpBar(Transform target, long maxHp, long nowHp)
        {
            if (hpGuageTargetDic.TryGetValue(target, out var hpGuage))
                hpGuage.SetGuage(maxHp, nowHp);
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

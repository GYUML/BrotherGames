using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameE
{
    public class HudLayout : MonoBehaviour
    {
        public HudLayoutItem healthBarPrefab;
        public TMP_Text damageTextPrefab;

        Stack<HudLayoutItem> healthBarPool = new Stack<HudLayoutItem>();
        Dictionary<Transform, HudLayoutItem> healthBarDic = new Dictionary<Transform, HudLayoutItem>();
        Stack<TMP_Text> damageTextPool = new Stack<TMP_Text>();

        Camera mainCamera;
        RectTransform rectTransform;

        private void OnEnable()
        {
            mainCamera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateHealthBar(Transform target, long maxHp, long nowHp)
        {
            if (!healthBarDic.ContainsKey(target))
                CreateHealthBar(target, 1f);

            if (healthBarDic.ContainsKey(target))
            {
                var guage = healthBarDic[target].GetComponent<GuageBar>();
                if (guage != null)
                    guage.SetGuage(maxHp, nowHp);
            }
            else
            {
                Debug.LogError("[HudLayout] Failed to create healthBar");
            }
        }

        public void CreateHealthBar(Transform target, float offsetY = 1f)
        {
            var healthBar = healthBarPool.Count > 0 ? healthBarPool.Pop() : Instantiate(healthBarPrefab, transform);
            healthBar.gameObject.SetActive(true);
            healthBar.Init(target, offsetY, HealthBarToPool);
            healthBarDic.Add(target, healthBar);
        }

        public void SpawnDamageText(long[] damages, Vector3 position)
        {
            var randomPos = position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.4f, 0.6f), 0f);
            StartCoroutine(SpawnDamageTextCo(damages, randomPos));
        }

        IEnumerator SpawnDamageTextCo(long[] damages, Vector3 position)
        {
            var anchoredPosition = WorldToAnchored(position, rectTransform);
            var diff = 50f;

            foreach (var damage in damages)
            {
                SpawnDamageText(damage, anchoredPosition);
                anchoredPosition.y += diff;
                yield return new WaitForSeconds(0.1f);
            }
        }

        void HealthBarToPool(HudLayoutItem item)
        {
            item.gameObject.SetActive(false);
            healthBarPool.Push(item);
            healthBarDic.Remove(item.GetTarget());
        }

        void SpawnDamageText(long damage, Vector2 anchoredPosition)
        {
            if (damageTextPool.Count == 0)
                damageTextPool.Push(Instantiate(damageTextPrefab, damageTextPrefab.transform.parent));

            var damageText = damageTextPool.Pop();
            damageText.gameObject.SetActive(true);
            damageText.transform.SetAsLastSibling();
            damageText.text = damage.ToString();
            damageText.alpha = 1f;
            damageText.rectTransform.anchoredPosition = anchoredPosition;
            damageText.rectTransform.DOAnchorPosY(anchoredPosition.y + 50f, 0.8f)
                .SetEase(Ease.Linear);
            damageText.DOFade(0f, 0.8f)
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

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class EffectSpawner : MonoBehaviour
    {
        public GameObject attackEffect;
        public GameObject hitEffect;
        public GameObject jumpEffect;
        public GameObject levelUpEffect;

        public Vector2 attackEffectOffset;
        public Vector2 jumpEffectOffset;

        Stack<GameObject> attackEffectPool = new Stack<GameObject>();
        Stack<GameObject> hitEffectPool = new Stack<GameObject>();
        Stack<GameObject> jumpEffectPool = new Stack<GameObject>();
        Stack<GameObject> levelUpEffectPool = new Stack<GameObject>();

        public void ShowHitEffect(Vector2 position, int count)
        {
            StartCoroutine(ShowHitEffectCo(position, count));
        }

        public void ShowAttackEffect(Vector2 position, bool isLeft)
        {
            var effect = attackEffectPool.Count > 0 ? attackEffectPool.Pop() : Instantiate(attackEffect);
            var effectOffset = new Vector2(isLeft ? -attackEffectOffset.x : attackEffectOffset.x, attackEffectOffset.y);
            effect.transform.position = position + effectOffset;
            effect.transform.rotation = Quaternion.Euler(effect.transform.rotation.x, isLeft ? 180f : 0f, effect.transform.rotation.z);
            effect.gameObject.SetActive(true);

            var renderer = effect.GetComponent<SpriteRenderer>();
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
            renderer.DOFade(0f, 0.5f)
                .OnComplete(() =>
                {
                    effect.gameObject.SetActive(false);
                    attackEffectPool.Push(effect);
                });
        }

        public void ShowJumpEffect(Vector2 position, bool isLeft)
        {
            var effect = jumpEffectPool.Count > 0 ? jumpEffectPool.Pop() : Instantiate(jumpEffect);
            var effectOffset = new Vector2(isLeft ? -jumpEffectOffset.x : jumpEffectOffset.x, jumpEffectOffset.y);

            effect.transform.position = position + effectOffset;
            effect.transform.rotation = Quaternion.Euler(effect.transform.rotation.x, isLeft ? 180f : 0f, effect.transform.rotation.z);
            effect.gameObject.SetActive(true);

            var renderer = effect.GetComponent<SpriteRenderer>();
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
            renderer.DOFade(0f, 1f)
                .OnComplete(() =>
                {
                    effect.gameObject.SetActive(false);
                    jumpEffectPool.Push(effect);
                });
        }

        public void ShowLevelUpEffect(Vector2 position)
        {
            var effect = levelUpEffectPool.Count > 0 ? levelUpEffectPool.Pop() : Instantiate(levelUpEffect);

            effect.transform.position = position;
            effect.gameObject.SetActive(true);
            effect.transform.localScale = Vector3.one * 4f;
            effect.transform.DOScale(20f, 1f);

            var renderer = effect.GetComponent<SpriteRenderer>();
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
            renderer.DOFade(0f, 1f)
                .OnComplete(() =>
                {
                    effect.gameObject.SetActive(false);
                    levelUpEffectPool.Push(effect);
                });
        }

        IEnumerator ShowHitEffectCo(Vector2 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randPos = position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f));
                ShowHitEffect(randPos);
                yield return null;
            }
        }

        void ShowHitEffect(Vector2 position)
        {
            var effect = hitEffectPool.Count > 0 ? hitEffectPool.Pop() : Instantiate(hitEffect);
            effect.transform.position = position;
            effect.gameObject.SetActive(true);
            effect.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f);

            var renderer = effect.GetComponent<SpriteRenderer>();
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
            renderer.DOFade(0f, 0.5f)
                .OnComplete(() =>
                {
                    effect.gameObject.SetActive(false);
                    hitEffectPool.Push(effect);
                });
        }
    }
}

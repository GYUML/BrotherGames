using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEventListner : MonoBehaviour
{
    public int Id;

    public float fadeDuration;
    public List<SpriteRenderer> renderers;

    BoxCollider2D boxCollider;

    Coroutine fadeCoroutine;
    List<float> alphaList = new List<float>();

    private void Awake()
    {
        foreach (var r in renderers)
            alphaList.Add(r.color.a);
    }

    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;
    }

    public void DoDrop()
    {
        boxCollider.enabled = false;
        fadeCoroutine = StartCoroutine(FadeCo(1f, 0f));
    }

    public void UndoDrop()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        boxCollider.enabled = true;
        MultipleAllAlpha(1f);
    }

    IEnumerator FadeCo(float from, float to)
    {
        var elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            MultipleAllAlpha(newAlpha);
            yield return null;
        }

        MultipleAllAlpha(to);
    }

    void MultipleAllAlpha(float multi)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            var newAlpha = alphaList[i] * multi;
            var color = renderers[i].color;
            color.a = newAlpha;
            renderers[i].color = color;
        }
    }
}

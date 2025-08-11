using System.Collections;
using UnityEngine;

public class TileEventListner : MonoBehaviour
{
    public int Id;

    public float fadeDuration;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    Coroutine fadeCoroutine;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        var color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    IEnumerator FadeCo(float from, float to)
    {
        var color = spriteRenderer.color;
        var elapsed = to;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, to);
    }
}

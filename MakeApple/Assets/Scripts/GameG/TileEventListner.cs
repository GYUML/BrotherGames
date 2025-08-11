using System.Collections;
using UnityEngine;

public class TileEventListner : MonoBehaviour
{
    public int Id;

    public float fadeDuration;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;
    }

    public void DoDrop()
    {
        boxCollider.enabled = false;
        StartCoroutine(FadeCo());
    }

    IEnumerator FadeCo()
    {
        var color = spriteRenderer.color;
        var elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(color.a, 0f, elapsed / fadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
    }
}

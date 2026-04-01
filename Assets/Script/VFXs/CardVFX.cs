using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVFX : MonoBehaviour
{
    [Header("Glow")]
    public Image glowImage;

    [Header("Shimmer")]
    public Image shimmerImage;

    private Coroutine glowCoroutine;
    private Coroutine shimmerCoroutine;
    private bool isPlayable = false;

    void Start()
    {
        if (glowImage != null)
        {
            Color c = glowImage.color;
            c.a = 0f;
            glowImage.color = c;
            glowImage.raycastTarget = false;
        }

        if (shimmerImage != null)
        {
            Color c = shimmerImage.color;
            c.a = 0f;
            shimmerImage.color = c;
            shimmerImage.raycastTarget = false;
        }
    }

    // Call this to show glow on playable cards
    public void SetPlayable(bool playable)
    {
        isPlayable = playable;

        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);

        if (playable)
            glowCoroutine = StartCoroutine(PulseGlow());
        else
            glowCoroutine = StartCoroutine(FadeGlow(0f));
    }

    IEnumerator PulseGlow()
    {
        if (glowImage == null) yield break;

        while (isPlayable)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < 0.6f)
            {
                elapsed += Time.deltaTime;
                Color c = glowImage.color;
                c.a = Mathf.Lerp(0f, 0.8f, elapsed / 0.6f);
                glowImage.color = c;
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < 0.6f)
            {
                elapsed += Time.deltaTime;
                Color c = glowImage.color;
                c.a = Mathf.Lerp(0.8f, 0.2f, elapsed / 0.6f);
                glowImage.color = c;
                yield return null;
            }
        }
    }

    IEnumerator FadeGlow(float targetAlpha)
    {
        if (glowImage == null) yield break;

        float startAlpha = glowImage.color.a;
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            Color c = glowImage.color;
            c.a = Mathf.Lerp(startAlpha,
                targetAlpha, elapsed / 0.3f);
            glowImage.color = c;
            yield return null;
        }
    }

    // Call this on card play
    public void PlayShimmer()
    {
        if (shimmerCoroutine != null)
            StopCoroutine(shimmerCoroutine);
        shimmerCoroutine = StartCoroutine(DoShimmer());
    }

    IEnumerator DoShimmer()
    {
        if (shimmerImage == null) yield break;

        RectTransform rt =
            shimmerImage.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = new Vector2(-100f, 0f);

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color c = shimmerImage.color;
            c.a = t < 0.5f ?
                Mathf.Lerp(0f, 0.6f, t * 2f) :
                Mathf.Lerp(0.6f, 0f, (t - 0.5f) * 2f);
            shimmerImage.color = c;

            if (rt != null)
                rt.anchoredPosition = new Vector2(
                    Mathf.Lerp(-100f, 100f, t), 0f);

            yield return null;
        }

        Color final = shimmerImage.color;
        final.a = 0f;
        shimmerImage.color = final;
    }
}
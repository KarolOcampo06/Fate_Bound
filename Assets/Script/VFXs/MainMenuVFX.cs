using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuVFX : MonoBehaviour
{
    [Header("Background Particles")]
    public RectTransform particleArea;
    public int particleCount = 25;

    [Header("Logo Shimmer")]
    public Image logoImage;
    public Image logoShimmerOverlay;

    [Header("Transition")]
    public Image transitionPanel;

    private List<RectTransform> particles =
        new List<RectTransform>();

    void Start()
    {
        SpawnBackgroundParticles();
        StartCoroutine(LogoShimmerLoop());

        if (transitionPanel != null)
        {
            Color c = transitionPanel.color;
            c.a = 0f;
            transitionPanel.color = c;
            transitionPanel.raycastTarget = false;
        }

        // Fade in from black on scene load
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (transitionPanel == null) yield break;

        transitionPanel.raycastTarget = true;
        Color c = transitionPanel.color;
        c.a = 1f;
        transitionPanel.color = c;

        float elapsed = 0f;
        while (elapsed < 0.8f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / 0.8f);
            transitionPanel.color = c;
            yield return null;
        }

        c.a = 0f;
        transitionPanel.color = c;
        transitionPanel.raycastTarget = false;
    }

    public IEnumerator FadeOut()
    {
        if (transitionPanel == null) yield break;

        transitionPanel.raycastTarget = true;
        Color c = transitionPanel.color;
        c.a = 0f;
        transitionPanel.color = c;

        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / 0.6f);
            transitionPanel.color = c;
            yield return null;
        }

        c.a = 1f;
        transitionPanel.color = c;
    }

    // ── Logo Shimmer ─────────────────────────────────────

    IEnumerator LogoShimmerLoop()
    {
        if (logoImage == null) yield break;

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            yield return StartCoroutine(PulseLogo());
        }
    }

    IEnumerator PulseLogo()
    {
        if (logoImage == null) yield break;

        RectTransform rt = logoImage.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;
        Vector3 bigScale = new Vector3(1.04f, 1.04f, 1f);

        // Scale up
        float elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1.2f;
            rt.localScale = Vector3.Lerp(
                originalScale, bigScale, EaseInOut(t));
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1.2f;
            rt.localScale = Vector3.Lerp(
                bigScale, originalScale, EaseInOut(t));
            yield return null;
        }

        rt.localScale = originalScale;
        yield return new WaitForSeconds(0.5f);
    }

    float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t :
            1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    // ── Background Particles ─────────────────────────────

    void SpawnBackgroundParticles()
    {
        if (particleArea == null) return;

        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("MenuParticle_" + i);
            p.transform.SetParent(particleArea, false);

            Image img = p.AddComponent<Image>();
            img.raycastTarget = false;

            Color c = Random.value > 0.5f ?
                new Color(1f, 0.85f, 0.2f, 0f) :
                new Color(0.6f, 0.2f, 1f, 0f);
            img.color = c;

            RectTransform rt = p.GetComponent<RectTransform>();
            float size = Random.Range(4f, 14f);
            rt.sizeDelta = new Vector2(size, size);
            particles.Add(rt);

            StartCoroutine(FloatMenuParticle(rt, img));
        }
    }

    IEnumerator FloatMenuParticle(RectTransform rt, Image img)
    {
        yield return new WaitForSeconds(Random.Range(0f, 4f));

        while (true)
        {
            if (rt == null) yield break;

            float floatDuration = Random.Range(5f, 10f);
            float fadeDuration = 1.5f;

            Vector2 startPos = new Vector2(
                Random.Range(-600f, 600f), -450f);
            Vector2 endPos = new Vector2(
                startPos.x + Random.Range(-80f, 80f), 500f);

            rt.anchoredPosition = startPos;

            float elapsed = 0f;

            // Fade in and float
            while (elapsed < fadeDuration)
            {
                if (rt == null) yield break;
                elapsed += Time.deltaTime;
                Color c = img.color;
                c.a = Mathf.Lerp(0f, 0.7f,
                    elapsed / fadeDuration);
                img.color = c;
                rt.anchoredPosition = Vector2.Lerp(
                    startPos, endPos,
                    elapsed / floatDuration);
                yield return null;
            }

            while (elapsed < floatDuration - fadeDuration)
            {
                if (rt == null) yield break;
                elapsed += Time.deltaTime;
                rt.anchoredPosition = Vector2.Lerp(
                    startPos, endPos,
                    elapsed / floatDuration);
                yield return null;
            }

            // Fade out
            float fadeStart = elapsed;
            while (elapsed < floatDuration)
            {
                if (rt == null) yield break;
                elapsed += Time.deltaTime;
                float t = (elapsed - fadeStart) / fadeDuration;
                Color c = img.color;
                c.a = Mathf.Lerp(0.7f, 0f, t);
                img.color = c;
                rt.anchoredPosition = Vector2.Lerp(
                    startPos, endPos,
                    elapsed / floatDuration);
                yield return null;
            }

            yield return new WaitForSeconds(
                Random.Range(0f, 1.5f));
        }
    }
}
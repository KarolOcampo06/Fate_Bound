using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Screen Flash")]
    public Image screenFlashImage;

    [Header("Card Burst Particles")]
    public int burstParticleCount = 12;

    [Header("Background Particles")]
    public RectTransform backgroundParticleArea;
    public int backgroundParticleCount = 20;
    public Sprite particleSprite;

    private List<RectTransform> bgParticles =
        new List<RectTransform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (screenFlashImage != null)
        {
            screenFlashImage.color = new Color(1, 1, 1, 0);
            screenFlashImage.raycastTarget = false;
        }
        SpawnBackgroundParticles();
    }

    // ── Screen Flash ────────────────────────────────────

    public void FlashScreen(Color color, float duration = 0.3f)
    {
        if (screenFlashImage == null) return;
        StartCoroutine(DoScreenFlash(color, duration));
    }

    IEnumerator DoScreenFlash(Color color, float duration)
    {
        color.a = 0.45f;
        screenFlashImage.color = color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(0.45f, 0f, t * t);
            screenFlashImage.color = color;
            yield return null;
        }
        color.a = 0f;
        screenFlashImage.color = color;
    }

    // ── Card Burst Particles ─────────────────────────────

    public void SpawnCardBurst(Vector3 worldPos, Color color)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        StartCoroutine(DoBurst(worldPos, color, canvas));
    }

    IEnumerator DoBurst(Vector3 worldPos,
        Color color, Canvas canvas)
    {
        List<RectTransform> particles = new List<RectTransform>();

        for (int i = 0; i < burstParticleCount; i++)
        {
            GameObject p = new GameObject("Burst_" + i);
            p.transform.SetParent(canvas.transform, false);

            Image img = p.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            RectTransform rt = p.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(
                Random.Range(6f, 14f),
                Random.Range(6f, 14f));
            rt.position = worldPos;
            particles.Add(rt);
        }

        float duration = 0.5f;
        float elapsed = 0f;
        Vector2[] directions = new Vector2[burstParticleCount];
        for (int i = 0; i < burstParticleCount; i++)
        {
            float angle = (360f / burstParticleCount) * i +
                Random.Range(-15f, 15f);
            float rad = angle * Mathf.Deg2Rad;
            float speed = Random.Range(80f, 180f);
            directions[i] = new Vector2(
                Mathf.Cos(rad), Mathf.Sin(rad)) * speed;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i] == null) continue;
                particles[i].anchoredPosition +=
                    directions[i] * Time.deltaTime;

                Image img = particles[i]
                    .GetComponent<Image>();
                if (img != null)
                {
                    Color c = img.color;
                    c.a = Mathf.Lerp(1f, 0f, t * t);
                    img.color = c;
                }
            }
            yield return null;
        }

        foreach (var p in particles)
            if (p != null) Destroy(p.gameObject);
    }

    // ── Background Floating Particles ────────────────────

    void SpawnBackgroundParticles()
    {
        if (backgroundParticleArea == null) return;

        for (int i = 0; i < backgroundParticleCount; i++)
        {
            GameObject p = new GameObject("BGParticle_" + i);
            p.transform.SetParent(
                backgroundParticleArea, false);

            Image img = p.AddComponent<Image>();
            img.raycastTarget = false;

            Color c = Random.value > 0.5f ?
                new Color(1f, 0.85f, 0.2f, 0f) :
                new Color(0.6f, 0.2f, 1f, 0f);
            img.color = c;

            if (particleSprite != null)
                img.sprite = particleSprite;

            RectTransform rt = p.GetComponent<RectTransform>();
            float size = Random.Range(4f, 12f);
            rt.sizeDelta = new Vector2(size, size);

            RectTransform area = backgroundParticleArea;
            rt.anchoredPosition = new Vector2(
                Random.Range(-area.rect.width / 2f,
                    area.rect.width / 2f),
                Random.Range(-area.rect.height / 2f,
                    area.rect.height / 2f));

            bgParticles.Add(rt);
            StartCoroutine(FloatParticle(rt, img));
        }
    }

    IEnumerator FloatParticle(RectTransform rt, Image img)
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f));

        RectTransform area = backgroundParticleArea;

        while (true)
        {
            if (rt == null) yield break;

            float floatDuration = Random.Range(4f, 8f);
            float fadeDuration = 1.5f;

            Vector2 startPos = new Vector2(
                Random.Range(-area.rect.width / 2f,
                    area.rect.width / 2f),
                -area.rect.height / 2f);

            Vector2 endPos = new Vector2(
                startPos.x + Random.Range(-60f, 60f),
                area.rect.height / 2f + 20f);

            rt.anchoredPosition = startPos;

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                if (rt == null) yield break;
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                Color c = img.color;
                c.a = Mathf.Lerp(0f, 0.6f, t);
                img.color = c;
                rt.anchoredPosition = Vector2.Lerp(
                    startPos, endPos,
                    elapsed / floatDuration);
                yield return null;
            }

            // Float up
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
                c.a = Mathf.Lerp(0.6f, 0f, t);
                img.color = c;
                rt.anchoredPosition = Vector2.Lerp(
                    startPos, endPos,
                    elapsed / floatDuration);
                yield return null;
            }

            yield return new WaitForSeconds(
                Random.Range(0.5f, 2f));
        }
    }
}
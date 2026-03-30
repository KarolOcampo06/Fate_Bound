using UnityEngine;
using System.Collections;
using System;

public class CardAnimator : MonoBehaviour
{
    [Header("Durations")]
    public float dealDuration = 0.5f;
    public float playDuration = 0.4f;
    public float drawDuration = 0.35f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // DEAL — card falls from above into position
    public IEnumerator DealAnimation(Vector2 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (this == null || rectTransform == null) yield break;

        Vector2 startPos = new Vector2(targetPos.x, targetPos.y + 800f);
        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = Vector3.zero;
        rectTransform.localRotation = Quaternion.Euler(0, 0,
            UnityEngine.Random.Range(-20f, 20f));

        float elapsed = 0f;
        while (elapsed < dealDuration)
        {
            if (this == null || rectTransform == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dealDuration);

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, EaseOutBack(t));
            rectTransform.localScale =
                Vector3.Lerp(Vector3.zero, Vector3.one, EaseOutBack(t));
            rectTransform.localRotation = Quaternion.Lerp(
                rectTransform.localRotation,
                Quaternion.identity, t * t);

            yield return null;
        }

        if (this == null || rectTransform == null) yield break;

        rectTransform.anchoredPosition = targetPos;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    // PLAY — card scales up then flies to discard pile
    public IEnumerator PlayAnimation(RectTransform discardRect,
        Action onComplete)
    {
        if (this == null || rectTransform == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        Vector3 startWorldPos = rectTransform.position;

        rectTransform.SetParent(canvas.transform, true);
        rectTransform.position = startWorldPos;

        Vector3 targetWorldPos = discardRect.position;
        float randomSpin = UnityEngine.Random.Range(-30f, 30f);

        // Punch scale up
        float scaleElapsed = 0f;
        while (scaleElapsed < 0.12f)
        {
            if (this == null || rectTransform == null)
            {
                onComplete?.Invoke();
                yield break;
            }
            scaleElapsed += Time.deltaTime;
            float t = scaleElapsed / 0.12f;
            rectTransform.localScale =
                Vector3.Lerp(Vector3.one, Vector3.one * 1.3f, t);
            yield return null;
        }

        // Fly to discard pile
        float elapsed = 0f;
        Vector3 punchScale = rectTransform.localScale;
        while (elapsed < playDuration)
        {
            if (this == null || rectTransform == null)
            {
                onComplete?.Invoke();
                yield break;
            }
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / playDuration);
            float eased = EaseInOutCubic(t);

            rectTransform.position =
                Vector3.Lerp(startWorldPos, targetWorldPos, eased);
            rectTransform.localRotation =
                Quaternion.Euler(0, 0, Mathf.Lerp(0, randomSpin, eased));
            rectTransform.localScale =
                Vector3.Lerp(punchScale, Vector3.one, eased);

            yield return null;
        }

        onComplete?.Invoke();
    }

    // DRAW — card slides in from the side into hand
    public IEnumerator DrawAnimation(Vector2 targetPos)
    {
        if (this == null || rectTransform == null) yield break;

        Vector2 startPos = new Vector2(targetPos.x + 400f,
            targetPos.y - 80f);
        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = Vector3.one * 0.5f;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -20f);

        float elapsed = 0f;
        while (elapsed < drawDuration)
        {
            if (this == null || rectTransform == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / drawDuration);

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, EaseOutBack(t));
            rectTransform.localScale =
                Vector3.Lerp(Vector3.one * 0.5f, Vector3.one,
                    EaseOutCubic(t));
            rectTransform.localRotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, -20f),
                Quaternion.identity, EaseOutCubic(t));

            yield return null;
        }

        if (this == null || rectTransform == null) yield break;

        rectTransform.anchoredPosition = targetPos;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    // Easing functions
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) +
            c1 * Mathf.Pow(t - 1f, 2f);
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t :
            1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
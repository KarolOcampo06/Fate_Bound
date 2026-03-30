using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float liftAmount = 40f;
    public float scaleAmount = 1.12f;
    public float animDuration = 0.2f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Coroutine hoverCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(false));
    }

    IEnumerator AnimateHover(bool hovering)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector3 startScale = rectTransform.localScale;

        Vector2 targetPos = hovering ?
            new Vector2(originalPosition.x,
                originalPosition.y + liftAmount) :
            originalPosition;

        Vector3 targetScale = hovering ?
            Vector3.one * scaleAmount : Vector3.one;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float eased = EaseOutCubic(t);

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, eased);
            rectTransform.localScale =
                Vector3.Lerp(startScale, targetScale, eased);

            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        rectTransform.localScale = targetScale;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
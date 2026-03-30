using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float liftAmount = 30f;
    public float animDuration = 0.15f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool positionSet = false;
    private bool isHovered = false;
    private Coroutine hoverCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateOriginalPosition()
    {
        StartCoroutine(StorePositionNextFrame());
    }

    IEnumerator StorePositionNextFrame()
    {
        yield return new WaitForEndOfFrame();
        if (rectTransform != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            positionSet = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!positionSet) return;
        isHovered = true;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateLift(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!positionSet) return;
        isHovered = false;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateLift(false));
    }

    IEnumerator AnimateLift(bool liftUp)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = liftUp ?
            new Vector2(originalPosition.x,
                originalPosition.y + liftAmount) :
            originalPosition;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            if (rectTransform == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, EaseOutCubic(t));
            yield return null;
        }

        if (rectTransform != null)
            rectTransform.anchoredPosition = targetPos;
    }

    public void ResetPosition()
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        if (rectTransform != null && positionSet)
            rectTransform.anchoredPosition = originalPosition;
        isHovered = false;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
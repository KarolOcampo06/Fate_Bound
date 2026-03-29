using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float hoverHeight = 30f;
    public float hoverSpeed = 10f;
    public float hoverScale = 1.1f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Vector2 targetPosition;
    private Vector3 targetScale;
    private bool initialized = false;

    void Update()
    {
        if (!initialized) return;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * hoverSpeed
        );
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * hoverSpeed
        );
    }

    // Initialize AFTER cards are positioned
    public void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        targetPosition = originalPosition;
        targetScale = originalScale;
        initialized = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized) Initialize();
        targetPosition = originalPosition + new Vector2(0, hoverHeight);
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!initialized) Initialize();
        targetPosition = originalPosition;
        targetScale = originalScale;
    }

    public void UpdateOriginalPosition()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        targetPosition = originalPosition;
        targetScale = originalScale;
        initialized = true;
    }
}
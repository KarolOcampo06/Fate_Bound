using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Debug = UnityEngine.Debug;

public class CardEffectAnimator : MonoBehaviour
{
    public static CardEffectAnimator Instance;

    [Header("Effect Panel")]
    public GameObject effectPanel;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI effectSubText;
    public Image effectBackground;

    [Header("Colors")]
    public Color blockColor = new Color(0.2f, 0.6f, 1f, 1f);
    public Color reverseColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color drawTwoColor = new Color(1f, 0.6f, 0.2f, 1f);
    public Color drawFourColor = new Color(1f, 0.2f, 0.2f, 1f);
    public Color rollDiceColor = new Color(0.8f, 0.2f, 0.8f, 1f);

    private bool isAnimating = false;

    void Awake()
    {
        Instance = this;
        if (effectPanel != null)
            effectPanel.SetActive(false);
    }

    public void ShowEffect(CardType type)
    {
        // Only show for special cards
        if (type == CardType.Number) return;

        // Don't overlap animations
        if (isAnimating)
            StopAllCoroutines();

        StartCoroutine(AnimateEffect(type));
    }

    IEnumerator AnimateEffect(CardType type)
    {
        isAnimating = true;
        if (effectPanel == null)
        {
            isAnimating = false;
            yield break;
        }

        // Reset state
        CanvasGroup canvasGroup = effectPanel
            .GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = effectPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        effectPanel.SetActive(true);

        // Set text and color based on card type
        switch (type)
        {
            case CardType.Block:
                if (effectText != null)
                    effectText.text = "BLOCKED!";
                if (effectSubText != null)
                    effectSubText.text = "Opponent loses their turn";
                if (effectBackground != null)
                    effectBackground.color = blockColor;
                break;

            case CardType.Reverse:
                if (effectText != null)
                    effectText.text = "REVERSED!";
                if (effectSubText != null)
                    effectSubText.text = "Direction changed!";
                if (effectBackground != null)
                    effectBackground.color = reverseColor;
                break;

            case CardType.DrawTwo:
                if (effectText != null)
                    effectText.text = "DRAW 2!";
                if (effectSubText != null)
                    effectSubText.text = "Opponent draws 2 cards!";
                if (effectBackground != null)
                    effectBackground.color = drawTwoColor;
                break;

            case CardType.DrawFour:
                if (effectText != null)
                    effectText.text = "DRAW 4!";
                if (effectSubText != null)
                    effectSubText.text = "Opponent draws 4 cards!";
                if (effectBackground != null)
                    effectBackground.color = drawFourColor;
                break;

            case CardType.RollDice:
                if (effectText != null)
                    effectText.text = "ROLL DICE!";
                if (effectSubText != null)
                    effectSubText.text = "Rolling the dice...";
                if (effectBackground != null)
                    effectBackground.color = rollDiceColor;
                break;
        }

        // Scale up animation
        RectTransform rect = effectPanel
            .GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                float scale = Mathf.Lerp(0f, 1.1f, elapsed / 0.3f);
                rect.localScale = new Vector3(scale, scale, 1f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < 0.15f)
            {
                float scale = Mathf.Lerp(1.1f, 1f, elapsed / 0.15f);
                rect.localScale = new Vector3(scale, scale, 1f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rect.localScale = Vector3.one;
        }

        // Keep visible
        yield return new WaitForSeconds(1.5f);

        // Fade out
        float fadeElapsed = 0f;
        while (fadeElapsed < 0.5f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                1f, 0f, fadeElapsed / 0.5f);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        effectPanel.SetActive(false);
        isAnimating = false;
    }
}
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

    // isPlayerCard = true  → PLAYER played the card
    // isPlayerCard = false → OPPONENT played the card
    public void ShowEffect(CardType type,
        bool isPlayerCard = true)
    {
        if (type == CardType.Number) return;
        if (isAnimating) StopAllCoroutines();
        StartCoroutine(AnimateEffect(type, isPlayerCard));
    }

    IEnumerator AnimateEffect(CardType type, bool isPlayerCard)
    {
        isAnimating = true;
        if (effectPanel == null)
        {
            isAnimating = false;
            yield break;
        }

        CanvasGroup canvasGroup =
            effectPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup =
                effectPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        effectPanel.SetActive(true);
        AudioManager.Instance?.PlaySpecialCardSFX(type);

        // Set title and color
        switch (type)
        {
            case CardType.Block:
                if (effectText != null)
                    effectText.text = "BLOCKED!";
                if (effectBackground != null)
                    effectBackground.color = blockColor;
                // Player played → opponent loses turn
                // Opponent played → player loses turn
                if (effectSubText != null)
                    effectSubText.text = isPlayerCard ?
                        "Opponent loses their turn!" :
                        "You lose your turn!";
                break;

            case CardType.Reverse:
                if (effectText != null)
                    effectText.text = "REVERSED!";
                if (effectBackground != null)
                    effectBackground.color = reverseColor;
                if (effectSubText != null)
                    effectSubText.text = isPlayerCard ?
                        "Opponent loses their turn!" :
                        "You lose your turn!";
                break;

            case CardType.DrawTwo:
                if (effectText != null)
                    effectText.text = "DRAW 2!";
                if (effectBackground != null)
                    effectBackground.color = drawTwoColor;
                // Player played → opponent draws
                // Opponent played → player draws
                if (effectSubText != null)
                    effectSubText.text = isPlayerCard ?
                        "Opponent draws 2 cards!" :
                        "You draw 2 cards!";
                break;

            case CardType.DrawFour:
                if (effectText != null)
                    effectText.text = "DRAW 4!";
                if (effectBackground != null)
                    effectBackground.color = drawFourColor;
                if (effectSubText != null)
                    effectSubText.text = isPlayerCard ?
                        "Opponent draws 4 cards!" :
                        "You draw 4 cards!";
                break;

            case CardType.RollDice:
                if (effectText != null)
                    effectText.text = "ROLL DICE!";
                if (effectBackground != null)
                    effectBackground.color = rollDiceColor;
                if (effectSubText != null)
                    effectSubText.text = isPlayerCard ?
                        "Opponent draws from the dice!" :
                        "You draw from the dice!";
                break;
        }

        // Scale up animation
        RectTransform rect =
            effectPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                float scale =
                    Mathf.Lerp(0f, 1.1f, elapsed / 0.3f);
                rect.localScale =
                    new Vector3(scale, scale, 1f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < 0.15f)
            {
                float scale =
                    Mathf.Lerp(1.1f, 1f, elapsed / 0.15f);
                rect.localScale =
                    new Vector3(scale, scale, 1f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rect.localScale = Vector3.one;
        }

        yield return new WaitForSeconds(1.5f);

        // Fade out
        float fadeElapsed = 0f;
        while (fadeElapsed < 0.5f)
        {
            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, fadeElapsed / 0.5f);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        effectPanel.SetActive(false);
        isAnimating = false;
    }
}
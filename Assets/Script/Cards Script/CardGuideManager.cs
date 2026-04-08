using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardGuideManager : MonoBehaviour
{
    public static CardGuideManager Instance;

    [Header("Guide Panel")]
    public GameObject guidePanel;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI canPlayText;
    public TextMeshProUGUI reasonText;

    [Header("Illegal Move Panel")]
    public GameObject illegalPanel;
    public TextMeshProUGUI illegalText;

    [Header("Turn Start Guide")]
    public GameObject turnStartPanel;
    public TextMeshProUGUI turnStartText;

    private CanvasGroup guideCanvasGroup;
    private CanvasGroup illegalCanvasGroup;
    private CanvasGroup turnStartCanvasGroup;
    private Coroutine hideCoroutine;
    private Coroutine illegalCoroutine;
    private Coroutine turnStartCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupCanvasGroups();
        HideAll();
    }

    void SetupCanvasGroups()
    {
        if (guidePanel != null)
        {
            guideCanvasGroup =
                guidePanel.GetComponent<CanvasGroup>();
            if (guideCanvasGroup == null)
                guideCanvasGroup =
                    guidePanel.AddComponent<CanvasGroup>();
            guideCanvasGroup.alpha = 0f;
            // CRITICAL — prevent panel intercepting mouse events
            guideCanvasGroup.blocksRaycasts = false;
            guideCanvasGroup.interactable = false;
            guidePanel.SetActive(false);
        }

        if (illegalPanel != null)
        {
            illegalCanvasGroup =
                illegalPanel.GetComponent<CanvasGroup>();
            if (illegalCanvasGroup == null)
                illegalCanvasGroup =
                    illegalPanel.AddComponent<CanvasGroup>();
            illegalCanvasGroup.alpha = 0f;
            illegalCanvasGroup.blocksRaycasts = false;
            illegalCanvasGroup.interactable = false;
            illegalPanel.SetActive(false);
        }

        if (turnStartPanel != null)
        {
            turnStartCanvasGroup =
                turnStartPanel.GetComponent<CanvasGroup>();
            if (turnStartCanvasGroup == null)
                turnStartCanvasGroup =
                    turnStartPanel.AddComponent<CanvasGroup>();
            turnStartCanvasGroup.alpha = 0f;
            turnStartCanvasGroup.blocksRaycasts = false;
            turnStartCanvasGroup.interactable = false;
            turnStartPanel.SetActive(false);
        }
    }

    void HideAll()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
        if (illegalPanel != null)
            illegalPanel.SetActive(false);
        if (turnStartPanel != null)
            turnStartPanel.SetActive(false);
    }

    // ── Show Card Guide On Hover ─────────────────────

    public void ShowCardGuide(Card card)
    {
        if (guidePanel == null) return;
        if (!GameManager.Instance.isPlayerTurn) return;

        Card topCard =
            GameManager.Instance.topCardOnDiscardPile;

        // Set card name
        if (cardNameText != null)
            cardNameText.text = GetCardDisplayName(card);

        // Set what can be played
        if (canPlayText != null)
            canPlayText.text = GetCanPlayText(card, topCard);

        // Set reason
        if (reasonText != null)
            reasonText.text = GetReasonText(card, topCard);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        StartCoroutine(FadePanel(
            guidePanel, guideCanvasGroup, true));
    }

    public void HideCardGuide()
    {
        if (guidePanel == null) return;
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(
            FadePanel(guidePanel, guideCanvasGroup, false));
    }

    // ── Show Illegal Move ────────────────────────────

    public void ShowIllegalMove(Card card)
    {
        if (illegalPanel == null) return;

        Card topCard =
            GameManager.Instance.topCardOnDiscardPile;

        if (illegalText != null)
            illegalText.text = GetIllegalMoveText(
                card, topCard);

        if (illegalCoroutine != null)
            StopCoroutine(illegalCoroutine);

        illegalCoroutine =
            StartCoroutine(ShowThenHide(
                illegalPanel, illegalCanvasGroup, 2.5f));
    }

    // ── Show Turn Start Guide ────────────────────────

    public void ShowTurnStartGuide()
    {
        if (turnStartPanel == null) return;

        Card topCard =
            GameManager.Instance.topCardOnDiscardPile;

        if (turnStartText != null)
            turnStartText.text = GetTurnStartText(topCard);

        if (turnStartCoroutine != null)
            StopCoroutine(turnStartCoroutine);

        turnStartCoroutine =
            StartCoroutine(ShowThenHide(
                turnStartPanel,
                turnStartCanvasGroup, 3f));
    }

    // ── Text Generators ──────────────────────────────

    string GetCardDisplayName(Card card)
    {
        string colorName = GetColorName(card.color);

        if (card.type == CardType.Number)
            return colorName + " " + card.number;

        return colorName + " " + GetTypeName(card.type);
    }

    string GetCanPlayText(Card card, Card topCard)
    {
        if (topCard == null)
            return "✅ You can play any card first!";

        bool legal =
            GameManager.Instance.IsMoveLegal(card);

        if (legal)
        {
            if (card.color == topCard.color)
                return "✅ Same color as " +
                    GetCardDisplayName(topCard);

            if (card.type == CardType.Number &&
                topCard.type == CardType.Number &&
                card.number == topCard.number)
                return "✅ Same number as " +
                    GetCardDisplayName(topCard);

            if (card.type != CardType.Number &&
                card.type == topCard.type)
                return "✅ Same type as " +
                    GetCardDisplayName(topCard);

            return "✅ This card can be played!";
        }
        else
        {
            return "❌ Cannot play on " +
                GetCardDisplayName(topCard);
        }
    }

    string GetReasonText(Card card, Card topCard)
    {
        if (topCard == null)
            return "First card — anything goes!";

        bool legal =
            GameManager.Instance.IsMoveLegal(card);

        if (legal)
        {
            // Explain why it's legal
            if (card.type == CardType.Number &&
                topCard.type == CardType.Number &&
                card.number == topCard.number &&
                card.color != topCard.color)
                return "💡 Same number works even " +
                    "with different colors!";

            if (card.type != CardType.Number &&
                card.type == topCard.type &&
                card.color != topCard.color)
                return "💡 Same special type works " +
                    "even with different colors!";

            return "💡 " + GetSpecialCardEffect(card.type);
        }
        else
        {
            // Explain what to match
            if (topCard.type == CardType.Number)
                return "💡 Match the color (" +
                    GetColorName(topCard.color) +
                    ") or number (" +
                    topCard.number + ")";

            return "💡 Match the color (" +
                GetColorName(topCard.color) +
                ") or same special type";
        }
    }

    string GetIllegalMoveText(Card card, Card topCard)
    {
        if (topCard == null)
            return "❌ Something went wrong!";

        string need = topCard.type == CardType.Number ?
            "Match color (" +
                GetColorName(topCard.color) +
                ") or number (" + topCard.number + ")" :
            "Match color (" +
                GetColorName(topCard.color) +
                ") or same special type";

        return "❌ Can't play " +
            GetCardDisplayName(card) +
            " here!\n" + need;
    }

    string GetTurnStartText(Card topCard)
    {
        if (topCard == null)
            return "🎮 Your turn! Play any card to start!";

        if (topCard.type == CardType.Number)
            return "🎮 Your turn! Match color (" +
                GetColorName(topCard.color) +
                ") or number (" +
                topCard.number + ") — " +
                "or draw a card!";

        return "🎮 Your turn! Match color (" +
            GetColorName(topCard.color) +
            ") or play same special (" +
            GetTypeName(topCard.type) +
            ") — or draw a card!";
    }

    // ── Helper Methods ───────────────────────────────

    string GetColorName(CardColor color)
    {
        switch (color)
        {
            case CardColor.Red: return "Red/Chaos";
            case CardColor.Gold: return "Gold/Destiny";
            case CardColor.Blue: return "Blue/Fortune";
            case CardColor.Purple: return "Purple/Shadow";
            default: return color.ToString();
        }
    }

    string GetTypeName(CardType type)
    {
        switch (type)
        {
            case CardType.Block: return "Block";
            case CardType.Reverse: return "Reverse";
            case CardType.DrawTwo: return "Draw Two";
            case CardType.DrawFour: return "Draw Four";
            case CardType.RollDice: return "Roll Dice";
            default: return type.ToString();
        }
    }

    string GetSpecialCardEffect(CardType type)
    {
        switch (type)
        {
            case CardType.Block:
                return "Opponent loses their turn!";
            case CardType.Reverse:
                return "Opponent loses their turn!";
            case CardType.DrawTwo:
                return "Opponent draws 2 cards!";
            case CardType.DrawFour:
                return "Opponent draws 4 cards!";
            case CardType.RollDice:
                return "Opponent draws 1-6 cards!";
            default: return "";
        }
    }

    // ── Animations ───────────────────────────────────

    IEnumerator FadePanel(GameObject panel,
        CanvasGroup cg, bool show)
    {
        if (panel == null || cg == null) yield break;

        if (show) panel.SetActive(true);

        float start = cg.alpha;
        float end = show ? 1f : 0f;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(
                start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;
        if (!show) panel.SetActive(false);
    }

    IEnumerator ShowThenHide(GameObject panel,
        CanvasGroup cg, float showDuration)
    {
        yield return StartCoroutine(
            FadePanel(panel, cg, true));

        yield return new WaitForSeconds(showDuration);

        yield return StartCoroutine(
            FadePanel(panel, cg, false));
    }
}
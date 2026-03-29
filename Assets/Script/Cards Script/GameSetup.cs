using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameSetup : MonoBehaviour
{
    public static GameSetup Instance;

    [Header("Card Prefab")]
    public GameObject cardPrefab;

    [Header("Areas")]
    public Transform playerArea;
    public Transform opponentArea;
    public Image discardPileImage;

    [Header("Card Back")]
    public Sprite cardBackSprite;

    [Header("Settings")]
    public int startingCards = 7;
    public float cardSpacing = 95f;

    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> opponentCards = new List<GameObject>();

    // Track actual card data for opponent AI
    public List<Card> opponentCardData = new List<Card>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SetupGame());
    }

    IEnumerator SetupGame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Debug.Log("=== GAME SETUP STARTING ===");

        ClearArea(playerArea);
        ClearArea(opponentArea);
        playerCards.Clear();
        opponentCards.Clear();
        opponentCardData.Clear();

        // Deal opponent cards
        for (int i = 0; i < startingCards; i++)
        {
            Card card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                // Get sprite for opponent card
                if (CardSpriteManager.Instance != null)
                {
                    card.cardSprite = CardSpriteManager.Instance
                        .GetSprite(card.color, card.type, card.number);
                }
                opponentCardData.Add(card);
                GameObject cardGO = SpawnCardBack(opponentArea);
                opponentCards.Add(cardGO);
            }
        }

        PositionCards(opponentCards, opponentArea);
        Debug.Log("Opponent cards dealt: " + opponentCards.Count);

        // Give cards to OpponentAI
        OpponentAI ai = GameManager.Instance.opponentAI;
        if (ai != null)
        {
            ai.SetCards(opponentCardData);
            Debug.Log("AI cards set: " + opponentCardData.Count);
        }

        // Deal player cards
        for (int i = 0; i < startingCards; i++)
        {
            Card card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                GameObject cardGO = SpawnPlayerCard(card, playerArea);
                playerCards.Add(cardGO);
            }
        }

        PositionCards(playerCards, playerArea);
        Debug.Log("Player cards dealt: " + playerCards.Count);

        // Setup discard pile
        Card firstCard = DeckManager.Instance.DrawCard();
        if (firstCard != null && discardPileImage != null)
        {
            if (CardSpriteManager.Instance != null)
            {
                firstCard.cardSprite = CardSpriteManager.Instance
                    .GetSprite(firstCard.color,
                        firstCard.type, firstCard.number);
            }
            if (firstCard.cardSprite != null)
                discardPileImage.sprite = firstCard.cardSprite;
            GameManager.Instance.topCardOnDiscardPile = firstCard;
            Debug.Log("Discard pile: " + firstCard.cardName);
        }

        Debug.Log("=== SETUP COMPLETE ===");
    }

    void PositionCards(List<GameObject> cards, Transform area)
    {
        int count = cards.Count;
        if (count == 0) return;

        float spacing = cardSpacing;

        RectTransform areaRect = area.GetComponent<RectTransform>();

        // Get actual width of the area
        float areaWidth = areaRect != null ?
            areaRect.rect.width : 900f;

        // Make sure cards never exceed area width
        float maxWidth = areaWidth - 100f;
        float totalWidth = (count - 1) * spacing;

        // Shrink spacing if needed
        if (totalWidth > maxWidth && count > 1)
        {
            spacing = maxWidth / (count - 1);
            totalWidth = maxWidth;
        }

        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            if (cards[i] == null) continue;

            RectTransform cardRect = cards[i]
                .GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.anchoredPosition =
                    new Vector2(startX + (i * spacing), 0);
                cardRect.localScale = Vector3.one;
            }

            CardHover hover = cards[i].GetComponent<CardHover>();
            if (hover != null) hover.UpdateOriginalPosition();
        }

        Debug.Log("Cards positioned: " + count +
            " Spacing: " + spacing +
            " Area width: " + areaWidth);
    }

    void ClearArea(Transform area)
    {
        foreach (Transform child in area)
            Destroy(child.gameObject);
    }

    GameObject SpawnCardBack(Transform area)
    {
        GameObject cardGO = Instantiate(cardPrefab, area);
        Image img = cardGO.GetComponent<Image>();
        if (img != null && cardBackSprite != null)
            img.sprite = cardBackSprite;

        CardClick click = cardGO.GetComponent<CardClick>();
        if (click != null) click.enabled = false;

        CardHover hover = cardGO.GetComponent<CardHover>();
        if (hover != null) hover.enabled = false;

        return cardGO;
    }

    GameObject SpawnPlayerCard(Card card, Transform area)
    {
        GameObject cardGO = Instantiate(cardPrefab, area);

        if (CardSpriteManager.Instance != null)
        {
            card.cardSprite = CardSpriteManager.Instance
                .GetSprite(card.color, card.type, card.number);
        }

        CardObject cardObj = cardGO.GetComponent<CardObject>();
        if (cardObj != null) cardObj.SetCard(card);

        return cardGO;
    }

    public void AddCardToPlayer()
    {
        Card card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            if (CardSpriteManager.Instance != null)
            {
                card.cardSprite = CardSpriteManager.Instance
                    .GetSprite(card.color, card.type, card.number);
            }
            GameObject cardGO = SpawnPlayerCard(card, playerArea);
            playerCards.Add(cardGO);
            PositionCards(playerCards, playerArea);
            GameManager.Instance.playerHandCount++;
            Debug.Log("Player drew! Total: " + playerCards.Count);
        }
    }

    public void RemoveCardFromPlayer(GameObject cardGO)
    {
        playerCards.Remove(cardGO);
        Destroy(cardGO);
        GameManager.Instance.playerHandCount--;
        StartCoroutine(RepositionAfterRemove());
        Debug.Log("Card played! Remaining: " + playerCards.Count);
    }

    IEnumerator RepositionAfterRemove()
    {
        yield return new WaitForEndOfFrame();
        playerCards.RemoveAll(card => card == null);
        PositionCards(playerCards, playerArea);
    }

    public void AddCardToOpponent()
    {
        Card card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            if (CardSpriteManager.Instance != null)
            {
                card.cardSprite = CardSpriteManager.Instance
                    .GetSprite(card.color, card.type, card.number);
            }
            opponentCardData.Add(card);
            GameObject cardGO = SpawnCardBack(opponentArea);
            opponentCards.Add(cardGO);
            PositionCards(opponentCards, opponentArea);
            GameManager.Instance.opponentHandCount++;

            // Sync with AI
            OpponentAI ai = GameManager.Instance.opponentAI;
            if (ai != null) ai.AddCardToHand(card);
        }
    }

    public void RemoveOpponentCard(Card card)
    {
        // Remove from data list
        opponentCardData.Remove(card);

        // Remove visual
        if (opponentArea.childCount > 0)
        {
            Destroy(opponentArea.GetChild(
                opponentArea.childCount - 1).gameObject);
        }

        // Remove from GO list
        if (opponentCards.Count > 0)
            opponentCards.RemoveAt(opponentCards.Count - 1);

        StartCoroutine(RepositionOpponentCards());
    }

    IEnumerator RepositionOpponentCards()
    {
        yield return new WaitForEndOfFrame();
        opponentCards.RemoveAll(card => card == null);
        PositionCards(opponentCards, opponentArea);
    }

    public int GetPlayerCardCount()
    {
        playerCards.RemoveAll(card => card == null);
        return playerCards.Count;
    }
}
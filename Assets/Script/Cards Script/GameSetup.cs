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
    public float cardSpacing = 85f;

    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> opponentCards = new List<GameObject>();
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
        Canvas.ForceUpdateCanvases();

        Debug.Log("=== GAME SETUP STARTING ===");

        ClearArea(playerArea);
        ClearArea(opponentArea);
        playerCards.Clear();
        opponentCards.Clear();
        opponentCardData.Clear();

        for (int i = 0; i < startingCards; i++)
        {
            Card card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                if (CardSpriteManager.Instance != null)
                    card.cardSprite = CardSpriteManager.Instance
                        .GetSprite(card.color, card.type, card.number);
                opponentCardData.Add(card);
                GameObject cardGO = SpawnCardBack(opponentArea);
                opponentCards.Add(cardGO);
            }
        }

        for (int i = 0; i < startingCards; i++)
        {
            Card card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                GameObject cardGO = SpawnPlayerCard(card, playerArea);
                playerCards.Add(cardGO);
            }
        }

        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        PositionCards(opponentCards, opponentArea, false);
        PositionCards(playerCards, playerArea, false);

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < opponentCards.Count; i++)
        {
            if (opponentCards[i] == null) continue;
            RectTransform rt =
                opponentCards[i].GetComponent<RectTransform>();
            CardAnimator anim =
                opponentCards[i].GetComponent<CardAnimator>();
            if (anim != null && rt != null)
                StartCoroutine(anim.DealAnimation(
                    rt.anchoredPosition, i * 0.07f));
        }

        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i] == null) continue;
            RectTransform rt =
                playerCards[i].GetComponent<RectTransform>();
            CardAnimator anim =
                playerCards[i].GetComponent<CardAnimator>();
            if (anim != null && rt != null)
                StartCoroutine(anim.DealAnimation(
                    rt.anchoredPosition, i * 0.07f));
        }

        OpponentAI ai = GameManager.Instance.opponentAI;
        if (ai != null)
        {
            ai.SetCards(opponentCardData);
            Debug.Log("AI cards set: " + opponentCardData.Count);
        }

        Card firstCard = DeckManager.Instance.DrawCard();
        if (firstCard != null && discardPileImage != null)
        {
            if (CardSpriteManager.Instance != null)
                firstCard.cardSprite = CardSpriteManager.Instance
                    .GetSprite(firstCard.color,
                        firstCard.type, firstCard.number);
            if (firstCard.cardSprite != null)
                discardPileImage.sprite = firstCard.cardSprite;
            GameManager.Instance.topCardOnDiscardPile = firstCard;
        }

        Debug.Log("=== SETUP COMPLETE ===");
    }

    void PositionCards(List<GameObject> cards, Transform area,
        bool animate = false, float delayPerCard = 0.08f)
    {
        int count = cards.Count;
        if (count == 0) return;

        float spacing = cardSpacing;
        RectTransform areaRect = area.GetComponent<RectTransform>();
        float areaWidth = (areaRect != null && areaRect.rect.width > 0)
            ? areaRect.rect.width : 1280f;

        float maxWidth = areaWidth - 100f;
        float totalWidth = (count - 1) * spacing;

        if (totalWidth > maxWidth && count > 1)
        {
            spacing = maxWidth / (count - 1);
            totalWidth = maxWidth;
        }

        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            if (cards[i] == null) continue;

            RectTransform cardRect =
                cards[i].GetComponent<RectTransform>();
            Vector2 targetPos =
                new Vector2(startX + (i * spacing), 0);

            if (cardRect != null)
                cardRect.anchoredPosition = targetPos;

            CardHover hover = cards[i].GetComponent<CardHover>();
            if (hover != null) hover.UpdateOriginalPosition();
        }
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
            card.cardSprite = CardSpriteManager.Instance
                .GetSprite(card.color, card.type, card.number);

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
                card.cardSprite = CardSpriteManager.Instance
                    .GetSprite(card.color, card.type, card.number);

            GameObject cardGO = SpawnPlayerCard(card, playerArea);
            playerCards.Add(cardGO);
            PositionCards(playerCards, playerArea);

            RectTransform cardRect =
                cardGO.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                Vector2 finalPos = cardRect.anchoredPosition;
                CardAnimator anim =
                    cardGO.GetComponent<CardAnimator>();
                if (anim != null)
                    StartCoroutine(anim.DrawAnimation(finalPos));
            }

            GameManager.Instance.playerHandCount++;
        }
    }

    public void RemoveCardFromPlayer(GameObject cardGO)
    {
        playerCards.Remove(cardGO);
        Destroy(cardGO);
        GameManager.Instance.playerHandCount--;
        StartCoroutine(RepositionAfterRemove());
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
                card.cardSprite = CardSpriteManager.Instance
                    .GetSprite(card.color, card.type, card.number);

            opponentCardData.Add(card);
            GameObject cardGO = SpawnCardBack(opponentArea);
            opponentCards.Add(cardGO);
            PositionCards(opponentCards, opponentArea, false);

            RectTransform cardRect =
                cardGO.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                Vector2 finalPos = cardRect.anchoredPosition;
                CardAnimator anim =
                    cardGO.GetComponent<CardAnimator>();
                if (anim != null)
                    StartCoroutine(anim.DrawAnimation(finalPos));
            }

            GameManager.Instance.opponentHandCount++;

            OpponentAI ai = GameManager.Instance.opponentAI;
            if (ai != null) ai.AddCardToHand(card);
        }
    }

    public void RemoveOpponentCard(Card card)
    {
        opponentCardData.Remove(card);
        if (opponentArea.childCount > 0)
            Destroy(opponentArea.GetChild(
                opponentArea.childCount - 1).gameObject);
        if (opponentCards.Count > 0)
            opponentCards.RemoveAt(opponentCards.Count - 1);
        StartCoroutine(RepositionOpponentCards());
    }

    public void RemoveOpponentCardByData(Card card)
    {
        opponentCardData.Remove(card);
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
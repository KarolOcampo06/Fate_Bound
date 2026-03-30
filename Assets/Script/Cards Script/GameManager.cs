using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool isPlayerTurn = true;
    public int playerHandCount = 7;
    public int opponentHandCount = 7;

    [Header("Gameplay Logic")]
    public Card topCardOnDiscardPile;

    [Header("References")]
    public OpponentAI opponentAI;
    public WinLoseManager winLoseManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("FateBound Started!");
    }

    public void PlayerDrawsCard()
    {
        if (!isPlayerTurn)
        {
            Debug.Log("Not player's turn!");
            return;
        }
        GameSetup.Instance.AddCardToPlayer();
        Debug.Log("Player drew a card!");
        GiveOpponentTurn();
    }

    public void PlayCard(GameObject cardGO)
    {
        if (!isPlayerTurn)
        {
            Debug.Log("Not player's turn!");
            return;
        }

        CardObject cardObj = cardGO.GetComponent<CardObject>();
        if (cardObj == null) return;

        if (IsMoveLegal(cardObj.cardData))
        {
            CardClick click = cardGO.GetComponent<CardClick>();
            if (click != null) click.enabled = false;

            CardHover hover = cardGO.GetComponent<CardHover>();
            if (hover != null) hover.enabled = false;

            StartCoroutine(PlayCardWithAnimation(cardGO, cardObj));
        }
        else
        {
            Debug.Log("Illegal move! Card does not match " +
                "color or number.");
        }
    }

    IEnumerator PlayCardWithAnimation(GameObject cardGO,
        CardObject cardObj)
    {
        CardAnimator anim = cardGO.GetComponent<CardAnimator>();
        if (anim != null &&
            GameSetup.Instance.discardPileImage != null)
        {
            bool animDone = false;
            StartCoroutine(anim.PlayAnimation(
                GameSetup.Instance.discardPileImage
                    .GetComponent<RectTransform>(),
                () => animDone = true));
            yield return new WaitUntil(() => animDone);
        }

        topCardOnDiscardPile = cardObj.cardData;
        if (GameSetup.Instance.discardPileImage != null)
            GameSetup.Instance.discardPileImage.sprite =
                cardObj.cardData.cardSprite;

        GameSetup.Instance.RemoveCardFromPlayer(cardGO);
        Debug.Log("Player played: " + cardObj.cardData.cardName);

        if (GameSetup.Instance.GetPlayerCardCount() == 0)
        {
            WinLoseManager.Instance?.PlayerWins();
            yield break;
        }

        if (GameSetup.Instance.GetPlayerCardCount() == 1)
            WinLoseManager.Instance?.FateBoundAlert();

        if (cardObj.cardData.type == CardType.Number)
            GiveOpponentTurn();
        else
            HandleSpecialCard(cardObj.cardData);
    }

    void HandleSpecialCard(Card card)
    {
        CardEffectAnimator.Instance?.ShowEffect(card.type);

        switch (card.type)
        {
            case CardType.Block:
                Debug.Log("BLOCK! Opponent loses their turn.");
                isPlayerTurn = true;
                break;

            case CardType.Reverse:
                Debug.Log("REVERSE! Acts like Block in 2P.");
                isPlayerTurn = true;
                break;

            case CardType.DrawTwo:
                Debug.Log("DRAW TWO! Opponent draws 2 cards.");
                GameSetup.Instance.AddCardToOpponent();
                GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 2;
                isPlayerTurn = true;
                break;

            case CardType.DrawFour:
                Debug.Log("DRAW FOUR! Opponent draws 4 cards.");
                for (int i = 0; i < 4; i++)
                    GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 4;
                isPlayerTurn = true;
                break;

            case CardType.RollDice:
                Debug.Log("ROLL DICE!");
                isPlayerTurn = false;
                DiceManager.Instance?.RollDice(OnDiceResult);
                break;
        }
    }

    void OnDiceResult(int result)
    {
        Debug.Log("Dice result: " + result +
            " — Opponent draws " + result + " cards!");
        for (int i = 0; i < result; i++)
            GameSetup.Instance.AddCardToOpponent();
        opponentHandCount += result;
        isPlayerTurn = true;
    }

    public bool IsMoveLegal(Card card)
    {
        if (topCardOnDiscardPile == null) return true;

        if (card.color == topCardOnDiscardPile.color) return true;

        if (card.type == CardType.Number &&
            topCardOnDiscardPile.type == CardType.Number &&
            card.number == topCardOnDiscardPile.number) return true;

        if (card.type != CardType.Number &&
            topCardOnDiscardPile.type != CardType.Number &&
            card.type == topCardOnDiscardPile.type) return true;

        return false;
    }

    public void GiveOpponentTurn()
    {
        isPlayerTurn = false;
        Debug.Log("Opponent's turn!");
        Invoke("SimulateOpponentTurn", 1.5f);
    }

    public void GivePlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Player's turn!");
    }

    void SimulateOpponentTurn()
    {
        if (opponentAI != null)
            opponentAI.StartOpponentTurn();
        else
            GivePlayerTurn();
    }

    public void EndTurn()
    {
        if (isPlayerTurn)
            GiveOpponentTurn();
        else
            GivePlayerTurn();
    }
}
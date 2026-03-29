using UnityEngine;
using UnityEngine.UI;
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

        // After drawing, turn passes to opponent
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
            // Update discard pile
            topCardOnDiscardPile = cardObj.cardData;
            if (GameSetup.Instance.discardPileImage != null)
            {
                GameSetup.Instance.discardPileImage.sprite =
                    cardObj.cardData.cardSprite;
            }

            // Remove card from player hand
            GameSetup.Instance.RemoveCardFromPlayer(cardGO);

            Debug.Log("Player played: " + cardObj.cardData.cardName);

            // Check win condition
            if (GameSetup.Instance.GetPlayerCardCount() == 0)
            {
                WinLoseManager.Instance?.PlayerWins();
                return;
            }

            // Check Fatebound
            if (GameSetup.Instance.GetPlayerCardCount() == 1)
            {
                WinLoseManager.Instance?.FateBoundAlert();
            }

            // Handle based on card type
            if (cardObj.cardData.type == CardType.Number)
            {
                // Number card — turn passes to opponent
                Debug.Log("Number card played — opponent's turn");
                GiveOpponentTurn();
            }
            else
            {
                // Special card — handle effect
                HandleSpecialCard(cardObj.cardData);
            }
        }
        else
        {
            Debug.Log("Illegal move! Card does not match " +
                "color or number.");
        }
    }

    void HandleSpecialCard(Card card)
    {
        // Show visual effect
        CardEffectAnimator.Instance?.ShowEffect(card.type);

        switch (card.type)
        {
            case CardType.Block:
                // Opponent loses their turn
                // Player gets to play again
                Debug.Log("BLOCK! Opponent loses their turn.");
                Debug.Log("Player plays again!");
                isPlayerTurn = true;
                // Do NOT call GiveOpponentTurn
                break;

            case CardType.Reverse:
                // In 2 player, Reverse = Block
                // Player gets to play again
                Debug.Log("REVERSE! Acts like Block in 2P.");
                Debug.Log("Player plays again!");
                isPlayerTurn = true;
                // Do NOT call GiveOpponentTurn
                break;

            case CardType.DrawTwo:
                // Opponent draws 2 cards AND loses their turn
                // Player gets to play again
                Debug.Log("DRAW TWO! Opponent draws 2 cards.");
                GameSetup.Instance.AddCardToOpponent();
                GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 2;
                Debug.Log("Player plays again!");
                isPlayerTurn = true;
                // Do NOT call GiveOpponentTurn
                break;

            case CardType.DrawFour:
                // Opponent draws 4 cards AND loses their turn
                // Player gets to play again
                Debug.Log("DRAW FOUR! Opponent draws 4 cards.");
                for (int i = 0; i < 4; i++)
                    GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 4;
                Debug.Log("Player plays again!");
                isPlayerTurn = true;
                // Do NOT call GiveOpponentTurn
                break;

            case CardType.RollDice:
                // Roll dice — opponent draws that many cards
                // Player gets to play again after dice result
                Debug.Log("ROLL DICE!");
                isPlayerTurn = false; // Lock until dice result
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

        // Player gets to play again after Roll Dice
        isPlayerTurn = true;
        Debug.Log("Player plays again after Roll Dice!");
    }

    public bool IsMoveLegal(Card card)
    {
        // No card on discard pile yet — any card is legal
        if (topCardOnDiscardPile == null) return true;

        // RollDice and DrawFour are wild — always legal
        if (card.type == CardType.RollDice ||
            card.type == CardType.DrawFour) return true;

        // Match by color
        if (card.color == topCardOnDiscardPile.color) return true;

        // Match by number (number cards only)
        if (card.type == CardType.Number &&
            topCardOnDiscardPile.type == CardType.Number &&
            card.number == topCardOnDiscardPile.number) return true;

        // Match by type (special cards)
        if (card.type != CardType.Number &&
            card.type == topCardOnDiscardPile.type) return true;

        return false;
    }

    // Give turn to opponent
    public void GiveOpponentTurn()
    {
        isPlayerTurn = false;
        Debug.Log("Opponent's turn!");
        Invoke("SimulateOpponentTurn", 1.5f);
    }

    // Give turn back to player
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

    // Keep EndTurn for compatibility
    public void EndTurn()
    {
        if (isPlayerTurn)
            GiveOpponentTurn();
        else
            GivePlayerTurn();
    }
}
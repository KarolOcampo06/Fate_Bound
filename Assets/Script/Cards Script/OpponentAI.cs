using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class OpponentAI : MonoBehaviour
{
    [Header("Difficulty")]
    public GameSettings.Difficulty difficulty =
        GameSettings.Difficulty.Medium;

    private List<Card> opponentCards = new List<Card>();

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            difficulty = GameSettings.Instance.selectedDifficulty;
            Debug.Log("AI Difficulty: " + difficulty);
        }
    }

    public void SetCards(List<Card> cards)
    {
        opponentCards = new List<Card>(cards);
        Debug.Log("AI received " + opponentCards.Count + " cards");
    }

    public void AddCardToHand(Card card)
    {
        opponentCards.Add(card);
        Debug.Log("AI drew a card. Total: " + opponentCards.Count);
    }

    public void RemoveCardFromHand(Card card)
    {
        opponentCards.Remove(card);
        Debug.Log("AI played a card. Total: " + opponentCards.Count);
    }

    public void StartOpponentTurn()
    {
        Debug.Log("Opponent thinking... (" + difficulty + ")");
        StartCoroutine(ThinkAndPlay());
    }

    IEnumerator ThinkAndPlay()
    {
        float thinkTime = GetThinkTime();
        yield return new WaitForSeconds(thinkTime);

        // Check if game over
        if (GameSetup.Instance.GetPlayerCardCount() == 0)
        {
            WinLoseManager.Instance?.PlayerLoses();
            yield break;
        }

        Card topCard = GameManager.Instance.topCardOnDiscardPile;
        List<Card> playableCards = GetPlayableCards(topCard);

        Debug.Log("AI has " + opponentCards.Count +
            " cards. Playable: " + playableCards.Count);

        if (playableCards.Count > 0)
        {
            // AI has playable cards — play one!
            Card cardToPlay = SelectCard(playableCards, topCard);
            PlayCard(cardToPlay);
        }
        else
        {
            // No playable cards — draw one
            Debug.Log("AI has no playable card — drawing!");
            GameSetup.Instance.AddCardToOpponent();

            // Check if drawn card is playable
            yield return new WaitForSeconds(0.5f);

            Card topCardNow =
                GameManager.Instance.topCardOnDiscardPile;
            List<Card> newPlayable = GetPlayableCards(topCardNow);

            if (newPlayable.Count > 0 &&
                difficulty == GameSettings.Difficulty.Hard)
            {
                // Hard AI plays drawn card if possible
                Card cardToPlay = SelectCard(newPlayable, topCardNow);
                PlayCard(cardToPlay);
            }
            else
            {
                // Give turn back to player
                GameManager.Instance.GivePlayerTurn();
            }
        }
    }

    float GetThinkTime()
    {
        switch (difficulty)
        {
            case GameSettings.Difficulty.Easy:
                return Random.Range(0.5f, 1f);
            case GameSettings.Difficulty.Medium:
                return Random.Range(1f, 2f);
            case GameSettings.Difficulty.Hard:
                return Random.Range(1.5f, 2f);
            default: return 1.5f;
        }
    }

    List<Card> GetPlayableCards(Card topCard)
    {
        List<Card> playable = new List<Card>();
        if (topCard == null)
        {
            playable.AddRange(opponentCards);
            return playable;
        }

        foreach (Card card in opponentCards)
        {
            if (GameManager.Instance.IsMoveLegal(card))
                playable.Add(card);
        }
        return playable;
    }

    Card SelectCard(List<Card> playable, Card topCard)
    {
        switch (difficulty)
        {
            case GameSettings.Difficulty.Easy:
                // Random card
                return playable[Random.Range(0, playable.Count)];

            case GameSettings.Difficulty.Medium:
                return SelectMediumCard(playable);

            case GameSettings.Difficulty.Hard:
                return SelectHardCard(playable);

            default:
                return playable[Random.Range(0, playable.Count)];
        }
    }

    Card SelectMediumCard(List<Card> playable)
    {
        // 70% prefer number cards
        List<Card> numbers = new List<Card>();
        List<Card> specials = new List<Card>();

        foreach (Card c in playable)
        {
            if (c.type == CardType.Number) numbers.Add(c);
            else specials.Add(c);
        }

        if (numbers.Count > 0 && Random.Range(0f, 1f) < 0.7f)
            return numbers[Random.Range(0, numbers.Count)];

        if (specials.Count > 0)
            return specials[Random.Range(0, specials.Count)];

        return playable[Random.Range(0, playable.Count)];
    }

    Card SelectHardCard(List<Card> playable)
    {
        // Priority: DrawFour > RollDice > DrawTwo > Block > Reverse > Number
        CardType[] priority = {
            CardType.DrawFour,
            CardType.RollDice,
            CardType.DrawTwo,
            CardType.Block,
            CardType.Reverse,
            CardType.Number
        };

        foreach (CardType type in priority)
        {
            foreach (Card card in playable)
            {
                if (card.type == type) return card;
            }
        }

        return playable[0];
    }

    void PlayCard(Card card)
    {
        Debug.Log("AI plays: " + card.cardName);

        // Update discard pile
        GameManager.Instance.topCardOnDiscardPile = card;

        // Update visual
        if (GameSetup.Instance.discardPileImage != null &&
            card.cardSprite != null)
        {
            GameSetup.Instance.discardPileImage.sprite =
                card.cardSprite;
        }

        // Remove from AI hand and visual
        RemoveCardFromHand(card);
        GameSetup.Instance.RemoveOpponentCard(card);
        GameManager.Instance.opponentHandCount--;

        // Check opponent wins
        if (opponentCards.Count == 0)
        {
            Debug.Log("Opponent wins!");
            WinLoseManager.Instance?.PlayerLoses();
            return;
        }

        // Check Fatebound
        if (opponentCards.Count == 1)
            Debug.Log("Opponent Fatebound!");

        // Handle special cards
        HandleSpecialCard(card);
    }

    void HandleSpecialCard(Card card)
    {
        CardEffectAnimator.Instance?.ShowEffect(card.type);

        switch (card.type)
        {
            case CardType.Number:
                GameManager.Instance.GivePlayerTurn();
                break;

            case CardType.Block:
            case CardType.Reverse:
                Debug.Log("AI Block/Reverse — player skips!");
                GameManager.Instance.isPlayerTurn = false;
                Invoke("StartOpponentTurn", 2f);
                break;

            case CardType.DrawTwo:
                Debug.Log("AI Draw Two — player draws 2!");
                GameSetup.Instance.AddCardToPlayer();
                GameSetup.Instance.AddCardToPlayer();
                GameManager.Instance.playerHandCount += 2;
                GameManager.Instance.isPlayerTurn = false;
                Invoke("StartOpponentTurn", 2f);
                break;

            case CardType.DrawFour:
                Debug.Log("AI Draw Four — player draws 4!");
                for (int i = 0; i < 4; i++)
                    GameSetup.Instance.AddCardToPlayer();
                GameManager.Instance.playerHandCount += 4;
                GameManager.Instance.isPlayerTurn = false;
                Invoke("StartOpponentTurn", 2f);
                break;

            case CardType.RollDice:
                Debug.Log("AI Roll Dice!");
                DiceManager.Instance?.RollDice((result) =>
                {
                    Debug.Log("AI dice: " + result +
                        " — player draws " + result);
                    for (int i = 0; i < result; i++)
                        GameSetup.Instance.AddCardToPlayer();
                    GameManager.Instance.playerHandCount += result;
                    GameManager.Instance.isPlayerTurn = false;
                    Invoke("StartOpponentTurn", 2f);
                });
                break;
        }
    }
}
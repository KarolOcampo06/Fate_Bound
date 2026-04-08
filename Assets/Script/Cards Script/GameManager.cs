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
        // DO NOT use DontDestroyOnLoad
        Time.timeScale = 1f;
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
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

            // Hide guide when card is played
            CardGuideManager.Instance?.HideCardGuide();

            // ── FIX: Clear ALL glows immediately the moment
            //    a card is played — before animation even starts.
            //    This stops the old hint glows right away.
            ClearAllGlows();

            StartCoroutine(PlayCardWithAnimation(cardGO, cardObj));
        }
        else
        {
            Debug.Log("Illegal move!");
            // Show illegal move guide
            CardGuideManager.Instance?.ShowIllegalMove(cardObj.cardData);
        }
    }

    IEnumerator PlayCardWithAnimation(GameObject cardGO,
        CardObject cardObj)
    {
        // Shimmer before animation
        CardVFX vfx = cardGO.GetComponent<CardVFX>();
        if (vfx != null) vfx.PlayShimmer();

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

        // Burst at discard pile position
        if (GameSetup.Instance.discardPileImage != null)
        {
            Color burstColor = GetCardColor(cardObj.cardData.color);
            VFXManager.Instance?.SpawnCardBurst(
                GameSetup.Instance.discardPileImage
                    .transform.position, burstColor);
        }

        topCardOnDiscardPile = cardObj.cardData;

        if (GameSetup.Instance.discardPileImage != null)
            GameSetup.Instance.discardPileImage.sprite =
                cardObj.cardData.cardSprite;

        GameSetup.Instance.RemoveCardFromPlayer(cardGO);

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

    Color GetCardColor(CardColor color)
    {
        switch (color)
        {
            case CardColor.Red: return new Color(1f, 0.2f, 0.2f);
            case CardColor.Gold: return new Color(1f, 0.85f, 0.1f);
            case CardColor.Blue: return new Color(0.2f, 0.5f, 1f);
            case CardColor.Purple: return new Color(0.6f, 0.1f, 0.8f);
            default: return Color.white;
        }
    }

    void HandleSpecialCard(Card card)
    {
        // ALWAYS pass true here — player played the card
        CardEffectAnimator.Instance?.ShowEffect(card.type, true);

        switch (card.type)
        {
            case CardType.Block:
                VFXManager.Instance?.FlashScreen(
                    new Color(0.2f, 0.6f, 1f));
                Debug.Log("BLOCK! Opponent loses their turn.");
                isPlayerTurn = true;
                break;

            case CardType.Reverse:
                VFXManager.Instance?.FlashScreen(
                    new Color(0.2f, 0.8f, 0.2f));
                Debug.Log("REVERSE! Acts like Block in 2P.");
                isPlayerTurn = true;
                break;

            case CardType.DrawTwo:
                VFXManager.Instance?.FlashScreen(
                    new Color(1f, 0.6f, 0.2f));
                Debug.Log("DRAW TWO! Opponent draws 2 cards.");
                GameSetup.Instance.AddCardToOpponent();
                GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 2;
                isPlayerTurn = true;
                break;

            case CardType.DrawFour:
                VFXManager.Instance?.FlashScreen(
                    new Color(1f, 0.2f, 0.2f));
                Debug.Log("DRAW FOUR! Opponent draws 4 cards.");
                for (int i = 0; i < 4; i++)
                    GameSetup.Instance.AddCardToOpponent();
                opponentHandCount += 4;
                isPlayerTurn = true;
                break;

            case CardType.RollDice:
                VFXManager.Instance?.FlashScreen(
                    new Color(0.8f, 0.2f, 0.8f));
                Debug.Log("ROLL DICE!");
                isPlayerTurn = false;
                DiceManager.Instance?.RollDice(OnDiceResult);
                break;
        }

        // ── FIX: After a special card, the turn stays with
        //    the player (except RollDice). The discard pile has
        //    changed, so we must:
        //    1. Clear all old glows from the previous card
        //    2. Re-evaluate which cards are now valid to play
        //       so the 10-second hint timer restarts fresh
        //       based on the NEW top card.
        if (isPlayerTurn)
            UpdatePlayableCardGlow();
    }

    void OnDiceResult(int result)
    {
        Debug.Log("Dice result: " + result +
            " — Opponent draws " + result + " cards!");
        for (int i = 0; i < result; i++)
            GameSetup.Instance.AddCardToOpponent();
        opponentHandCount += result;
        isPlayerTurn = true;

        // ── FIX: RollDice also keeps player's turn,
        //    so refresh glows here too after dice resolves.
        UpdatePlayableCardGlow();
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

    public void GivePlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Player's turn!");
        UpdatePlayableCardGlow();
        // Show turn start guide
        CardGuideManager.Instance?.ShowTurnStartGuide();
    }

    public void UpdatePlayableCardGlow()
    {
        foreach (GameObject cardGO in
            GameSetup.Instance.playerCards)
        {
            if (cardGO == null) continue;
            CardObject cardObj = cardGO.GetComponent<CardObject>();
            CardVFX vfx = cardGO.GetComponent<CardVFX>();
            if (cardObj != null && vfx != null)
            {
                bool playable = IsMoveLegal(cardObj.cardData);
                vfx.SetPlayable(playable);
            }
        }
    }

    // ── FIX: New helper — clears ALL glows immediately
    //    without changing turn state. Used when a card
    //    is played so old hint glows vanish right away.
    public void ClearAllGlows()
    {
        foreach (GameObject cardGO in
            GameSetup.Instance.playerCards)
        {
            if (cardGO == null) continue;
            CardVFX vfx = cardGO.GetComponent<CardVFX>();
            if (vfx != null) vfx.ResetAll();
        }
    }

    void SimulateOpponentTurn()
    {
        if (opponentAI != null)
            opponentAI.StartOpponentTurn();
        else
            GivePlayerTurn();
    }

    public void GiveOpponentTurn()
    {
        isPlayerTurn = false;
        Debug.Log("Opponent's turn!");

        // Reset all card glows
        foreach (GameObject cardGO in
            GameSetup.Instance.playerCards)
        {
            if (cardGO == null) continue;
            CardVFX vfx = cardGO.GetComponent<CardVFX>();
            if (vfx != null) vfx.ResetAll();
        }

        Invoke("SimulateOpponentTurn", 1.5f);
    }
}
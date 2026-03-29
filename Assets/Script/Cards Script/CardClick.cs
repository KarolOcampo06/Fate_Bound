using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Selection Visual")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private Image cardImage;
    private bool isSelected = false;

    void Start()
    {
        cardImage = GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // If it's not player's turn, do nothing
        if (!GameManager.Instance.isPlayerTurn) return;

        if (!isSelected)
        {
            // Deselect all other cards first
            DeselectAllCards();
            SelectCard();
        }
        else
        {
            // If already selected, try to play it
            TryPlayCard();
        }
    }

    void DeselectAllCards()
    {
        // Find all cards and deselect them
        CardClick[] allCards = FindObjectsOfType<CardClick>();
        foreach (CardClick card in allCards)
        {
            card.Deselect();
        }
    }

    void SelectCard()
    {
        isSelected = true;
        if (cardImage != null)
        {
            cardImage.color = selectedColor;
        }
        Debug.Log("Card selected: " + gameObject.name);
    }

    void TryPlayCard()
    {
        // Check if move is legal before playing
        CardObject cardObj = GetComponent<CardObject>();
        if (cardObj != null && GameManager.Instance != null)
        {
            if (GameManager.Instance.IsMoveLegal(cardObj.cardData))
            {
                GameManager.Instance.PlayCard(gameObject);
                Debug.Log("Card played: " + gameObject.name);
            }
            else
            {
                // Illegal move — shake the card
                Debug.Log("Illegal move! Card doesn't match.");
                Deselect();
            }
        }
    }

    public void DeselectCard()
    {
        if (cardImage != null)
        {
            cardImage.color = normalColor;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Deselect()
    {
        isSelected = false;
        DeselectCard();
    }

    public void PlayThisCard()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayCard(gameObject);
        }
    }
}
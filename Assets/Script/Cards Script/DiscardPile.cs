using UnityEngine;
using UnityEngine.UI;

public class DiscardPile : MonoBehaviour
{
    public static DiscardPile Instance;

    public Image topCardImage;
    public Card topCard;

    void Awake()
    {
        Instance = this;
    }

    public void PlaceCard(Card card)
    {
        topCard = card;
        if (topCardImage != null && card.cardSprite != null)
        {
            topCardImage.sprite = card.cardSprite;
        }
        Debug.Log("Discard pile top card: " + card.cardName);
    }
}
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using static System.Net.Mime.MediaTypeNames;

public class CardObject : MonoBehaviour
{
    public Card cardData;
    public Image cardImage;

    public void SetCard(Card data)
    {
        cardData = data;

        if (cardImage == null)
            cardImage = GetComponent<Image>();

        if (cardImage != null && data.cardSprite != null)
        {
            cardImage.sprite = data.cardSprite;
        }

        gameObject.name = data.cardName;
    }
}
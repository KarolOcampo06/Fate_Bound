using UnityEngine;

// Card.cs
// This class represents a single card in the game
// It stores all the information about what the card is

[System.Serializable] // This lets you see it in the Unity Inspector
public class Card
{
    // What color is this card? (Red, Gold, Blue, Purple)
    public CardColor color;

    // What type of card is it? (Number, Block, DrawTwo, etc.)
    public CardType type;

    // If it's a number card, what number? (1-9)
    // If it's a special card, this will be 0
    public int number;

    // The visual image of this card (drag the card sprite here)
    public Sprite cardSprite;

    // Optional: The name of the card for debugging
    public string cardName;

    // Constructor - A way to create a card easily
    public Card(CardColor color, CardType type, int number = 0)
    {
        this.color = color;
        this.type = type;
        this.number = number;

        // Automatically generate a card name
        if (type == CardType.Number)
        {
            cardName = color.ToString() + " " + number;
        }
        else
        {
            cardName = color.ToString() + " " + type.ToString();
        }
    }
}
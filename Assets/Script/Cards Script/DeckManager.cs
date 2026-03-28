using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public List<Card> drawPile = new List<Card>();
    public GameObject cardPrefab;

    void Awake()
    {
        Instance = this;
        BuildDeck();
        ShuffleDeck();
    }

    void BuildDeck()
    {
        drawPile.Clear();

        // Add ALL cards for each color
        foreach (CardColor color in
            System.Enum.GetValues(typeof(CardColor)))
        {
            // Number cards 1-9 (2 copies each)
            for (int number = 1; number <= 9; number++)
            {
                drawPile.Add(new Card(color, CardType.Number, number));
                drawPile.Add(new Card(color, CardType.Number, number));
            }

            // Special cards (2 copies each)
            drawPile.Add(new Card(color, CardType.Block, 0));
            drawPile.Add(new Card(color, CardType.Block, 0));
            drawPile.Add(new Card(color, CardType.Reverse, 0));
            drawPile.Add(new Card(color, CardType.Reverse, 0));
            drawPile.Add(new Card(color, CardType.DrawTwo, 0));
            drawPile.Add(new Card(color, CardType.DrawTwo, 0));
            drawPile.Add(new Card(color, CardType.DrawFour, 0));
            drawPile.Add(new Card(color, CardType.DrawFour, 0));
            drawPile.Add(new Card(color, CardType.RollDice, 0));
            drawPile.Add(new Card(color, CardType.RollDice, 0));
        }

        Debug.Log("Deck built: " + drawPile.Count + " cards!");
    }

    void ShuffleDeck()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
        Debug.Log("Deck shuffled!");
    }

    public Card DrawCard()
    {
        // If deck is empty rebuild and reshuffle automatically!
        if (drawPile.Count == 0)
        {
            Debug.Log("Deck empty! Rebuilding...");
            BuildDeck();
            ShuffleDeck();
            Debug.Log("Deck rebuilt: " + drawPile.Count + " cards!");
        }

        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        return card;
    }
}
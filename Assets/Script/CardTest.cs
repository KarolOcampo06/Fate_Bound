using UnityEngine;

public class CardTest : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Debug.Log("=== CARD TEST STARTING ===");

        // Create a Red 5 card
        Card redFive = new Card(CardColor.Red, CardType.Number, 5);
        UnityEngine.Debug.Log("Created card: " + redFive.cardName);

        // Create a Blue Block card
        Card blueBlock = new Card(CardColor.Blue, CardType.Block);
        UnityEngine.Debug.Log("Created card: " + blueBlock.cardName);

        // Create a Gold Draw Two card
        Card goldDrawTwo = new Card(CardColor.Gold, CardType.DrawTwo);
        UnityEngine.Debug.Log("Created card: " + goldDrawTwo.cardName);

        // Create a Purple Roll Dice card
        Card purpleRollDice = new Card(CardColor.Purple, CardType.RollDice);
        UnityEngine.Debug.Log("Created card: " + purpleRollDice.cardName);

        UnityEngine.Debug.Log("=== CARD TEST COMPLETE ===");
    }
}
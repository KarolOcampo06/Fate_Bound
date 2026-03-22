using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Prefab")]
    public GameObject cardPrefab;

    [Header("Player Hand Area")]
    public Transform playerHandArea;

    [Header("Settings")]
    public int numberOfCards = 7;
    public float cardSpacing = 150f;

    void Start()
    {
        DisplayTestCards();
    }

    void DisplayTestCards()
    {
        foreach (Transform child in playerHandArea)
        {
            Destroy(child.gameObject);
        }

        float totalWidth = (numberOfCards - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, playerHandArea);

            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect == null)
            {
                cardRect = card.AddComponent<RectTransform>();
            }

            float xPosition = startX + (i * cardSpacing);
            cardRect.anchoredPosition = new Vector2(xPosition, 0);
            cardRect.localScale = Vector3.one;

            card.name = "Card_" + i;

            UnityEngine.Debug.Log("Created card " + i + " at X: " + xPosition);
        }

        UnityEngine.Debug.Log("Displayed " + numberOfCards + " cards in Player_Area");
    }
}
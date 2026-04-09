using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("UI Elements")]
    public Image slideIcon;
    public TextMeshProUGUI slideTitleText;
    public TextMeshProUGUI slideBodyText;
    public TextMeshProUGUI slideCounterText;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;

    [Header("Slide Images")]
    public Sprite[] cardImages;

    private int currentPage = 0;

    private string[] titles = {
        "Welcome to FateBound!",
        "How to Play",
        "The 4 Colors",
        "Skip Card",
        "Reverse Card",
        "Draw Two Card",
        "Draw Four Card",
        "Roll Dice Card",
        "Block Card",
        "Winning the Game"
    };

    private string[] descriptions = {
        "FateBound is a mystical UNO-style card game. You are a Fate Weaver battling to shape destiny. Be the first to empty your hand to win!",
        "Match cards by COLOR or NUMBER to the top card on the pile. If you cannot play, draw one card from the deck. Call 'Fatebound!' when you have one card left!",
        "There are 4 mystical colors in FateBound:\n\n🟡 Gold (Destiny)\n🔴 Red (Chaos)\n🔵 Blue (Fortune)\n🟣 Purple (Shadow)\n\nMatch any card by color or number!",
        "⏭️ SKIP — The next player loses their turn completely. Chain multiple Skips to keep skipping players in a row!",
        "🔄 REVERSE — Flips the direction of play. In a 2-player game, it acts like a Skip!",
        "➕ DRAW TWO — The next player must draw 2 cards and loses their turn!",
        "➕➕ DRAW FOUR — The next player must draw 4 cards and loses their turn. A powerful card — use it wisely!",
        "🎲 ROLL DICE — The next player draws 1 to 6 cards based on a dice roll. Pure fate decides their punishment!",
        "🛡️ BLOCK — The next player loses their turn completely. Use it to protect yourself at the right moment!",
        "Empty your hand before anyone else to win!\n\nRemember to call 'Fatebound!' when you have ONE card left or you will draw penalty cards.\n\nGood luck, Fate Weaver!"
    };

    void Start()
    {
        tutorialPanel.SetActive(false);
    }

    public void ShowTutorial()
    {
        currentPage = 0;
        tutorialPanel.SetActive(true);
        UpdatePage();
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialShown", 1);
        PlayerPrefs.Save();
    }

    public void NextPage()
    {
        if (currentPage < titles.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
        else
        {
            HideTutorial();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    public void SkipTutorial()
    {
        HideTutorial();
    }

    void UpdatePage()
    {
        slideTitleText.text = titles[currentPage];
        slideBodyText.text = descriptions[currentPage];
        slideCounterText.text = (currentPage + 1) + " / " + titles.Length;

        if (cardImages != null && currentPage < cardImages.Length && cardImages[currentPage] != null)
        {
            slideIcon.sprite = cardImages[currentPage];
            slideIcon.gameObject.SetActive(true);
        }
        else
        {
            slideIcon.gameObject.SetActive(false);
        }

        prevButton.gameObject.SetActive(currentPage > 0);

        TextMeshProUGUI nextText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
        if (nextText != null)
        {
            nextText.text = (currentPage == titles.Length - 1) ? "Finish" : "Next";
        }
    }
}
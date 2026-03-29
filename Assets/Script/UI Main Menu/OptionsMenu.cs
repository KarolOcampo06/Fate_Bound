using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = UnityEngine.Debug;

public class OptionsMenu : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Button Colors")]
    public Color selectedColor = new Color(0.2f, 0.8f, 0.2f);
    public Color normalColor = new Color(1f, 1f, 1f, 1f);

    [Header("Description Text")]
    public TextMeshProUGUI descriptionText;

    void Start()
    {
        // Make sure GameSettings exists
        if (GameSettings.Instance == null)
        {
            GameObject gs = new GameObject("GameSettings");
            gs.AddComponent<GameSettings>();
        }

        UpdateButtonColors();
        UpdateDescription();
    }

    public void SetEasy()
    {
        GameSettings.Instance.selectedDifficulty =
            GameSettings.Difficulty.Easy;
        Debug.Log("Difficulty: Easy");
        UpdateButtonColors();
        UpdateDescription();
    }

    public void SetMedium()
    {
        GameSettings.Instance.selectedDifficulty =
            GameSettings.Difficulty.Medium;
        Debug.Log("Difficulty: Medium");
        UpdateButtonColors();
        UpdateDescription();
    }

    public void SetHard()
    {
        GameSettings.Instance.selectedDifficulty =
            GameSettings.Difficulty.Hard;
        Debug.Log("Difficulty: Hard");
        UpdateButtonColors();
        UpdateDescription();
    }

    void UpdateButtonColors()
    {
        if (GameSettings.Instance == null) return;

        // Reset all to normal
        SetButtonColor(easyButton, normalColor);
        SetButtonColor(mediumButton, normalColor);
        SetButtonColor(hardButton, normalColor);

        // Highlight selected
        switch (GameSettings.Instance.selectedDifficulty)
        {
            case GameSettings.Difficulty.Easy:
                SetButtonColor(easyButton, selectedColor);
                break;
            case GameSettings.Difficulty.Medium:
                SetButtonColor(mediumButton, selectedColor);
                break;
            case GameSettings.Difficulty.Hard:
                SetButtonColor(hardButton, selectedColor);
                break;
        }
    }

    void SetButtonColor(Button btn, Color color)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = color;
    }

    void UpdateDescription()
    {
        if (descriptionText == null) return;
        if (GameSettings.Instance == null) return;

        switch (GameSettings.Instance.selectedDifficulty)
        {
            case GameSettings.Difficulty.Easy:
                descriptionText.text =
                    "Easy\nAI plays randomly.\n" +
                    "Good for beginners!";
                break;
            case GameSettings.Difficulty.Medium:
                descriptionText.text =
                    "Medium\nAI plays smart sometimes.\n" +
                    "A fair challenge!";
                break;
            case GameSettings.Difficulty.Hard:
                descriptionText.text =
                    "Hard\nAI always plays best card.\n" +
                    "Only for experts!";
                break;
        }
    }
}
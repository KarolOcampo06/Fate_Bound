using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject fateBoundPanel;

    [Header("UI Text")]
    public Text winText;
    public Text loseText;

    void Awake()
    {
        Instance = this;

        // Hide all panels at start
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (fateBoundPanel != null) fateBoundPanel.SetActive(false);
    }

    public void PlayerWins()
    {
        Debug.Log("PLAYER WINS!");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        // Stop the game
        Time.timeScale = 0f;
    }

    public void PlayerLoses()
    {
        Debug.Log("PLAYER LOSES!");
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
        // Stop the game
        Time.timeScale = 0f;
    }

    public void FateBoundAlert()
    {
        Debug.Log("FATEBOUND! One card left!");
        if (fateBoundPanel != null)
        {
            fateBoundPanel.SetActive(true);
            // Hide after 2 seconds
            Invoke("HideFateBound", 2f);
        }
    }

    void HideFateBound()
    {
        if (fateBoundPanel != null)
        {
            fateBoundPanel.SetActive(false);
        }
    }

    // Button functions
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
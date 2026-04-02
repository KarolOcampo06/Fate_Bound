using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject fateBoundPanel;

    [Header("Win Panel References")]
    public TextMeshProUGUI winTitleText;
    public TextMeshProUGUI winSubText;
    public Image winPanelBackground;

    [Header("Lose Panel References")]
    public TextMeshProUGUI loseTitleText;
    public TextMeshProUGUI loseSubText;
    public Image losePanelBackground;

    [Header("FateBound Panel References")]
    public TextMeshProUGUI fateBoundText;

    [Header("Screen Overlay")]
    public Image screenOverlay;

    [Header("Colors")]
    public Color winColor = new Color(1f, 0.85f, 0.1f, 1f);
    public Color loseColor = new Color(0.7f, 0.1f, 0.1f, 1f);
    public Color fateBoundColor = new Color(0.6f, 0.1f, 0.8f, 1f);

    void Awake()
    {
        Instance = this;
        HideAllPanels();
        ResetOverlay();
    }

    void ResetOverlay()
    {
        if (screenOverlay != null)
        {
            Color c = screenOverlay.color;
            c.a = 0f;
            screenOverlay.color = c;
            screenOverlay.raycastTarget = false;
        }
    }

    void HideAllPanels()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (fateBoundPanel != null)
            fateBoundPanel.SetActive(false);
    }

    // ── Player Wins ──────────────────────────────────────

    public void PlayerWins()
    {
        Debug.Log("PLAYER WINS!");
        StartCoroutine(ShowWinSequence());
    }

    IEnumerator ShowWinSequence()
    {
        VFXManager.Instance?.FlashScreen(
            new Color(1f, 0.85f, 0.1f), 0.5f);

        yield return new WaitForSeconds(0.3f);

        AudioManager.Instance?.PlayWin();

        yield return StartCoroutine(
            FadeOverlay(0f, 0.6f, 0.4f));

        if (winPanel != null)
        {
            winPanel.SetActive(true);

            if (winTitleText != null)
                winTitleText.text = "YOU WIN!";
            if (winSubText != null)
                winSubText.text = "Destiny has been fulfilled!";
            if (winPanelBackground != null)
                winPanelBackground.color = winColor;

            yield return StartCoroutine(
                AnimatePanel(winPanel, true));
        }

        VFXManager.Instance?.SpawnCardBurst(
            new Vector3(Screen.width / 2f,
                Screen.height / 2f, 0f),
            new Color(1f, 0.85f, 0.1f));

        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }

    // ── Player Loses ─────────────────────────────────────

    public void PlayerLoses()
    {
        Debug.Log("PLAYER LOSES!");
        StartCoroutine(ShowLoseSequence());
    }

    IEnumerator ShowLoseSequence()
    {
        VFXManager.Instance?.FlashScreen(
            new Color(0.8f, 0.1f, 0.1f), 0.5f);

        yield return new WaitForSeconds(0.3f);

        AudioManager.Instance?.PlayLose();

        yield return StartCoroutine(
            FadeOverlay(0f, 0.6f, 0.4f));

        if (losePanel != null)
        {
            losePanel.SetActive(true);

            if (loseTitleText != null)
                loseTitleText.text = "YOU LOSE!";
            if (loseSubText != null)
                loseSubText.text = "Fate was not on your side...";
            if (losePanelBackground != null)
                losePanelBackground.color = loseColor;

            yield return StartCoroutine(
                AnimatePanel(losePanel, true));
        }

        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }

    // ── FateBound Alert ──────────────────────────────────

    public void FateBoundAlert()
    {
        Debug.Log("FATEBOUND! One card left!");
        StartCoroutine(ShowFateBoundSequence());
    }

    IEnumerator ShowFateBoundSequence()
    {
        if (fateBoundPanel == null) yield break;

        string[] callouts = {
            "FATEBOUND!",
            "ONE CARD REMAINS!",
            "THE END IS NEAR!",
            "DESTINY AWAITS!",
            "LAST CARD STANDING!",
            "THE FINAL THREAD!",
            "FATE HANGS BY A THREAD!",
            "ONE STEP FROM VICTORY!",
            "THE WEAVE IS ALMOST COMPLETE!",
            "DARKNESS CLOSES IN!"
        };

        string randomCallout =
            callouts[Random.Range(0, callouts.Length)];

        VFXManager.Instance?.FlashScreen(
            new Color(0.6f, 0.1f, 0.8f), 0.3f);

        fateBoundPanel.SetActive(true);

        if (fateBoundText != null)
            fateBoundText.text = randomCallout;

        yield return StartCoroutine(
            AnimatePanel(fateBoundPanel, true));

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(
            AnimatePanel(fateBoundPanel, false));

        fateBoundPanel.SetActive(false);
    }

    // ── Panel Animation ──────────────────────────────────

    IEnumerator AnimatePanel(GameObject panel, bool show)
    {
        RectTransform rt =
            panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        float duration = 0.35f;
        float elapsed = 0f;

        Vector3 startScale = show ?
            Vector3.zero : Vector3.one;
        Vector3 endScale = show ?
            Vector3.one : Vector3.zero;

        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        if (rt != null) rt.localScale = startScale;
        cg.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float eased = EaseOutBack(t);

            if (rt != null)
                rt.localScale = Vector3.Lerp(
                    startScale, endScale, eased);
            cg.alpha = Mathf.Lerp(
                startAlpha, endAlpha, t);

            yield return null;
        }

        if (rt != null) rt.localScale = endScale;
        cg.alpha = endAlpha;
    }

    // ── Screen Overlay ───────────────────────────────────

    IEnumerator FadeOverlay(float from, float to,
        float duration)
    {
        if (screenOverlay == null) yield break;

        screenOverlay.raycastTarget = to > 0f;
        Color c = screenOverlay.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            screenOverlay.color = c;
            yield return null;
        }

        c.a = to;
        screenOverlay.color = c;
    }

    // ── Button Functions ─────────────────────────────────

    public void RestartGame()
    {
        // Reset everything before loading
        Time.timeScale = 1f;
        StartCoroutine(RestartWithFade());
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(GoToMenuWithFade());
    }

    IEnumerator RestartWithFade()
    {
        if (screenOverlay != null)
        {
            screenOverlay.raycastTarget = true;
            Color c = screenOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.4f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                screenOverlay.color = c;
                yield return null;
            }
            c.a = 1f;
            screenOverlay.color = c;
        }

        Time.timeScale = 1f;
        yield return null;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.ReloadCurrentScene();
        else
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
    }

    IEnumerator GoToMenuWithFade()
    {
        if (screenOverlay != null)
        {
            screenOverlay.raycastTarget = true;
            Color c = screenOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.4f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                screenOverlay.color = c;
                yield return null;
            }
            c.a = 1f;
            screenOverlay.color = c;
        }

        Time.timeScale = 1f;
        yield return null;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("MainMenuScene");
        else
            SceneManager.LoadScene("MainMenuScene");
    }

    // ── Easing ───────────────────────────────────────────

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) +
            c1 * Mathf.Pow(t - 1f, 2f);
    }
}
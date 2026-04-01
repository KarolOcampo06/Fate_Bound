using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Panel")]
    public GameObject pausePanel;
    public Image pauseOverlay;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;

        // Always force reset timeScale on awake
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseOverlay != null)
        {
            Color c = pauseOverlay.color;
            c.a = 0f;
            pauseOverlay.color = c;
            pauseOverlay.raycastTarget = false;
        }
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current
                .escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        StartCoroutine(ShowPausePanel());
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        StartCoroutine(HidePausePanel());
    }

    IEnumerator ShowPausePanel()
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.raycastTarget = true;
            Color c = pauseOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 0.7f, elapsed / 0.3f);
                pauseOverlay.color = c;
                yield return null;
            }
            c.a = 0.7f;
            pauseOverlay.color = c;
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            RectTransform rt =
                pausePanel.GetComponent<RectTransform>();
            CanvasGroup cg =
                pausePanel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = pausePanel.AddComponent<CanvasGroup>();

            rt.localScale = Vector3.zero;
            cg.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / 0.3f;
                rt.localScale = Vector3.Lerp(
                    Vector3.zero, Vector3.one,
                    EaseOutBack(t));
                cg.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            rt.localScale = Vector3.one;
            cg.alpha = 1f;
        }

        Time.timeScale = 0f;
    }

    IEnumerator HidePausePanel()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            RectTransform rt =
                pausePanel.GetComponent<RectTransform>();
            CanvasGroup cg =
                pausePanel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = pausePanel.AddComponent<CanvasGroup>();

            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / 0.2f;
                rt.localScale = Vector3.Lerp(
                    Vector3.one, Vector3.zero, t);
                cg.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            pausePanel.SetActive(false);
        }

        if (pauseOverlay != null)
        {
            Color c = pauseOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0.7f, 0f,
                    elapsed / 0.2f);
                pauseOverlay.color = c;
                yield return null;
            }
            c.a = 0f;
            pauseOverlay.color = c;
            pauseOverlay.raycastTarget = false;
        }
    }

    public void OnRestartButton()
    {
        StartCoroutine(RestartSequence());
    }

    public void OnMainMenuButton()
    {
        StartCoroutine(MainMenuSequence());
    }

    IEnumerator RestartSequence()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay != null)
        {
            pauseOverlay.raycastTarget = true;
            Color c = pauseOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.4f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                pauseOverlay.color = c;
                yield return null;
            }
        }

        Time.timeScale = 1f;
        yield return null;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.ReloadCurrentScene();
        else
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator MainMenuSequence()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay != null)
        {
            pauseOverlay.raycastTarget = true;
            Color c = pauseOverlay.color;
            float elapsed = 0f;
            while (elapsed < 0.4f)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                pauseOverlay.color = c;
                yield return null;
            }
        }

        Time.timeScale = 1f;
        yield return null;

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("MainMenuScene");
        else
            SceneManager.LoadScene("MainMenuScene");
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) +
            c1 * Mathf.Pow(t - 1f, 2f);
    }
}
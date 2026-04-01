using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // This runs EVERY time any scene loads
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always force reset timeScale
        Time.timeScale = 1f;
        Debug.Log("Scene loaded: " + scene.name +
            " — TimeScale reset to 1");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Load any scene safely
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSequence(sceneName));
    }

    public void ReloadCurrentScene()
    {
        StartCoroutine(LoadSequence(
            SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadSequence(string sceneName)
    {
        // Force reset before loading
        Time.timeScale = 1f;
        yield return null;
        yield return null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
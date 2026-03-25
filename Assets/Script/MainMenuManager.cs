using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("FateBound");
    }

    public void OpenOptions()
    {
        Debug.Log("Options Opened");
    }

    public void ExitGame()
    {
        Debug.Log("Game is Exiting...");
        Application.Quit();
    }

    private bool isMuted = false;
    public void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
        Debug.Log("Sound: " + (isMuted ? "Muted" : "On"));
    }
}
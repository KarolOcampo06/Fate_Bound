using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip buttonClickSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip transitionSFX;
    public AudioClip logoAppearSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private bool isMuted = false;
    private bool isTransitioning = false;

    void Awake()
    {
        // CRITICAL — always reset timeScale on Main Menu
        Time.timeScale = 1f;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        // Double safety reset
        Time.timeScale = 1f;

        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        PlaySFX(logoAppearSFX);
    }

    void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip,
            sfxVolume * volumeScale);
    }

    public void OnButtonHover()
    {
        if (isMuted) return;
        PlaySFX(buttonHoverSFX, 0.7f);
    }

    public void OnButtonClick()
    {
        if (isMuted) return;
        PlaySFX(buttonClickSFX);
    }

    public void PlayGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        OnButtonClick();
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        PlaySFX(transitionSFX);

        MainMenuVFX vfx = FindObjectOfType<MainMenuVFX>();
        if (vfx != null)
            yield return StartCoroutine(vfx.FadeOut());
        else
            yield return new WaitForSeconds(0.6f);

        float elapsed = 0f;
        float startVolume = bgmSource.volume;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume =
                Mathf.Lerp(startVolume, 0f, elapsed / 0.3f);
            yield return null;
        }

        bgmSource.Stop();

        // Safety reset before loading game
        Time.timeScale = 1f;
        SceneManager.LoadScene("FateBound");
    }

    public void OpenOptions()
    {
        OnButtonClick();
        Debug.Log("Options Opened");
    }

    public void ExitGame()
    {
        OnButtonClick();
        StartCoroutine(ExitWithSound());
    }

    IEnumerator ExitWithSound()
    {
        yield return new WaitForSeconds(0.3f);
        Application.Quit();
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
        bgmSource.mute = isMuted;
    }
}
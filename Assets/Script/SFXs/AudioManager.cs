using UnityEngine;
using System.Collections;
using Debug = UnityEngine.Debug;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    private AudioSource audioSource;

    [Header("Card SFX")]
    public AudioClip cardClickSFX;
    public AudioClip cardDealSFX;
    public AudioClip cardFlySFX;
    public AudioClip cardDrawSFX;

    [Header("Special Card SFX")]
    public AudioClip blockSFX;
    public AudioClip reverseSFX;
    public AudioClip drawTwoSFX;
    public AudioClip drawFourSFX;
    public AudioClip rollDiceSFX;

    [Header("Game Result SFX")]
    public AudioClip winSFX;
    public AudioClip loseSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DO NOT use DontDestroyOnLoad here
            // AudioManager stays in the scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip,
            masterVolume * sfxVolume * volumeScale);
    }

    void PlaySFXWithDuration(AudioClip clip, float duration)
    {
        if (clip == null) return;
        StartCoroutine(PlayAndStop(clip, duration));
    }

    // Card SFX
    public void PlayCardClick()
    {
        PlaySFX(cardClickSFX);
    }

    public void PlayCardDeal()
    {
        PlaySFX(cardDealSFX, 0.8f);
    }

    public void PlayCardFly()
    {
        PlaySFXWithDuration(cardFlySFX, 0.4f);
    }

    public void PlayCardDraw()
    {
        PlaySFX(cardDrawSFX);
    }

    public void PlayRollDice()
    {
        PlaySFXWithDuration(rollDiceSFX, 2f);
    }

    IEnumerator PlayAndStop(AudioClip clip, float duration)
    {
        audioSource.PlayOneShot(clip,
            masterVolume * sfxVolume);
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
    }

    public void PlaySpecialCardSFX(CardType type)
    {
        switch (type)
        {
            case CardType.Block:
                PlaySFX(blockSFX);
                break;
            case CardType.Reverse:
                PlaySFX(reverseSFX);
                break;
            case CardType.DrawTwo:
                PlaySFX(drawTwoSFX);
                break;
            case CardType.DrawFour:
                PlaySFX(drawFourSFX);
                break;
            case CardType.RollDice:
                // Use duration to stop it after 2 seconds
                PlaySFXWithDuration(rollDiceSFX, 2f);
                break;
        }
    }

    // Game Result SFX
    public void PlayWin()
    {
        PlaySFX(winSFX);
    }

    public void PlayLose()
    {
        PlaySFX(loseSFX);
    }
}
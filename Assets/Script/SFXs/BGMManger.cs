using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // DO NOT use DontDestroyOnLoad
        // Music stays in this scene only
    }
}
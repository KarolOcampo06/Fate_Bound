using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty selectedDifficulty = Difficulty.Medium;

    void Awake()
    {
        // Keep this alive between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
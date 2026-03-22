using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool isPlayerTurn = true;
    public int playerHandCount = 7;
    public int opponentHandCount = 7;

    [Header("UI References")]
    public Transform playerHandArea;
    public Transform opponentHandArea;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        UnityEngine.Debug.Log("Game Started!");
        UnityEngine.Debug.Log("Player's turn: " + isPlayerTurn);
    }

    public void PlayCard(GameObject card)
    {
        if (!isPlayerTurn)
        {
            UnityEngine.Debug.Log("Not your turn!");
            return;
        }

        UnityEngine.Debug.Log("Playing card: " + card.name);

        Destroy(card);
        playerHandCount--;

        EndTurn();
    }

    void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            UnityEngine.Debug.Log("Your turn!");
        }
        else
        {
            UnityEngine.Debug.Log("Opponent's turn!");
            Invoke("SimulateOpponentTurn", 1f);
        }
    }

    void SimulateOpponentTurn()
    {
        UnityEngine.Debug.Log("Opponent played a card");
        EndTurn();
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
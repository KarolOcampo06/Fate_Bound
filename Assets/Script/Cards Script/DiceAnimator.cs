using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Debug = UnityEngine.Debug;

public class DiceAnimator : MonoBehaviour
{
    public static DiceAnimator Instance;

    [Header("UI References")]
    public GameObject dicePanel;
    public Image diceImage;
    public Text resultText;

    [Header("Dice Sprites")]
    public Sprite[] diceFaces; // 6 sprites for dice faces 1-6

    [Header("Animation Settings")]
    public float rollDuration = 2f;
    public float rollSpeed = 0.1f;

    private System.Action<int> onRollComplete;

    void Awake()
    {
        Instance = this;
        if (dicePanel != null)
            dicePanel.SetActive(false);
    }

    public void RollDice(System.Action<int> callback)
    {
        onRollComplete = callback;
        if (dicePanel != null)
            dicePanel.SetActive(true);
        StartCoroutine(AnimateDice());
    }

    IEnumerator AnimateDice()
    {
        float elapsed = 0f;
        int finalResult = Random.Range(1, 7);

        // Animate dice rolling
        while (elapsed < rollDuration)
        {
            int randomFace = Random.Range(0, 6);
            if (diceImage != null && diceFaces != null
                && diceFaces.Length > 0)
            {
                diceImage.sprite = diceFaces[randomFace];
            }

            // Show number on dice image
            if (resultText != null)
                resultText.text = (randomFace + 1).ToString();

            elapsed += rollSpeed;
            yield return new WaitForSeconds(rollSpeed);
        }

        // Show final result
        if (diceImage != null && diceFaces != null
            && diceFaces.Length > 0)
        {
            diceImage.sprite = diceFaces[finalResult - 1];
        }

        if (resultText != null)
            resultText.text = finalResult.ToString();

        Debug.Log("Dice result: " + finalResult);

        // Wait a moment then hide
        yield return new WaitForSeconds(1.5f);

        if (dicePanel != null)
            dicePanel.SetActive(false);

        // Send result back to GameManager
        onRollComplete?.Invoke(finalResult);
    }
}
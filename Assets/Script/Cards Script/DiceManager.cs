using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("UI References")]
    public GameObject dicePanel;
    public Image diceImage;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    public float rollDuration = 2f;
    public float rollSpeed = 0.08f;

    private Sprite[] diceFaceSprites;
    private System.Action<int> onRollComplete;

    void Awake()
    {
        Instance = this;
        GenerateDiceSprites();
        if (dicePanel != null)
            dicePanel.SetActive(false);
    }

    void GenerateDiceSprites()
    {
        diceFaceSprites = new Sprite[6];
        for (int i = 0; i < 6; i++)
        {
            Texture2D tex = DiceFaceCreator.CreateDiceFace(i + 1);
            diceFaceSprites[i] = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        Debug.Log("Dice sprites generated!");
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

        if (descriptionText != null)
            descriptionText.text = "Rolling...";

        while (elapsed < rollDuration)
        {
            int randomFace = Random.Range(0, 6);
            if (diceImage != null && diceFaceSprites != null)
                diceImage.sprite = diceFaceSprites[randomFace];

            if (resultText != null)
                resultText.text = (randomFace + 1).ToString();

            elapsed += rollSpeed;
            yield return new WaitForSeconds(rollSpeed);
        }

        if (diceImage != null && diceFaceSprites != null)
            diceImage.sprite = diceFaceSprites[finalResult - 1];

        if (resultText != null)
            resultText.text = finalResult.ToString();

        if (descriptionText != null)
            descriptionText.text = "Opponent draws "
                + finalResult + " cards!";

        Debug.Log("Final dice result: " + finalResult);

        yield return new WaitForSeconds(2f);

        if (dicePanel != null)
            dicePanel.SetActive(false);

        onRollComplete?.Invoke(finalResult);
    }
}
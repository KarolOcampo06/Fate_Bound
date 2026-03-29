using UnityEngine;
using UnityEngine.UI;

public class TitleGlow : MonoBehaviour
{
    private Image titleImage;
    public float glowSpeed = 2f;

    private Color goldBright = new Color(1f, 0.9f, 0.2f, 1f);
    private Color goldDim = new Color(0.8f, 0.6f, 0.1f, 1f);

    void Start()
    {
        titleImage = GetComponent<Image>();
    }

    void Update()
    {
        float lerp = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f;
        titleImage.color = Color.Lerp(goldDim, goldBright, lerp);
    }
}
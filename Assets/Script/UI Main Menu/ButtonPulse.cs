using UnityEngine;
using UnityEngine.UI;

public class ButtonPulse : MonoBehaviour
{
    private Image buttonImage;
    public float pulseSpeed = 1.5f;
    public float minAlpha = 0.7f;
    public float maxAlpha = 1f;

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        Color c = buttonImage.color;
        c.a = alpha;
        buttonImage.color = c;
    }
}
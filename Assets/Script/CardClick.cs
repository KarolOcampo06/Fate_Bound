using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Selection Visual")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private UnityEngine.UI.Image cardImage;
    private bool isSelected = false;

    void Start()
    {
        cardImage = GetComponent<UnityEngine.UI.Image>();
        if (cardImage != null)
        {
            cardImage.color = normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelection();
    }

    void ToggleSelection()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            SelectCard();
        }
        else
        {
            DeselectCard();
        }
    }

    void SelectCard()
    {
        if (cardImage != null)
        {
            cardImage.color = selectedColor;
        }
        UnityEngine.Debug.Log("Card selected: " + gameObject.name);
    }

    void DeselectCard()
    {
        if (cardImage != null)
        {
            cardImage.color = normalColor;
        }
        UnityEngine.Debug.Log("Card deselected: " + gameObject.name);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Deselect()
    {
        isSelected = false;
        DeselectCard();
    }

    public void PlayThisCard()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayCard(gameObject);
        }
    }
}

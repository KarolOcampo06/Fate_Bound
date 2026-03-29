using UnityEngine;

public class CardSpriteManager : MonoBehaviour
{
    public static CardSpriteManager Instance;

    [Header("Red/Chaos Number Cards")]
    public Sprite[] chaosCards;

    [Header("Gold/Destiny Number Cards")]
    public Sprite[] destinyCards;

    [Header("Blue/Fortune Number Cards")]
    public Sprite[] fortuneCards;

    [Header("Purple/Shadow Number Cards")]
    public Sprite[] shadowCards;

    [Header("Red/Chaos Special Cards")]
    public Sprite chaosBlock;
    public Sprite chaosReverse;
    public Sprite chaosDrawTwo;
    public Sprite chaosDrawFour;
    public Sprite chaosRollDice;

    [Header("Gold/Destiny Special Cards")]
    public Sprite destinyBlock;
    public Sprite destinyReverse;
    public Sprite destinyDrawTwo;
    public Sprite destinyDrawFour;
    public Sprite destinyRollDice;

    [Header("Blue/Fortune Special Cards")]
    public Sprite fortuneBlock;
    public Sprite fortuneReverse;
    public Sprite fortuneDrawTwo;
    public Sprite fortuneDrawFour;
    public Sprite fortuneRollDice;

    [Header("Purple/Shadow Special Cards")]
    public Sprite shadowBlock;
    public Sprite shadowReverse;
    public Sprite shadowDrawTwo;
    public Sprite shadowDrawFour;
    public Sprite shadowRollDice;

    void Awake()
    {
        Instance = this;
    }

    public Sprite GetSprite(CardColor color, CardType type, int number = 0)
    {
        // Number cards
        if (type == CardType.Number)
        {
            if (number <= 0) return null;

            Sprite[] targetArray = null;

            switch (color)
            {
                case CardColor.Red:
                    targetArray = chaosCards;
                    break;
                case CardColor.Gold:
                    targetArray = destinyCards;
                    break;
                case CardColor.Blue:
                    targetArray = fortuneCards;
                    break;
                case CardColor.Purple:
                    targetArray = shadowCards;
                    break;
            }

            if (targetArray != null &&
                targetArray.Length > 0 &&
                number - 1 >= 0 &&
                number - 1 < targetArray.Length)
            {
                return targetArray[number - 1];
            }

            return null;
        }

        // Special cards
        switch (color)
        {
            case CardColor.Red:
                return GetChaosSpecial(type);
            case CardColor.Gold:
                return GetDestinySpecial(type);
            case CardColor.Blue:
                return GetFortuneSpecial(type);
            case CardColor.Purple:
                return GetShadowSpecial(type);
        }

        return null;
    }

    Sprite GetChaosSpecial(CardType type)
    {
        switch (type)
        {
            case CardType.Block: return chaosBlock;
            case CardType.Reverse: return chaosReverse;
            case CardType.DrawTwo: return chaosDrawTwo;
            case CardType.DrawFour: return chaosDrawFour;
            case CardType.RollDice: return chaosRollDice;
        }
        return null;
    }

    Sprite GetDestinySpecial(CardType type)
    {
        switch (type)
        {
            case CardType.Block: return destinyBlock;
            case CardType.Reverse: return destinyReverse;
            case CardType.DrawTwo: return destinyDrawTwo;
            case CardType.DrawFour: return destinyDrawFour;
            case CardType.RollDice: return destinyRollDice;
        }
        return null;
    }

    Sprite GetFortuneSpecial(CardType type)
    {
        switch (type)
        {
            case CardType.Block: return fortuneBlock;
            case CardType.Reverse: return fortuneReverse;
            case CardType.DrawTwo: return fortuneDrawTwo;
            case CardType.DrawFour: return fortuneDrawFour;
            case CardType.RollDice: return fortuneRollDice;
        }
        return null;
    }

    Sprite GetShadowSpecial(CardType type)
    {
        switch (type)
        {
            case CardType.Block: return shadowBlock;
            case CardType.Reverse: return shadowReverse;
            case CardType.DrawTwo: return shadowDrawTwo;
            case CardType.DrawFour: return shadowDrawFour;
            case CardType.RollDice: return shadowRollDice;
        }
        return null;
    }
}
// CardEnums.cs
// This file defines all the possible card colors and types

// All possible card colors in Fate Bound
public enum CardColor
{
    Red,      // Chaos
    Gold,     // Destiny
    Blue,     // Fortune
    Purple    // Shadow
}

// All possible card types in Fate Bound
public enum CardType
{
    Number,     // Regular numbered cards (1-9)
    Block,      // Block card
    DrawTwo,    // Draw 2 cards
    DrawFour,   // Draw 4 cards
    Reverse,    // Reverse direction
    RollDice    // Roll the dice card
}
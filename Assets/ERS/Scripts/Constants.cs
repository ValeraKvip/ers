using System.ComponentModel;

public enum Suit
{
    [Description("spades")] Spades,
    [Description("clubs")] Clubs,
    [Description("diamonds")] Diamonds,
    [Description("hearts")] Hearts,
}

public enum Rank
{
    [Description("A")] Ace,
    [Description("2")] Two,
    [Description("3")] Three,
    [Description("4")] Four,
    [Description("5")] Five,
    [Description("6")] Six,
    [Description("7")] Seven,
    [Description("8")] Eight,
    [Description("9")] Nine,
    [Description("10")] Ten,
    [Description("J")] Jack,
    [Description("Q")] Queen,
    [Description("K")] King,
}

public enum SlapCombination
{
    [Description("Ouch!")] None,
    [Description("Double")] Double,
    [Description("Marriage")] Marriage,
    [Description("Sandwich")] Sandwich,
    [Description("Divorce")] Divorce,
    [Description("Three in a Row")] ThreeInRow,
    [Description("Cheat win!")] CheatWin,
}
public struct CardValue
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public CardValue(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }
}
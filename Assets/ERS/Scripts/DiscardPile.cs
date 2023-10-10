using System.Collections.Generic;

public class DiscardPile
{
    private readonly List<CardValue> cards = new();

    public int CardCount => cards.Count;
    
    public void AddCard(CardValue card)
    {
        cards.Add(card);
    }

    public CardValue GetTopCard()
    {
        var card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }

    public SlapCombination GetSlapCombination()
    {
        if (cards.Count >= 2)
        {
            Rank r1 = cards[^1].Rank;
            Rank r2 = cards[^2].Rank;

            // Double
            if (r1 == r2)
                return SlapCombination.Double;

            // Marriage
            if ((r1 == Rank.King && r2 == Rank.Queen) || (r1 == Rank.Queen && r2 == Rank.King))
                return SlapCombination.Marriage;
        }

        if (cards.Count >= 3)
        {
            Rank r1 = cards[^1].Rank;
            Rank r2 = cards[^2].Rank;
            Rank r3 = cards[^3].Rank;

            // Sandwich
            if (r1 == r3)
                return SlapCombination.Sandwich;

            // Divorce
            if ((r1 == Rank.King && r3 == Rank.Queen) || (r1 == Rank.Queen && r3 == Rank.King))
                return SlapCombination.Divorce;

            // Three in a row
            if (r2 == r1.Next() && r3 == r2.Next())
                return SlapCombination.ThreeInRow;
            if (r2 == r1.Previous() && r3 == r2.Previous())
                return SlapCombination.ThreeInRow;

        }

        return SlapCombination.None;
    }
}
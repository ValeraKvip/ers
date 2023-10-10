using System.Collections.Generic;

public class Player
{
    private readonly Queue<CardValue> cards = new();
    public int CardCount => cards.Count;

    public void AddCard(CardValue card)
    {
        cards.Enqueue(card);
    }

    public CardValue GetCard()
    {
        return cards.Dequeue();
    }
}
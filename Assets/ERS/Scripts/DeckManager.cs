using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class DeckManager 
{
    public static void DealCards(Player player1, Player player2)
    {
        List<CardValue> cards = new();
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                cards.Add(new CardValue(suit, rank));

        for (int n = cards.Count - 1; n > 0; --n)
        {
            int k = Random.Range(0, n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }

        bool flag = true;
        foreach (var card in cards)
        {
            if (flag)
                player1.AddCard(card);
            else
                player2.AddCard(card);
            flag = !flag;
        }
    }

    public static bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return true;

        return false;
    }
}

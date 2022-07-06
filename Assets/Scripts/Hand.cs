using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cards = new();

    public void Sort()
    {
        cards.Sort((a, b) =>
        {
            if (a.Rank != b.Rank)
            {
                return a.Rank < b.Rank ? 1 : -1;
            }

            return (int)a.Suit > (int)b.Suit ? 1 : -1;
        });
    }

    public void Add(Card card)
    {
        cards.Add(card);
    }

    public void Clear()
    {
        foreach (Card c in cards)
        {
            c.Destroy();
        }

        cards.Clear();
    }
}

using UnityEngine;

public class Deck : Hand
{
    public void Populate()
    {
        Clear();
        CreateCards();
    }

    private void CreateCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                Card card = Card.CreateInstance(j, (Suit)i, new Vector3(0f, 0f, 0f));

                Add(card);
            }
        }
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            (cards[j], cards[i]) = (cards[i], cards[j]);
            Vector3 position = cards[i].cardObject.transform.position;
            cards[i].cardObject.transform.position = new Vector3(position.x, position.y, -i);
        }
    }

    /* public void Deal(Player hand)
     {
         if (cards.Count != 0)
         {
             Card top = cards[0];
             cards.Remove(top);
             hand.Add(top);
         }

     }*/
}

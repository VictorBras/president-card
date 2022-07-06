using System.Collections.Generic;
using System.Threading.Tasks;

public enum Title
{
    President = 1,
    vicePresident,
    viceScum,
    Scum
}


public abstract class Player : Hand
{
    public int Score { get; set; } = 0;
    public bool Passed { get; set; } = false;
    public Title Position { get; set; }

    public abstract Task<List<Card>> Turn(int currentRank = 0, int countCards = 0);

    public List<Card> GetAllowedCards(int currentRank, int countCards)
    {
        List<Card> allowedCards = new();

        for (int i = 0; i < cards.Count; i++)
        {
            Card currentCard = cards[i];

            if (currentCard.Rank < currentRank)
            {
                break;
            }

            int lastIndex = cards.FindLastIndex(card => card.Rank == currentCard.Rank);
            int countSameRank = 1 + lastIndex - i;

            if (countSameRank >= countCards)
            {
                allowedCards.AddRange(cards.GetRange(i, countSameRank));
            }

            i = lastIndex;
        }


        return allowedCards;
    }
}

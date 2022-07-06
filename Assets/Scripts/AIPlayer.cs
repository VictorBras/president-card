using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AIPlayer : Player
{
    public override Task<List<Card>> Turn(int currentRank = 0, int countCards = 0)
    {
        List<Card> allowedCards = GetAllowedCards(currentRank, countCards);
        List<Card> cardsToPlay = GetCardsToPlay(allowedCards, countCards);

        TaskCompletionSource<List<Card>> taskPlayCards = new TaskCompletionSource<List<Card>>();

        StartCoroutine(AwaitToPlayCards(taskPlayCards, cardsToPlay));

        return taskPlayCards.Task;
    }

    private IEnumerator AwaitToPlayCards(TaskCompletionSource<List<Card>> taskPlayCards, List<Card> cardsToPlay)
    {
        yield return new WaitForSeconds(1f);

        taskPlayCards.SetResult(cardsToPlay);
    }

    private List<Card> GetCardsToPlay(List<Card> allowedCards, int countCards)
    {
        int maxCountSameRank = 0;
        List<Card> cardsToPlay = new();

        for (int i = 0; i < allowedCards.Count; i++)
        {
            int lastIndex = allowedCards.FindLastIndex(card => card.Rank == allowedCards[i].Rank);
            int countSameRank = 1 + lastIndex - i;

            if (countSameRank >= maxCountSameRank && (countSameRank == countCards || countCards == 0))
            {
                maxCountSameRank = countSameRank;
                cardsToPlay = allowedCards.GetRange(i, countSameRank);
            }

            i = lastIndex;
        }

        return cardsToPlay;
    }

    private TextMeshProUGUI GetCountCardsTextObject()
    {
        return gameObject.transform.Find("CountCards").GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        UpdateCountCardsText();
        UpdatePassedText();
        UpdateTitleText();

    }

    private void UpdateTitleText()
    {
        TextMeshProUGUI titleText = gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>();

        if (cards.Count == 0 && Position > 0)
        {
            string title = "";

            switch (Position)
            {
                case Title.President:
                    title = "Presidente";
                    break;
                case Title.vicePresident:
                    title = "Vice-Presidente";
                    break;
                case Title.viceScum:
                    title = "Vice-Escória";
                    break;
                case Title.Scum:
                    title = "Escória";
                    break;
            }

            titleText.text = title;
            titleText.gameObject.SetActive(true);
        } else
        {
            titleText.gameObject.SetActive(false);
        }
    }

    private void UpdatePassedText()
    {
        TextMeshProUGUI passedText = gameObject.transform.Find("Pass").GetComponent<TextMeshProUGUI>();

        passedText.gameObject.SetActive(Passed);
    }

    private void UpdateCountCardsText()
    {
        TextMeshProUGUI countCardsText = GetCountCardsTextObject();

        if (countCardsText)
        {
            countCardsText.text = string.Format("({0})", cards.Count.ToString());
        }
    }
}
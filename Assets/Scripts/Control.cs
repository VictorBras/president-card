using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public List<Player> players = new();
    public GameObject cardsPlayedContainer;

    private Deck deck = new();
    private int indexPlayerTurn = 0;
    private bool started = false;
    private int currentRank = 0;
    private int currentCount = 0;
    private int roundCount = 0;
    private int lastPlayerToPlayIndex;
    private int countBeatPlayers = 0;
    private bool awaitPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        started = false;
        deck.Populate();
        yield return new WaitForSeconds(0.5f);

        deck.Shuffle();
        Deal();

        ResetRoundValues();
        ResetTitlePlayers();
        ClearPlayCardsContainer();

        started = true;
    }

    private void ResetTitlePlayers()
    {
        players.ForEach(player => player.Position = (Title)0);
    }

    private void ResetRoundValues()
    {
        indexPlayerTurn = 0;
        currentRank = 0;
        currentCount = 0;
        roundCount = 0;
        countBeatPlayers = 0;
    }

    private void Deal()
    {
        int playerIndex = 0;

        while (deck.cards.Count > 0)
        {
            Card currentCard = deck.cards[0];

            players[playerIndex].Add(currentCard);
            deck.cards.RemoveAt(0);

            playerIndex = (playerIndex + 1) % 4;
        }

        players.ForEach(player => player.Sort());
    }

    private void Update()
    {
        Turn();
    }

    private async void Turn()
    {
        if (!CanTurn())
        {
            return;
        }

        awaitPlayer = true;
        Player playerOfTurn = GetPlayerOfTurn();
        List<Card> cardsPlayed = await playerOfTurn.Turn(currentRank, currentCount);

        if (cardsPlayed.Count > 0)
        {
            UpdateRoundData(cardsPlayed);
            RemoveCardsOfPlayer(playerOfTurn, cardsPlayed);
            DisplayPlayedCards(cardsPlayed);

            if (playerOfTurn.cards.Count == 0)
            {
                countBeatPlayers++;
                playerOfTurn.Score = 4 - countBeatPlayers;
                playerOfTurn.Position = (Title)countBeatPlayers;

                if (countBeatPlayers >= 3)
                {
                    StartCoroutine(InitGame());
                }
            }

            if (cardsPlayed[0].Number == 2)
            {
                StartCoroutine(AwaitToStartNewRound());

                return;
            }
        }
        else
        {
            playerOfTurn.Passed = true;
        }

        UpdateIndexPlayerOfTurn();

        if (indexPlayerTurn == lastPlayerToPlayIndex)
        {
            StartNewRound();

            return;
        }

        roundCount++;
        awaitPlayer = false;
    }

    private IEnumerator AwaitToStartNewRound()
    {

        yield return new WaitForSeconds(1f);
        StartNewRound();
    }

    private Player GetPlayerOfTurn()
    {
        Player playerOfTurn = players[indexPlayerTurn];

        while (playerOfTurn.cards.Count == 0)
        {
            UpdateIndexPlayerOfTurn();

            return GetPlayerOfTurn();
        }

        return playerOfTurn;
    }

    private void UpdateIndexPlayerOfTurn()
    {
        indexPlayerTurn = (indexPlayerTurn + 1) % 4;
    }

    private void UpdateRoundData(List<Card> cardsPlayed)
    {
        currentCount = cardsPlayed.Count;
        currentRank = cardsPlayed[0].Rank;
        lastPlayerToPlayIndex = indexPlayerTurn;
    }

    private static void RemoveCardsOfPlayer(Player playerOfTurn, List<Card> cardsPlayed)
    {
        cardsPlayed.ForEach(card => playerOfTurn.cards.Remove(card));
    }

    private void StartNewRound()
    {
        roundCount = 0;
        currentCount = 0;
        currentRank = 0;
        lastPlayerToPlayIndex = indexPlayerTurn;

        ClearPlayCardsContainer();

        players.ForEach(player => player.Passed = false);

        awaitPlayer = false;
    }

    private void ClearPlayCardsContainer()
    {
        foreach (Transform child in cardsPlayedContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayPlayedCards(List<Card> cardsPlayed)
    {

        cardsPlayed.ForEach(card =>
        {
            bool evenCountCards = currentCount % 2 == 0;
            int initialX = (evenCountCards ? 0 : 50) + (-50 * currentCount);
            int indexCard = cardsPlayed.IndexOf(card);

            float x = initialX + (100 * indexCard);
            float z = (indexCard + 1) * (roundCount + 1) * -1;

            card.cardObject.transform.SetParent(cardsPlayedContainer.transform);
            card.UpdatePosition(new Vector3(x, 0, z));
            card.cardObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
            card.cardObject.transform.localScale = new Vector3(40f, 40f, 40f);
        });
    }

    private bool CanTurn()
    {
        return started && !awaitPlayer;
    }
}

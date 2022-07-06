using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : Player
{
    private bool isTurn = false;
    private Transform cardsContainerTransform;
    private List<Card> selectedCards = new();
    private Button playButton;
    private Button passButton;

    private void Start()
    {
        Transform handPlayerTransform = GameObject.Find("HandPlayer").transform;
        cardsContainerTransform = handPlayerTransform.Find("CardsContainer").transform;
        playButton = handPlayerTransform.Find("Play").GetComponent<Button>();
        passButton = handPlayerTransform.Find("Pass").GetComponent<Button>();
    }

    public override Task<List<Card>> Turn(int currentRank = 0, int countCards = 0)
    {
        List<Card> allowedCards = GetAllowedCards(currentRank, countCards);
        TaskCompletionSource<List<Card>> taskReturn = new TaskCompletionSource<List<Card>>();
        isTurn = true;

        if (allowedCards.Count > 0)
        {
            UpdatePositionAllowedCards(allowedCards);
            AddListenerAllowedCards(allowedCards, countCards, taskReturn);
            AddListenerPassButton(taskReturn);
        }
        else
        {
            PlayCards(taskReturn, allowedCards);
        }


        return taskReturn.Task;
    }

    private void AddListenerPassButton(TaskCompletionSource<List<Card>> taskReturn)
    {
        passButton.gameObject.SetActive(true);
        passButton.onClick.AddListener(() =>
        {
            PlayCards(taskReturn, new List<Card>());
        });
    }

    private void AddListenerAllowedCards(List<Card> allowedCards, int countCards, TaskCompletionSource<List<Card>> taskReturn)
    {
        allowedCards.ForEach(card =>
        {
            card.OnClick.AddListener(() =>
            {
                selectedCards.Add(card);

                if (countCards > 0)
                {
                    if (countCards == selectedCards.Count)
                    {
                        PlayCards(taskReturn, selectedCards);

                        return;
                    }

                    List<Card> cardWithSameNumber = allowedCards.FindAll(c => c.Number == card.Number);

                    PlayCards(taskReturn, cardWithSameNumber.GetRange(0, countCards));
                }
                else
                {
                    List<Card> cardWithSameNumber = allowedCards.FindAll(c => c.Number == card.Number);

                    allowedCards.FindAll(c => c.Number != card.Number).ForEach(card =>
                    {
                        card.OnClick.RemoveAllListeners();
                    });

                    if (selectedCards.Count == cardWithSameNumber.Count)
                    {
                        PlayCards(taskReturn, cardWithSameNumber);
                    }
                    else
                    {
                        Vector3 cardPosition = card.Position;
                        card.UpdatePosition(new Vector3(cardPosition.x, 100, cardPosition.z));
                        card.OnClick.RemoveAllListeners();

                        playButton.gameObject.SetActive(true);
                        playButton.onClick.RemoveAllListeners();

                        playButton.onClick.AddListener(() =>
                        {
                            PlayCards(taskReturn, selectedCards);
                        });
                    }
                }


            });
        });
    }

    private void PlayCards(TaskCompletionSource<List<Card>> taskReturn, List<Card> cardsToPlay)
    {
        taskReturn.SetResult(cardsToPlay);
        selectedCards.Clear();
        isTurn = false;
        HideActionsButtons();
        RemoveListenerAllCards();
        RemoveListenerPlayButton();
        RemoveListenerPassButton();
    }

    private void HideActionsButtons()
    {
        playButton.gameObject.SetActive(false);
        passButton.gameObject.SetActive(false);
    }

    private void RemoveListenerPlayButton()
    {
        playButton.onClick.RemoveAllListeners();
    }

    private void RemoveListenerPassButton()
    {
        passButton.onClick.RemoveAllListeners();
    }

    private void RemoveListenerAllCards()
    {
        cards.ForEach(c => c.OnClick.RemoveAllListeners());
    }

    void FixedUpdate()
    {
        if (!isTurn)
        {
            UpdatePositionAllCards();
        }
    }

    private void UpdatePositionAllowedCards(List<Card> allowedCards)
    {
        allowedCards.ForEach(card => UpdatePositionCard(card, true));
    }

    private void UpdatePositionAllCards()
    {
        cards.ForEach(card => UpdatePositionCard(card));
    }

    private void UpdatePositionCard(Card card, bool allowedToPlay = false)
    {
        bool evenCountCards = cards.Count % 2 == 0;
        int initialX = (evenCountCards ? 0 : 50) + (-50 * cards.Count);
        int indexCard = cards.IndexOf(card);

        float x = initialX + (100 * indexCard);
        float y = allowedToPlay ? 70 : 40;
        float z = (indexCard + 1) * -1;

        card.cardObject.transform.SetParent(cardsContainerTransform);
        card.UpdatePosition(new Vector3(x, y, z));
        card.cardObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
        card.cardObject.transform.localScale = new Vector3(40f, 40f, 40f);
    }
}

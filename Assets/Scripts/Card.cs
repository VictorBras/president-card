using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum Suit
{
    Heart,
    Diamond,
    Club,
    Spade,
    None
}

public enum CardNumber
{
    Ace = 1,
    Jack = 11,
    Queen,
    King
}

public enum Face
{
    Up,
    Down
}

public class Card : MonoBehaviour
{
    public int Number { get; set; }
    public Suit Suit { get; set; }
    public Vector3 Position { get; private set; }
    public int Rank { get; private set; }
    public Face Face { get; private set; }

    public UnityEvent OnClick = new UnityEvent();
    public GameObject cardObject { get; private set; }

    public static Card CreateInstance(int number, Suit suit, Vector3 vector3)
    {
        GameObject gameObject = new GameObject("Card");
        Card card = gameObject.AddComponent<Card>();

        card.Number = number;
        card.Suit = suit;
        card.Position = vector3;

        return card;
    }

    void Start()
    {
        InstantiateCard();
        SetRank();

        Face = Face.Down;
    }

    private void SetRank()
    {
        switch (Number)
        {
            case 1:
                Rank = 13;
                break;
            case 2:
                Rank = 14;
                break;
            default:
                Rank = Number - 1;
                break;
        }
    }

    private void InstantiateCard()
    {
        cardObject = Instantiate(loadPrefabFromAssets(), Position, Quaternion.Euler(90f, 15f, 0));
        cardObject.transform.localScale = new Vector3(0, 0, 0);
    }

    private GameObject loadPrefabFromAssets()
    {
        GameObject objPrefab = Resources.Load(GetPrefabPath()) as GameObject;

        return objPrefab;
    }

    private string GetPrefabPath()
    {
        string prefabCardBasePath = "Playing_Cards/Resource/Prefab/BackColor_Blue/Blue_PlayingCards_";
        string extension = "_00";
        string cardNumber = Number.ToString("D2");
        string suitText = GetSuitText();

        return prefabCardBasePath + suitText + cardNumber + extension;
    }

    private string GetSuitText()
    {
        switch (Suit)
        {
            case Suit.Club:
                return "Club";
            case Suit.Diamond:
                return "Diamond";
            case Suit.Heart:
                return "Heart";
            case Suit.Spade:
                return "Spade";
            default:
                return "";
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        Position = position;

        if (cardObject && cardObject.transform)
        {
            cardObject.transform.localPosition = position;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit rayHit))
            {
                if (rayHit.transform.gameObject == cardObject)
                {
                    OnClick.Invoke();
                }
            }
        }
    }

    public void Flip()
    {
        float xAngle = 90f;
        Face faceCard = Face.Down;

        if (Face == Face.Down)
        {
            xAngle = -90f;
            faceCard = Face.Up;
        }

        Face = faceCard;
        cardObject.transform.rotation = Quaternion.Euler(xAngle, 15f, 0);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public override bool Equals(object obj)
    {
        return obj is Card card &&
               base.Equals(obj) &&
               Number == card.Number &&
               Suit == card.Suit;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Number, Suit);
    }

    public static bool operator ==(Card a, Card b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a == null || b == null)
            return false;

        return a.Number == b.Number && a.Suit == b.Suit;
    }

    public static bool operator !=(Card a, Card b)
    {
        return !(a == b);
    }
}

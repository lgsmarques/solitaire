using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;

public class GameManager : Singleton<GameManager>
{
    [Header("Setups")]
    public SODeckSetup soDeckSetup;
    public SOGameSetup soGameSetup;

    [Header("Start Points")]
    public Transform deckPile;
    public Transform discardPile;
    public List<Transform> suitsPiles = new();
    public List<Transform> cardsColumns = new();

    [Header("Cards In Play")]
    public List<GameObject> cardsInSuitPiles;
    public List<GameObject> cardsShownInColumns;
    public List<GameObject> cardsHiddenInColumns;
    public GameObject cardsInDiscardPile;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Init()
    {
        DeckManager.Instance.Shuffle();
        StartPlaces();
        StartGame();
        DeckManager.Instance.InstantiateDeck(deckPile.position);
    }

    public void StartPlaces()
    {
        deckPile = Instantiate(soGameSetup.startPoints[0]);
        discardPile = Instantiate(soGameSetup.startPoints[1]);

        for (int i = 2; i <= 5; i++) suitsPiles.Add(Instantiate(soGameSetup.startPoints[i]));

        for (int i = 6; i <= 12; i++) cardsColumns.Add(Instantiate(soGameSetup.startPoints[i]));

        InstanceCardPlaceholders();
    }

    public void StartGame()
    {
        for (int i = 0; i < 7; i++)
        {
            GameObject placeholder = Instantiate(soGameSetup.cardPlaceholderColumn, cardsColumns[i].position + soGameSetup.placeholderPlacement, Quaternion.Euler(0f, 0f, 0f));
            placeholder.transform.parent = cardsShownInColumns[i].transform;

            for (int j = 0; j < i; j++)
            {
                DeckManager.Instance.DrawCards(1, cardsColumns[i].position, cardsHiddenInColumns[i], false);
                cardsColumns[i].position = NextPosition(cardsColumns[i].position, soGameSetup.cardColumnIncrease);
            }

            DeckManager.Instance.DrawCards(1, cardsColumns[i].position, cardsShownInColumns[i]);
            cardsColumns[i].position = NextPosition(cardsColumns[i].position, soGameSetup.cardColumnIncrease);
        }
    }

    public void InstanceCardPlaceholders()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject placeholder = Instantiate(soGameSetup.cardPlaceholderSuit, suitsPiles[i].position + soGameSetup.placeholderPlacement, Quaternion.Euler(0f, 0f, 0f));
            placeholder.transform.parent = cardsInSuitPiles[i].transform;
            suitsPiles[i].position = NextPosition(suitsPiles[i].position, soGameSetup.pilesIncrease);
        }
    }

    public void DrawToDiscardPile()
    {
        DeckManager.Instance.DrawCards(1, discardPile.position, cardsInDiscardPile);
        discardPile.position = NextPosition(discardPile.position, soGameSetup.pilesIncrease);
    }

    public Vector3 NextPosition(Vector3 currentPosition, Vector3 increase)
    {
        return currentPosition + increase;
    }

    public void TurnCardFaceUp(int columnIndex)
    {
        int cardsInCollumn = cardsHiddenInColumns[columnIndex].transform.childCount;
        if (cardsInCollumn > 0)
        {
            GameObject lastCard = cardsHiddenInColumns[columnIndex].transform.GetChild(cardsInCollumn - 1).gameObject;
            lastCard.transform.parent = cardsShownInColumns[columnIndex].transform;
            lastCard.GetComponent<Card>().faceUp = true;
            lastCard.transform.GetChild(0).transform.rotation = soDeckSetup.faceUpRotation;
        }
        cardsColumns[columnIndex].position = NextPosition(cardsColumns[columnIndex].position, -soGameSetup.cardColumnIncrease);
    }

    public bool PlaceCard(GameObject card)
    {
        Vector3 direction = new(0, 0, 5);

        if (Physics.Raycast(card.transform.position, direction, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.transform.parent.CompareTag("CardsInPlaySuitPile"))
            {
                if (SuitPilePlacement(card, hitObject))
                {
                    return true;
                }
            }

            if (hitObject.transform.parent.CompareTag("CardsInPlayShownColumn"))
            {
                if (ColumnPlacement(card, hitObject))
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            return false;
        }
    }

    public bool SuitPilePlacement(GameObject card, GameObject hitObject)
    {
        int index = cardsInSuitPiles.IndexOf(hitObject.transform.parent.gameObject);

        GameObject cardInitialParent = card.transform.parent.gameObject;

        Card hitObjectComponent = hitObject.GetComponent<Card>();
        Card cardComponent = card.GetComponent<Card>();

        if (!hitObjectComponent && cardComponent.value != Card.Values.Ace)
        {
            return false;
        }

        if (hitObjectComponent && (hitObjectComponent.value != (cardComponent.value - 1) || hitObjectComponent.suit != cardComponent.suit))
        {
            return false;
        }

        if (cardInitialParent.CompareTag("CardsInPlayShownColumn"))
        {
            int columnIndex = cardsShownInColumns.IndexOf(cardInitialParent);
            TurnCardFaceUp(columnIndex);
        }

        card.transform.parent = hitObject.transform.parent;
        card.transform.position = suitsPiles[index].position;
        suitsPiles[index].position = NextPosition(suitsPiles[index].position, soGameSetup.pilesIncrease);
        return true;
    }

    public bool ColumnPlacement(GameObject card, GameObject hitObject)
    {
        int index = cardsShownInColumns.IndexOf(hitObject.transform.parent.gameObject);

        GameObject cardInitialParent = card.transform.parent.gameObject;

        Card hitObjectComponent = hitObject.GetComponent<Card>();
        Card cardComponent = card.GetComponent<Card>();

        if (!hitObjectComponent && cardComponent.value != Card.Values.King) return false;

        if (hitObjectComponent && (hitObjectComponent.value != (cardComponent.value + 1) || hitObjectComponent.CardColor() == cardComponent.CardColor()))
        {
            return false;
        }

        if (cardInitialParent.CompareTag("CardsInPlayShownColumn"))
        {
            int columnIndex = cardsShownInColumns.IndexOf(cardInitialParent);
            TurnCardFaceUp(columnIndex);
        }

        card.transform.parent = hitObject.transform.parent;
        card.transform.position = cardsColumns[index].position;
        cardsColumns[index].position = NextPosition(cardsColumns[index].position, soGameSetup.cardColumnIncrease);
        return true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class GameManager : Singleton<GameManager>
{
    [Header("Setups")]
    public SODeckSetup soDeckSetup;
    public SOGameSetup soGameSetup;

    private Transform _deckPile;
    private Transform _discardPile;
    private List<Transform> _suitsPiles = new();
    private List<Transform> _cardsColumns = new();

    private List<List<GameObject>> _cardsInSuitPiles = new();
    private List<List<GameObject>> _cardsShownInColumns = new();
    private List<List<GameObject>> _cardsHiddenInColumns = new();
    private List<GameObject> _cardsInDiscardPile = new();

    private void Start()
    {
        Init();
    }

    private void Update()
    {
    }

    private void Init()
    {
        DeckManager.Instance.Shuffle();
        StartPlaces();
        StartGame();
        DeckManager.Instance.InstantiateDeck(_deckPile.position);
    }

    public void StartPlaces()
    {
        _deckPile = Instantiate(soGameSetup.startPoints[0]);
        _discardPile = Instantiate(soGameSetup.startPoints[1]);

        for (int i = 2; i <= 5; i++) _suitsPiles.Add(Instantiate(soGameSetup.startPoints[i]));

        for (int i = 6; i <= 12; i++) _cardsColumns.Add(Instantiate(soGameSetup.startPoints[i]));

        InstanceCardPlaceholders();
    }

    public void StartGame()
    {
        for (int i = 0; i < 7; i++)
        {
            _cardsHiddenInColumns.Add(new List<GameObject>());
            _cardsShownInColumns.Add(new List<GameObject>());

            for (int j = 0; j < i; j++)
            {
                _cardsHiddenInColumns[i].AddRange(DeckManager.Instance.DrawCards(1, _cardsColumns[i].position, false));
                Vector3 nextColumnPosition = _cardsColumns[i].position + soGameSetup.cardColumnIncrease;
                _cardsColumns[i].position = nextColumnPosition;
            }

            _cardsShownInColumns[i].AddRange(DeckManager.Instance.DrawCards(1, _cardsColumns[i].position));
        }
    }

    public void InstanceCardPlaceholders()
    {
        for (int i = 0; i < 4; i++)
        {
            _cardsInSuitPiles.Add(new List<GameObject>());
            _cardsInSuitPiles[i].Add(Instantiate(soGameSetup.cardPlaceholder, _suitsPiles[i].position + soGameSetup.placeholderPlacement, Quaternion.Euler(0f, 0f, 0f)));
        }
    }

    public void DrawToDiscardPile()
    {
        _cardsInDiscardPile.AddRange(DeckManager.Instance.DrawCards(1, _discardPile.position));
        Vector3 nextPosition = _discardPile.position + soGameSetup.discardPileIncrease;
        _discardPile.position = nextPosition;
    }

    public bool PlaceCard(GameObject card)
    {
        Vector3 direction = new Vector3(0, 0, 5);
        RaycastHit hit;

        if (Physics.Raycast(card.transform.position, direction, out hit))
        {
            if (hit.collider.CompareTag("CardPlaceholder"))
            {
                Vector3 newPosition = hit.transform.position + soGameSetup.discardPileIncrease;
                card.transform.position = newPosition;
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}

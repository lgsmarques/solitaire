using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class GameManager : Singleton<GameManager>
{
    [Header("Setups")]
    public SODeckSetup soDeckSetup;
    public SOGameSetup sOGameSetup;

    private Transform _deckPile;
    private Transform _discardPile;
    private List<Transform> _suitsPiles = new();
    private List<Transform> _cardsColumns = new();

    //[SerializeField] private List<List<GameObject>> cardsInSuitPiles = new();
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
        _deckPile = Instantiate(sOGameSetup.startPoints[0]);
        _discardPile = Instantiate(sOGameSetup.startPoints[1]);

        for (int i = 2; i <= 5; i++) _suitsPiles.Add(Instantiate(sOGameSetup.startPoints[i]));

        for (int i = 6; i <= 12; i++) _cardsColumns.Add(Instantiate(sOGameSetup.startPoints[i]));

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
                Vector3 nextColumnPosition = _cardsColumns[i].position + sOGameSetup.cardColumnIncrease;
                _cardsColumns[i].position = nextColumnPosition;
            }

            _cardsShownInColumns[i].AddRange(DeckManager.Instance.DrawCards(1, _cardsColumns[i].position));
        }
    }

    public void InstanceCardPlaceholders()
    {
        foreach (Transform suitPile in _suitsPiles) Instantiate(sOGameSetup.cardPlaceholder, suitPile.position + sOGameSetup.placeholderPlacement, soDeckSetup.faceUpRotation);
    }

    public void DrawToDiscardPile()
    {
        _cardsInDiscardPile.AddRange(DeckManager.Instance.DrawCards(1, _discardPile.position));
        Vector3 nextPosition = _discardPile.position + sOGameSetup.discardPileIncrease;
        _discardPile.position = nextPosition;
    }
}

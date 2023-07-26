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
                _cardsColumns[i].position = NextPosition(_cardsColumns[i].position, soGameSetup.cardColumnIncrease);
            }

            _cardsShownInColumns[i].AddRange(DeckManager.Instance.DrawCards(1, _cardsColumns[i].position));
            _cardsColumns[i].position = NextPosition(_cardsColumns[i].position, soGameSetup.cardColumnIncrease);
        }
    }

    public void InstanceCardPlaceholders()
    {
        for (int i = 0; i < 4; i++)
        {
            _cardsInSuitPiles.Add(new List<GameObject>());
            _cardsInSuitPiles[i].Add(Instantiate(soGameSetup.cardPlaceholder, _suitsPiles[i].position + soGameSetup.placeholderPlacement, Quaternion.Euler(0f, 0f, 0f)));
            _suitsPiles[i].position = NextPosition(_suitsPiles[i].position, soGameSetup.pilesIncrease);
        }
    }

    public void DrawToDiscardPile()
    {
        _cardsInDiscardPile.AddRange(DeckManager.Instance.DrawCards(1, _discardPile.position));
        _discardPile.position = NextPosition(_discardPile.position, soGameSetup.pilesIncrease);
    }

    public Vector3 NextPosition(Vector3 currentPosition, Vector3 increase)
    {
        return currentPosition + increase;
    }

    public bool PlaceCard(GameObject card)
    {
        Vector3 direction = new Vector3(0, 0, 5);
        RaycastHit hit;

        if (Physics.Raycast(card.transform.position, direction, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (CheckSuitPile(card, hitObject)) return true;

            return false;
        }
        else
        {
            return false;
        }
    }

    public bool CheckSuitPile(GameObject card, GameObject hitObject)
    {
        for (int i = 0; i < 4; i++)
        {
            if (_cardsInSuitPiles[i].Contains(hitObject))
            {
                Card hitObjectComponent = hitObject.GetComponent<Card>();
                Card cardComponent = card.GetComponent<Card>();

                if(!hitObjectComponent)
                {
                    if(cardComponent.value == Card.Values.Ace)
                    {
                        _cardsInSuitPiles[i].Add(card);
                        card.transform.position = _suitsPiles[i].position;
                        _suitsPiles[i].position = NextPosition(_suitsPiles[i].position, soGameSetup.pilesIncrease);
                        return true;
                    }
                }

                if(hitObjectComponent)
                {
                    if (hitObjectComponent.value == (cardComponent.value - 1) && hitObjectComponent.suit == cardComponent.suit)
                    {
                        _cardsInSuitPiles[i].Add(card);
                        card.transform.position = _suitsPiles[i].position;
                        _suitsPiles[i].position = NextPosition(_suitsPiles[i].position, soGameSetup.pilesIncrease);
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
}

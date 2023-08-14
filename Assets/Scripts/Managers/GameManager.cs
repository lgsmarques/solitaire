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
                NextPosition(cardsColumns[i], soGameSetup.cardColumnIncrease);
            }

            DeckManager.Instance.DrawCards(1, cardsColumns[i].position, cardsShownInColumns[i]);
            NextPosition(cardsColumns[i], soGameSetup.cardColumnIncrease);
        }
    }

    public void InstanceCardPlaceholders()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject placeholder = Instantiate(soGameSetup.cardPlaceholderSuit, suitsPiles[i].position + soGameSetup.placeholderPlacement, Quaternion.Euler(0f, 0f, 0f));
            placeholder.transform.parent = cardsInSuitPiles[i].transform;
            NextPosition(suitsPiles[i], soGameSetup.pilesIncrease);
        }
    }

    public void DrawToDiscardPile()
    {
        DeckManager.Instance.DrawCards(1, discardPile.position, cardsInDiscardPile);
        NextPosition(discardPile, soGameSetup.pilesIncrease);
    }

    public void NextPosition(Transform currentPosition, Vector3 increase)
    {
        currentPosition.position += increase;
    }    
}
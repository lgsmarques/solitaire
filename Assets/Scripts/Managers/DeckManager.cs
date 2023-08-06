using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Core.Singleton;

public class DeckManager : Singleton<DeckManager>
{
    [Header("Setups")]
    public SODeckSetup sODeckSetup;
    public GameManager gameManager;

    [Header("Decks")]
    public List<GameObject> deck;

    [Header("Card Spawn")]
    public Transform cardSpawn;

    private GameObject deckPileParent;
    private GameObject deckPileChild;
    //private GameObject cardParent;
    //private GameObject cardChild;
    private GameObject deckPlaceholder;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        DestroyAllCards();
    //    }
    //}

    #region Deck

    public void ResetDeck()
    {
        deck = new List<GameObject>();
        DestroyAllCards();

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Cards" });

        for (int i = 0; i < prefabGUIDs.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                GameObject card = Instantiate(prefab);
                card.transform.position = cardSpawn.position;
                deck.Add(card);
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        int initialDeck = deck.Count();
        List<GameObject> shuffledDeck = new();

        for (int i = 0; i < initialDeck; i++)
        {
            var index = Random.Range(0, deck.Count());

            shuffledDeck.Add(deck[index]);
            deck.RemoveAt(index);
        }

        deck = shuffledDeck;
    }

    public List<GameObject> DrawCards(int amount, Vector3 position, GameObject parent, bool faceUp = true)
    {
        List<GameObject> curentCards = new();

        for (int i = 0; i < amount; i++)
        {
            if (deck.Count <= 0) break;

            GameObject currentCard = InstantiateCard(deck.FirstOrDefault(), position, faceUp);
            currentCard.transform.parent = parent.transform;
            curentCards.Add(currentCard);
            deck.RemoveAt(0);
        }

        if (deck.Count <= 0)
        {
            Transform deckPlacement = deckPileParent.transform;
            DestroyDeck();
            deckPlaceholder = Instantiate(sODeckSetup.deckPlaceholder);
            deckPlaceholder.transform.parent = GameObject.FindWithTag("LevelBase").transform;
            deckPlaceholder.transform.position = deckPlacement.position;
        }

        return curentCards;
    }

    #endregion Deck

    #region Card

    public GameObject InstantiateCard(GameObject card, Vector3 position, bool faceUp = true)
    {
        card.transform.position = position;
        card.GetComponent<Card>().faceUp = faceUp;
        
        if (!card.gameObject.activeSelf)
        {
            card.SetActive(true);
        }

        GameObject cardChild;

        if (sODeckSetup.deckColor == SODeckSetup.Colors.Black) cardChild = Instantiate(card.GetComponent<Card>().blackBack);
        else if (sODeckSetup.deckColor == SODeckSetup.Colors.Blue) cardChild = Instantiate(card.GetComponent<Card>().blueBack);
        else cardChild = Instantiate(card.GetComponent<Card>().redBack);

        cardChild.transform.parent = card.transform;
        cardChild.transform.position = position;
        cardChild.transform.localScale = sODeckSetup.defaultScale;

        if (card.GetComponent<Card>().faceUp)
            cardChild.transform.rotation = sODeckSetup.faceUpRotation;
        else
            cardChild.transform.rotation = sODeckSetup.faceDownRotation;

        return card;
    }

    public void DestroyAllCards()
    {
        GameObject cardsInPlay = GameObject.FindWithTag("CardsInPlay");

        foreach (Transform child in cardsInPlay.transform)
        {
            int cardCount = child.childCount;

            for (int i = cardCount - 1; i >= 0; i--)
            {
                GameObject card = child.GetChild(i).gameObject;
                Destroy(card);
            }
        }
    }

    #endregion Card

    #region DeckPile

    public void InstantiateDeck(Vector3 position)
    {
        if (deckPlaceholder)
        {
            Destroy(deckPlaceholder);
        }

        if (!deckPileParent)
        {
            deckPileParent = Instantiate(sODeckSetup.deckPile);
            deckPileParent.transform.parent = GameObject.FindWithTag("LevelBase").transform;
            deckPileParent.transform.position = position;

            if (sODeckSetup.deckColor == SODeckSetup.Colors.Black) deckPileChild = Instantiate(deckPileParent.GetComponent<Deck>().backBlack);
            else if (sODeckSetup.deckColor == SODeckSetup.Colors.Blue) deckPileChild = Instantiate(deckPileParent.GetComponent<Deck>().backBlue);
            else deckPileChild = Instantiate(deckPileParent.GetComponent<Deck>().backRed);

            deckPileChild.transform.parent = deckPileParent.transform;
            deckPileChild.transform.position = position;
            deckPileChild.transform.localScale = sODeckSetup.defaultScale;
            deckPileChild.transform.rotation = sODeckSetup.faceUpRotation;
        }
    }

    public void DestroyDeck()
    {
        Destroy(deckPileParent);
    }

    public void ResetDeckPile()
    {
        GameObject cardsInDiscard = GameObject.FindWithTag("CardsInPlayDiscard").gameObject;

        for (int i = 0; i < cardsInDiscard.transform.childCount; i++)
        {
            GameObject child = cardsInDiscard.transform.GetChild(i).gameObject;
            deck.Add(child);
            child.SetActive(false);
            child.transform.position = cardSpawn.position;
            GameObject childChild = child.transform.GetChild(0).gameObject;
            Destroy(childChild);

            gameManager.NextPosition(gameManager.discardPile, -gameManager.soGameSetup.pilesIncrease);
        }

        //Shuffle();
    }

    #endregion DeckPile
}
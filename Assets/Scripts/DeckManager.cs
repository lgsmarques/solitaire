using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Core.Singleton;

public class DeckManager : Singleton<DeckManager>
{
    [Header("Setups")]
    public SODeckSetup soDeckSetup;

    [Header("Decks")]
    public List<GameObject> deck;

    private GameObject _deckPileParent;
    private GameObject _deckPileChild;
    private GameObject _cardParent;
    private GameObject _cardChild;

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
                deck.Add(prefab);
            }
        }
    }

    public void Shuffle()
    {
        ResetDeck();

        int initialDeck = deck.Count();
        List<GameObject> shuffledDeck = new List<GameObject>();

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
            DestroyDeck();
        }

        return curentCards;
    }
    #endregion

    #region Card
    public GameObject InstantiateCard(GameObject card, Vector3 position, bool faceUp = true)
    {
        _cardParent = Instantiate(card);
        _cardParent.transform.position = position;
        _cardParent.GetComponent<Card>().faceUp = faceUp;

        if (soDeckSetup.deckColor == SODeckSetup.Colors.Black) _cardChild = Instantiate(card.GetComponent<Card>().blackBack);
        else if (soDeckSetup.deckColor == SODeckSetup.Colors.Blue) _cardChild = Instantiate(card.GetComponent<Card>().blueBack);
        else _cardChild = Instantiate(card.GetComponent<Card>().redBack);

        _cardChild.transform.parent = _cardParent.transform;
        _cardChild.transform.position = position;
        _cardChild.transform.localScale = soDeckSetup.defaultScale;

        if (_cardParent.GetComponent<Card>().faceUp)
            _cardChild.transform.rotation = soDeckSetup.faceUpRotation;
        else
            _cardChild.transform.rotation = soDeckSetup.faceDownRotation;

        return _cardParent;
    }

    public void DestroyAllCards()
    {
        GameObject cardsInPlay = GameObject.FindWithTag("CardsInPlay");

        foreach (Transform child in cardsInPlay.transform )
        {
            int cardCount = child.childCount;

            for (int i = cardCount - 1; i >= 0; i--)
            {
                GameObject card = child.GetChild(i).gameObject;
                Destroy(card);
            }
        }
    }
    #endregion

    #region DeckPile
    public void InstantiateDeck(Vector3 position)
    {
        if (!_deckPileParent)
        { 
            _deckPileParent = Instantiate(soDeckSetup.deckPile);
            _deckPileParent.transform.parent = GameObject.FindWithTag("LevelBase").transform ;
            _deckPileParent.transform.position = position;

            if (soDeckSetup.deckColor == SODeckSetup.Colors.Black) _deckPileChild = Instantiate(_deckPileParent.GetComponent<Deck>().backBlack);
            else if (soDeckSetup.deckColor == SODeckSetup.Colors.Blue) _deckPileChild = Instantiate(_deckPileParent.GetComponent<Deck>().backBlue);
            else _deckPileChild = Instantiate(_deckPileParent.GetComponent<Deck>().backRed);

            _deckPileChild.transform.parent = _deckPileParent.transform;
            _deckPileChild.transform.position = position;
            _deckPileChild.transform.localScale = soDeckSetup.defaultScale;
            _deckPileChild.transform.rotation = soDeckSetup.faceUpRotation;
        }
    }

    public void DestroyDeck()
    {
        Destroy(_deckPileParent);
    }
    #endregion
}
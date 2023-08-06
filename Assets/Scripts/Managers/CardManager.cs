using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class CardManager : Singleton<CardManager>
{
    public GameManager gameManager;

    public void MoveCard(GameObject card, Transform parent, Transform position)
    {
        GameObject cardInitialParent = card.transform.parent.gameObject;              

        card.transform.parent = parent;
        card.transform.position = position.position;

        if (cardInitialParent.CompareTag("CardsInPlayShownColumn"))
        {
            int columnIndex = gameManager.cardsShownInColumns.IndexOf(cardInitialParent);
            TurnCardFaceUp(columnIndex);
        }
        if (cardInitialParent.CompareTag("CardsInPlayDiscard"))
        {
            gameManager.NextPosition(gameManager.discardPile, -gameManager.soGameSetup.pilesIncrease);
        }
    }

    public void TurnCardFaceUp(int columnIndex)
    {
        int cardsInHiddenCollumn = gameManager.cardsHiddenInColumns[columnIndex].transform.childCount;
        int cardsInShownCollumn = gameManager.cardsShownInColumns[columnIndex].transform.childCount;

        if (cardsInHiddenCollumn > 0 && cardsInShownCollumn == 1)
        {
            GameObject lastCard = gameManager.cardsHiddenInColumns[columnIndex].transform.GetChild(cardsInHiddenCollumn - 1).gameObject;
            lastCard.transform.parent = gameManager.cardsShownInColumns[columnIndex].transform;
            lastCard.GetComponent<Card>().faceUp = true;
            lastCard.transform.GetChild(0).transform.rotation = gameManager.soDeckSetup.faceUpRotation;
        }
        gameManager.NextPosition(gameManager.cardsColumns[columnIndex], -gameManager.soGameSetup.cardColumnIncrease);
    }

    public bool PlaceCards(List<GameObject> cards)
    {
        Vector3 direction = new(0, 0, 5);

        if (Physics.Raycast(cards[0].transform.position, direction, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.transform.parent.CompareTag("CardsInPlaySuitPile") && cards.Count == 1)
            {
                if (SuitPilePlacement(cards[0], hitObject))
                {
                    return true;
                }
            }

            if (hitObject.transform.parent.CompareTag("CardsInPlayShownColumn"))
            {
                if (ColumnPlacement(cards, hitObject))
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

        int index = gameManager.cardsInSuitPiles.IndexOf(hitObject.transform.parent.gameObject);

        MoveCard(card, hitObject.transform.parent, gameManager.suitsPiles[index]);
        gameManager.NextPosition(gameManager.suitsPiles[index], gameManager.soGameSetup.pilesIncrease);
        return true;
    }

    public bool ColumnPlacement(List<GameObject> cards, GameObject hitObject)
    {
        if (hitObject.transform.parent == cards[0].transform.parent) return false;

        Card hitObjectComponent = hitObject.GetComponent<Card>();
        Card firstCardComponent = cards[0].GetComponent<Card>();

        if (!hitObjectComponent && firstCardComponent.value != Card.Values.King) return false;

        if (hitObjectComponent && (hitObjectComponent.value != (firstCardComponent.value + 1) || hitObjectComponent.CardColor() == firstCardComponent.CardColor()))
        {
            return false;
        }

        int index = gameManager.cardsShownInColumns.IndexOf(hitObject.transform.parent.gameObject);

        for (int i = 0; i < cards.Count; i++)
        {
            MoveCard(cards[i], hitObject.transform.parent, gameManager.cardsColumns[index]);
            gameManager.NextPosition(gameManager.cardsColumns[index], gameManager.soGameSetup.cardColumnIncrease);
        }        
        
        return true;
    }
}

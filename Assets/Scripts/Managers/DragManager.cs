using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class DragManager : Singleton<DragManager>
{
    public List<GameObject> cardsToDrag;

    [SerializeField] private List<Vector3> cardsToDragInitialPositions;
    [SerializeField] private List<Vector3> cardsToDragOffsets;
    [SerializeField] private bool isDraging;

    public void SetCardsToDrag(GameObject firstCard)
    {
        isDraging = true;
        
        if (firstCard.transform.parent.CompareTag("CardsInPlayShownColumn"))
        {
            GameObject collumn = firstCard.transform.parent.gameObject;
            
            for (int i = 1; i < collumn.transform.childCount; i++)
            {
                GameObject currentCard = collumn.transform.GetChild(i).gameObject;
                if (currentCard.GetComponent<Card>().value <= firstCard.GetComponent<Card>().value)
                {
                    cardsToDrag.Add(currentCard);
                    cardsToDragInitialPositions.Add(currentCard.transform.position);

                    Vector3 currentOffset = currentCard.transform.position - GetMouseWorldPosition();
                    cardsToDragOffsets.Add(currentOffset);
                }
            }
        }
        else
        {
            cardsToDrag.Add(firstCard);
            cardsToDragInitialPositions.Add(firstCard.transform.position);

            Vector3 currentOffset = firstCard.transform.position - GetMouseWorldPosition();
            cardsToDragOffsets.Add(currentOffset);
        }
    }

    public void DragCards()
    {
        if (isDraging)
        {
            for (int i = 0; i < cardsToDrag.Count; i++)
            {
                Vector3 newPosition = GetMouseWorldPosition() + cardsToDragOffsets[i];
                newPosition.z = -2 - (0.25f*i);
                cardsToDrag[i].transform.position = newPosition;
            }
        }
    }

    public void ReleaseCards()
    {
        bool cardPlaced = CardManager.Instance.PlaceCards(cardsToDrag);

        if (!cardPlaced)
        {
            for (int i = 0; i < cardsToDrag.Count; i++)
            {
                cardsToDrag[i].transform.position = cardsToDragInitialPositions[i];
            }
        }

        cardsToDrag = new();
        cardsToDragInitialPositions = new();
        cardsToDragOffsets = new();
        isDraging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}

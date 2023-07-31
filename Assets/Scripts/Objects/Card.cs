using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    public Suits suit;
    public Values value;
    public GameObject blackBack;
    public GameObject blueBack;
    public GameObject redBack;
    public bool faceUp;

    private Vector3 offset;
    private Vector3 initialPosition;
    private bool isDraging = false;

    public enum Suits
    {
        Club, Diamond, Heart, Spade
    }

    public enum Values
    {
        Ace = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6, 
        Eight = 7, 
        Nine = 8, 
        Ten = 9, 
        Jack = 10, 
        Queen = 11, 
        King = 12
    }
    private void OnMouseDown()
    {
        if(!gameObject.transform.parent.CompareTag("CardsInPlaySuitPile"))
        { 
            initialPosition = gameObject.transform.position;
            isDraging = true;
            offset = gameObject.transform.position - GetMouseWorldPosition();
        }
    }

    private void OnMouseDrag()
    {
        if (isDraging && faceUp)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.z = 0;
            gameObject.transform.position = newPosition;
        }
    }

    private void OnMouseUp()
    {
        if (isDraging)
        { 
            bool cardPlaced = GameManager.Instance.PlaceCard(gameObject);

            if (!cardPlaced) gameObject.transform.position = initialPosition;
        }
        isDraging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}

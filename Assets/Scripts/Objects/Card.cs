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
        if(!gameObject.transform.parent.CompareTag("CardsInPlaySuitPile") && faceUp)
        { 
            //initialPosition = gameObject.transform.position;
            //isDraging = true;
            //offset = gameObject.transform.position - GetMouseWorldPosition();

            DragManager.Instance.SetCardsToDrag(gameObject);
        }
    }

    private void OnMouseDrag()
    {
        if (faceUp)
        {
            //Vector3 newPosition = GetMouseWorldPosition() + offset;
            //newPosition.z = 0;
            //gameObject.transform.position = newPosition;

            DragManager.Instance.DragCards();
        }
    }

    private void OnMouseUp()
    {
        //if (isDraging)
        //{ 
        


        //}
        //isDraging = false;

        DragManager.Instance.ReleaseCards();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public string CardColor()
    {
        return suit switch
        {
            Suits.Diamond or Suits.Heart => "Red",
            Suits.Club or Suits.Spade => "Black",
            _ => "",
        };
    }
}

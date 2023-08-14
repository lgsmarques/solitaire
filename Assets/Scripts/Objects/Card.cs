using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [Header("Card Attributes")]
    public Suits suit;
    public Values value;
    public bool faceUp;

    [Header("Card Objects")]
    public GameObject blackBack;
    public GameObject blueBack;
    public GameObject redBack;

    [Header("Card Click Control")]
    public float doubleClickTimeThreshold = 0.2f;

    private float lastClickTime = 0f;

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
            if (Time.time - lastClickTime <= doubleClickTimeThreshold)
            {
                DragManager.Instance.AutoDragToSuitPlace(gameObject);
            }
            else
            {
                DragManager.Instance.SetCardsToDrag(gameObject);
            }

            lastClickTime = Time.time;
        }
    }

    private void OnMouseDrag()
    {
        if (faceUp)
        {
            DragManager.Instance.DragCards();
        }
    }

    private void OnMouseUp()
    {
        DragManager.Instance.ReleaseCards();
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

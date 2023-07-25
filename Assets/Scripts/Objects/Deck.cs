using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IPointerDownHandler
{
    public GameObject backBlack;
    public GameObject backBlue;
    public GameObject backRed;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.DrawToDiscardPile();
    }
}

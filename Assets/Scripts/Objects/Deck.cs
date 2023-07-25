using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour
{
    public GameObject backBlack;
    public GameObject backBlue;
    public GameObject backRed;

    private void OnMouseDown()
    {
        GameManager.Instance.DrawToDiscardPile();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPlaceholder : MonoBehaviour
{
    private void OnMouseDown()
    {
        DeckManager.Instance.ResetDeckPile();
        DeckManager.Instance.InstantiateDeck(gameObject.transform.position);
    }
}

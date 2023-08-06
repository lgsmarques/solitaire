using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SODeckSetup : ScriptableObject
{
    [Header("Deck Setup")]
    public Colors deckColor;
    public Quaternion faceUpRotation = Quaternion.Euler(-90f, 0f, 0f);
    public Quaternion faceDownRotation = Quaternion.Euler(90f, 0f, 0f);
    public Vector3 defaultScale = new(1, 1, 1);
    public GameObject deckPile;
    public GameObject deckPlaceholder;

    public enum Colors
    {
        Black, Blue, Red
    }
}

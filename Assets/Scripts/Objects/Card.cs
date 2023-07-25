using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Suits suit;
    public Values value;
    public GameObject blackBack;
    public GameObject blueBack;
    public GameObject redBack;
    public bool faceUp;

    public enum Suits
    {
        Club, Diamond, Heart, Spade
    }

    public enum Values
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }
}

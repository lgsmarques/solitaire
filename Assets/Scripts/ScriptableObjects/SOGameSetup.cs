using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOGameSetup : ScriptableObject
{
    [Header("Card Places")]
    public List<Transform> startPoints;
    public Vector3 pilesIncrease = new(0, 0, -0.25f);
    public Vector3 cardColumnIncrease = new(0, -2.5f, -0.25f);

    [Header("Card Placeholder")]
    public GameObject cardPlaceholder;
    public Vector3 placeholderPlacement = new(0, 0, 0.25f);
}

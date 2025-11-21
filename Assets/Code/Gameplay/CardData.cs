using UnityEngine;
using Game.Core;

[CreateAssetMenu(fileName = "NewCard", menuName = "Ecliptica/CardData")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    [TextArea]
    public string description;

    [Header("Stats")]
    public CardType cardType;
    public int sanityCost;

    //TODO Effect Card
}
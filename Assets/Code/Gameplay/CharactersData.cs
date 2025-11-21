using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Ecliptica/CharactersData")]
public class CharactersData : ScriptableObject {
    [Header("Identify")]
    public string CharacterName;

    [Header("BaseStats")]
    public int agility;
    public int power;
    public int durability;
}
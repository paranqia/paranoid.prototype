using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using Game.Gameplay.Cards;

namespace Game.Gameplay
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Ecliptica/CharactersData")]
    public class CharactersData : ScriptableObject {
        [Header("Identify")]
        public string characterName;
        public CharacterClass characterClass;

        [Header("BaseStats")]
        public int baseHP = 1000;
        public int agility;
        public int power;
        public int durability;

        [Header("Resource")]
        public int maxSanity = 100;

        [Header("Skill Pool")]
        public List<CardData> skillPool = new List<CardData>();
    }
}

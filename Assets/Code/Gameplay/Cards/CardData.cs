using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Gameplay.Cards
{
    public enum TargetType
    {
        Self,
        SingleEnemy,
        AllEnemies,
        RandomEnemy,
        Ally
    }

    [CreateAssetMenu(fileName = "NewCard", menuName = "Ecliptica/CardData")]
    public class CardData : ScriptableObject
    {
        [Header("Visuals")]
        public string cardName;
        [TextArea(3, 5)]
        public string description;
        public Sprite artwork;

        [Header("Stats")]
        public int cost;
        public Element element;
        public Game.Core.CardType cardType; // Explicitly use Game.Core.CardType
        public TargetType targetType;

        [Header("Mechanics")]
        // We will use a list of EffectDefinitions or similar later.
        // For MVP, we might hardcode some behavior or use a simple ActionType list like Combos.
        // Let's use a simplified approach for now: A list of "CardActions".
        
        public List<CardActionData> actions = new List<CardActionData>();
    }

    [System.Serializable]
    public class CardActionData
    {
        public ActionType actionType;
        public int value; // Damage, Shield, etc.
        public int duration; // For status effects
    }
}

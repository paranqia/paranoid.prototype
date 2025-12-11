using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.BattleActions
{
    public class DefendCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public CommandPriority Priority => CommandPriority.Normal;
        public CommandTags Tags => CommandTags.Defensive;

        // Modifiers
        public float ShieldMultiplier { get; set; } = 1.0f;

        public DefendCommand(Unit owner)
        {
            Owner = owner;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{Owner.unitName} assumes defensive stance.");
            
            // TODO: Play Animation
            yield return new WaitForSeconds(0.5f);

            // Add Shield logic
            // Formula: ShieldGain = Base * DurabilityScale * Multiplier
            int baseShield = 20;
            float statScale = Owner.currentDurability * 0.1f; // Example scaling
            int totalShield = Mathf.RoundToInt((baseShield + statScale) * ShieldMultiplier);

            Owner.currentShield += totalShield;
            
            Debug.Log($"{Owner.unitName} gained {totalShield} Shield! (Current: {Owner.currentShield})");
            yield return null;
        }
    }
}

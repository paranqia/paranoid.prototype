using UnityEngine;
using Game.Core;
using System.Collections.Generic;

namespace Game.Gameplay
{
    public class BossUnit : Unit
    {
        [Header("Boss Config")]
        public List<int> phaseHealthPools = new List<int>(); // HP for each phase
        public int currentPhaseIndex = 0;

        public override void Initialize()
        {
            base.Initialize(); // Initializes stats from Data

            // Override HP if phases are defined
            if (phaseHealthPools.Count > 0)
            {
                currentPhaseIndex = 0;
                maxHP = phaseHealthPools[0];
                currentHP = maxHP;
                Debug.Log($"<color=red>BOSS INITIALIZED: Phase {currentPhaseIndex + 1}/{phaseHealthPools.Count} (HP: {maxHP})</color>");
            }
            else
            {
                // Fallback: Create 3 phases of base HP if not defined
                if (maxHP > 0)
                {
                    phaseHealthPools.Add(maxHP);     // Phase 1
                    phaseHealthPools.Add(maxHP);     // Phase 2
                    phaseHealthPools.Add(maxHP / 2); // Phase 3 (Desperation)
                    Debug.Log($"<color=red>BOSS INITIALIZED (Auto-Gen Phases): {phaseHealthPools.Count} Phases</color>");
                }
            }
        }

        public override void TakeDamage(int amount)
        {
            int damageAfterShield = Mathf.Max(0, amount - currentShield);
            currentShield = Mathf.Max(0, currentShield - amount);

            currentHP -= damageAfterShield;
            Debug.Log($"BOSS took {damageAfterShield} damage. HP: {currentHP}/{maxHP} (Phase {currentPhaseIndex + 1})");

            if (currentHP <= 0)
            {
                // Check if we can transition to next phase
                if (currentPhaseIndex < phaseHealthPools.Count - 1)
                {
                    TransitionToNextPhase();
                }
                else
                {
                    Die();
                }
            }
        }

        private void TransitionToNextPhase()
        {
            currentPhaseIndex++;
            int nextMaxHP = phaseHealthPools[currentPhaseIndex];
            
            // Restore HP to full for next phase
            maxHP = nextMaxHP;
            currentHP = maxHP;
            
            // Clear Debuffs? (Reset state)
            // Clear Status Effects logic would go here
            
            // Publish Event
            Debug.Log($"<color=red>BOSS PHASE TRANSITION! Entering Phase {currentPhaseIndex + 1}</color>");
            EventBus.Publish(new BossPhaseChangedEvent(this, currentPhaseIndex, phaseHealthPools.Count));
            
            // Optional: Buff stats per phase
            basePower += 50; // Getting stronger
            baseAgility += 10;
            RecalculateStats();
        }

        protected override void Die()
        {
            Debug.Log("<color=red>BOSS DEFEATED!</color>");
            base.Die();
            // Publish Victory Event?
        }
    }
}


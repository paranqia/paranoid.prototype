using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Gameplay
{
    public class Unit : MonoBehaviour
    {
        [Header("Data Blueprint")]
        public CharactersData data; // ScriptableObject for base stats

        [Header("Live Stats")]
        public bool isPlayer; // Explicit flag to identify player
        public string unitName;
        public int currentHP;
        public int maxHP;
        public int currentSanity;
        public int maxSanity = 100;
        public int currentShield;
        public int currentAgility;
        public int currentPower;      // Live Power (Base + Modifiers)
        public int currentDurability; // Live Durability (Base + Modifiers)

        [Header("State")]
        public SanityState sanityState = SanityState.Lucid;
        public List<ICommand> plannedCommands = new List<ICommand>();

        public void Initialize()
        {
            // Initialize from data if available
            if (data != null)
            {
                unitName = data.characterName;
                maxHP = data.baseHP;
                currentHP = maxHP;
                currentAgility = data.agility;
                maxSanity = data.maxSanity;
                currentSanity = maxSanity;
                currentPower = data.power;
                currentDurability = data.durability;
            }
            UpdateSanityState();
        }

        public void AddCommand(ICommand command)
        {
            if (plannedCommands.Count < 3)
            {
                plannedCommands.Add(command);
                EventBus.Publish(new CommandAddedEvent(this, command));
                Debug.Log($"{unitName} added command: {command.GetType().Name}");
            }
            else
            {
                Debug.LogWarning($"{unitName} command queue full!");
            }
        }

        public void ClearCommands()
        {
            plannedCommands.Clear();
        }

        public void TakeDamage(int amount)
        {
            // Apply Defense Mitigation? 
            // GDD Formula: Final Damage = ... - (Target DEF / (Target DEF + K))
            // This usually happens in the Damage Calculation logic (e.g. in AttackCommand).
            // Here we just take the final calculated amount (minus shield).
            
            // But wait, if logic is in AttackCommand, then TakeDamage receives raw damage?
            // Usually TakeDamage receives the post-mitigation damage OR raw damage.
            // Let's assume AttackCommand calculates EVERYTHING including mitigation.
            
            int damageAfterShield = Mathf.Max(0, amount - currentShield);
            currentShield = Mathf.Max(0, currentShield - amount);
            
            currentHP -= damageAfterShield;
            Debug.Log($"{unitName} took {damageAfterShield} damage. HP: {currentHP}/{maxHP}");
            
            if (currentHP <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{unitName} has died!");
            EventBus.Publish(new UnitDiedEvent(this));
            // Disable or destroy
            gameObject.SetActive(false);
        }

        public bool SpendSanity(int amount)
        {
            if (currentSanity >= amount)
            {
                currentSanity -= amount;
                UpdateSanityState();
                EventBus.Publish(new SanityChangedEvent(this, currentSanity, maxSanity));
                return true;
            }
            return false;
        }

        public void RestoreSanity(int amount)
        {
            ModifySanity(amount);
        }

        public void ModifySanity(int amount)
        {
            int oldSanity = currentSanity;
            currentSanity = Mathf.Clamp(currentSanity + amount, 0, maxSanity);

            if (currentSanity != oldSanity)
            {
                UpdateSanityState();
                EventBus.Publish(new SanityChangedEvent(this, currentSanity, maxSanity));

                if (currentSanity <= 0 && oldSanity > 0)
                {
                    Debug.Log($"<color=red>{unitName} has suffered a SANITY BREAK!</color>");
                    EventBus.Publish(new SanityZeroEvent(this));
                }
            }
        }

        private void UpdateSanityState()
        {
            float percentage = (float)currentSanity / maxSanity;
            SanityState oldState = sanityState;

            if (percentage >= 0.7f) sanityState = SanityState.Lucid;
            else if (percentage >= 0.3f) sanityState = SanityState.Strained;
            else sanityState = SanityState.Fractured;

            if (oldState != sanityState)
            {
                Debug.Log($"{unitName} Sanity State changed to {sanityState}");
                EventBus.Publish(new SanityStateChangedEvent(this, sanityState));
            }
            
            // Apply Stat Modifiers based on State
            RecalculateStats();
        }

        private void RecalculateStats()
        {
            if (data == null) return;

            // Reset to base
            currentPower = data.power;
            currentDurability = data.durability;
            // Agility usually constant or buffed, let's keep base for now
            // currentAgility = data.agility; 

            // Apply Sanity Modifiers (GDD 6.1)
            switch (sanityState)
            {
                case SanityState.Lucid:
                    // Bonus: +DEF (Durability)
                    currentDurability += Mathf.RoundToInt(data.durability * 0.2f); // +20% DEF example
                    break;
                case SanityState.Strained:
                    // Normal
                    break;
                case SanityState.Fractured:
                    // Bonus: +ATK (Power), -DEF (Durability)
                    currentPower += Mathf.RoundToInt(data.power * 0.5f); // +50% ATK
                    currentDurability -= Mathf.RoundToInt(data.durability * 0.5f); // -50% DEF
                    break;
            }
            
            Debug.Log($"{unitName} Stats Updated [Sanity: {sanityState}] -> ATK: {currentPower}, DEF: {currentDurability}");
        }
        
        public float GetDefenseMitigation()
        {
            // Formula: (Target DEF / (Target DEF + K))
            // K = 500 (MVP)
            float k = 500f;
            return (float)currentDurability / (currentDurability + k);
        }
    }
}

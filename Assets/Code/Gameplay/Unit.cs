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
        public string unitName;
        public int currentHP;
        public int maxHP;
        public int currentSanity;
        public int maxSanity = 100;
        public int currentShield;
        public int currentAgility;

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
            }
            UpdateSanityState();
        }

        public void AddCommand(ICommand command)
        {
            if (plannedCommands.Count < 3)
            {
                plannedCommands.Add(command);
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
            currentSanity = Mathf.Min(maxSanity, currentSanity + amount);
            UpdateSanityState();
            EventBus.Publish(new SanityChangedEvent(this, currentSanity, maxSanity));
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
        }
    }
}

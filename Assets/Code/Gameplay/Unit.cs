using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Gameplay
{
    public class Unit : MonoBehaviour
    {
        [Header("Data Blueprint")]
        public CharactersData data;

        [Header("Live Stats")]
        public bool isPlayer;
        public string unitName;
        public int currentHP;
        public int maxHP;
        public int currentSanity;
        public int maxSanity = 100;
        public int currentShield;
        
        // Base Stats (from Data)
        public int baseAgility;
        public int basePower;
        public int baseDurability;

        // Current Effective Stats
        public int currentAgility;
        public int currentPower;
        public int currentDurability;
        
        // New Secondary Stats
        public float critChance = 0.05f; // 5% Base
        public float critDamage = 1.50f; // 150% Base

        [Header("State")]
        public SanityState sanityState = SanityState.Lucid;
        public List<ICommand> plannedCommands = new List<ICommand>();

        protected virtual void Start()
        {
             // Subscribe to events
             EventBus.Subscribe<FieldStateChangedEvent>(OnFieldStateChanged);
        }

        protected virtual void OnDestroy()
        {
             EventBus.Unsubscribe<FieldStateChangedEvent>(OnFieldStateChanged);
        }

        public virtual void Initialize()
        {
            if (data != null)
            {
                unitName = data.characterName;
                maxHP = data.baseHP;
                currentHP = maxHP;
                maxSanity = data.maxSanity;
                currentSanity = maxSanity;
                
                // Store Base
                baseAgility = data.agility;
                basePower = data.power;
                baseDurability = data.durability;
            }
            UpdateSanityState(); // This calls RecalculateStats
        }

        private void OnFieldStateChanged(FieldStateChangedEvent evt)
        {
            RecalculateStats();
        }

        public void RecalculateStats()
        {
            if (data == null) return;

            // 1. Reset to Base
            currentPower = basePower;
            currentDurability = baseDurability;
            currentAgility = baseAgility;
            
            // 2. Sanity Modifiers
            ApplySanityModifiers();

            // 3. Field Modifiers (GDD 8.1)
            ApplyFieldModifiers();

            Debug.Log($"Stats Updated for {unitName}: ATK {currentPower}, DEF {currentDurability}, CRIT {critChance*100}%");
        }

        private void ApplySanityModifiers()
        {
            switch (sanityState)
            {
                case SanityState.Lucid:
                    currentDurability += Mathf.RoundToInt(baseDurability * 0.2f);
                    break;
                case SanityState.Strained:
                    break;
                case SanityState.Fractured:
                    currentPower += Mathf.RoundToInt(basePower * 0.5f);
                    currentDurability -= Mathf.RoundToInt(baseDurability * 0.5f);
                    break;
            }
        }

        private void ApplyFieldModifiers()
        {
            if (FieldManager.Instance == null) return;
            
            FieldState field = FieldManager.Instance.CurrentFieldState;

            // Reset secondary stats to base first (simplification)
            critChance = 0.05f; 

            switch (field)
            {
                case FieldState.LogosDominance:
                    // +DEF both sides, -Crit
                    currentDurability += Mathf.RoundToInt(baseDurability * 0.2f); // +20% DEF
                    critChance = 0f; // No crits (Safe)
                    break;
                    
                case FieldState.IllogicDominance:
                    // -DEF both sides, +Crit Massive
                    currentDurability -= Mathf.RoundToInt(baseDurability * 0.3f); // -30% DEF
                    critChance += 0.50f; // +50% Crit Rate!
                    break;
                    
                case FieldState.NihilDominance:
                    // No stat mods here, effect happens at End Turn (Reset Buffs)
                    break;
            }
        }

        public void AddCommand(ICommand command)
        {
            if (plannedCommands.Count < 3)
            {
                plannedCommands.Add(command);
                EventBus.Publish(new CommandAddedEvent(this, command));
            }
        }

        public void ClearCommands()
        {
            plannedCommands.Clear();
        }

        public virtual void TakeDamage(int amount)
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

        protected virtual void Die()
        {
            Debug.Log($"{unitName} has died!");
            EventBus.Publish(new UnitDiedEvent(this));
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
            
            RecalculateStats();
        }
        
        public float GetDefenseMitigation()
        {
            float k = 500f;
            // Ensure Durability doesn't go below 0
            float def = Mathf.Max(0, currentDurability);
            return def / (def + k);
        }
    }
}

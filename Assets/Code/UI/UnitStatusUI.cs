using UnityEngine;
using UnityEngine.UI; // Assuming standard UI for now, or TMPro
using System.Collections.Generic;
using Game.Managers;
using Game.Gameplay;
using Game.Core;

namespace Game.UI
{
    // This script manages the HUD for a single Unit in the Party Panel
    public class UnitStatusUI : MonoBehaviour
    {
        [Header("References")]
        public Unit targetUnit;
        public Text nameText; // Placeholder for TMPro
        public Slider hpSlider;
        public Text hpText;
        public Slider sanitySlider;
        public Text sanityText;
        public Text shieldText;
        public Image portraitImage;
        public Image activeTurnIndicator; // Visual cue for selected unit

        [Header("Sanity State Colors")]
        public Color lucidColor = Color.cyan;
        public Color strainedColor = Color.yellow;
        public Color fracturedColor = Color.red;
        public Image sanityFillImage;

        private void OnEnable()
        {
            // Subscribe to events via EventBus? 
            // Better to listen globally and filter by targetUnit to decouple?
            // Or just update in Update() for MVP?
            // Let's use EventBus for efficiency.
            EventBus.Subscribe<UnitDamagedEvent>(OnUnitDamaged);
            EventBus.Subscribe<SanityChangedEvent>(OnSanityChanged);
            EventBus.Subscribe<SanityStateChangedEvent>(OnSanityStateChanged);
            // EventBus.Subscribe<UnitSelectedEvent>(OnUnitSelected); // We need this event
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<UnitDamagedEvent>(OnUnitDamaged);
            EventBus.Unsubscribe<SanityChangedEvent>(OnSanityChanged);
            EventBus.Unsubscribe<SanityStateChangedEvent>(OnSanityStateChanged);
        }

        public void Bind(Unit unit)
        {
            targetUnit = unit;
            if (unit == null) return;

            if (nameText) nameText.text = unit.unitName;
            
            // Initial Updates
            UpdateHP();
            UpdateSanity();
            UpdateShield();
            UpdateVisualState();

            // if (portraitImage && unit.data.portrait) portraitImage.sprite = unit.data.portrait;
        }

        private void UpdateHP()
        {
            if (targetUnit == null) return;
            if (hpSlider) 
            {
                hpSlider.maxValue = targetUnit.maxHP;
                hpSlider.value = targetUnit.currentHP;
            }
            if (hpText) hpText.text = $"{targetUnit.currentHP}/{targetUnit.maxHP}";
        }

        private void UpdateSanity()
        {
            if (targetUnit == null) return;
            if (sanitySlider)
            {
                sanitySlider.maxValue = targetUnit.maxSanity;
                sanitySlider.value = targetUnit.currentSanity;
            }
            if (sanityText) sanityText.text = $"{targetUnit.currentSanity}";
        }
        
        private void UpdateShield()
        {
            if (targetUnit == null) return;
            if (shieldText) 
            {
                shieldText.text = targetUnit.currentShield > 0 ? $"[{targetUnit.currentShield}]" : "";
                shieldText.gameObject.SetActive(targetUnit.currentShield > 0);
            }
        }

        private void UpdateVisualState()
        {
             if (targetUnit == null || sanityFillImage == null) return;

             switch (targetUnit.sanityState)
             {
                 case SanityState.Lucid: sanityFillImage.color = lucidColor; break;
                 case SanityState.Strained: sanityFillImage.color = strainedColor; break;
                 case SanityState.Fractured: sanityFillImage.color = fracturedColor; break;
             }
        }

        // --- Event Handlers ---

        private void OnUnitDamaged(UnitDamagedEvent evt)
        {
            if (evt.Target == targetUnit)
            {
                UpdateHP();
                UpdateShield();
                // Play shake animation?
            }
        }

        private void OnSanityChanged(SanityChangedEvent evt)
        {
             if (evt.Target == targetUnit)
             {
                 UpdateSanity();
             }
        }

        private void OnSanityStateChanged(SanityStateChangedEvent evt)
        {
            if (evt.Target == targetUnit)
            {
                UpdateVisualState();
            }
        }
        
        public void SetSelected(bool selected)
        {
            if (activeTurnIndicator) activeTurnIndicator.gameObject.SetActive(selected);
        }
    }
}


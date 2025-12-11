using UnityEngine;
using Game.Core;
using Game.Gameplay;
using System.Collections.Generic;

namespace Game.Gameplay
{
    public class FieldManager : MonoBehaviour
    {
        public static FieldManager Instance { get; private set; }

        [Header("State")]
        public FieldState CurrentFieldState = FieldState.Neutral;
        public Element DominantElement = Element.None;

        [Header("Logic")]
        private List<Element> recentElements = new List<Element>();
        private const int RESONANCE_THRESHOLD = 3;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            EventBus.Subscribe<CardPlayedEvent>(OnCardPlayed);
            EventBus.Subscribe<TurnEndedEvent>(OnTurnEnded);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<CardPlayedEvent>(OnCardPlayed);
            EventBus.Unsubscribe<TurnEndedEvent>(OnTurnEnded);
        }

        private void OnCardPlayed(CardPlayedEvent evt)
        {
            if (evt.Card != null && evt.Card.Data != null)
            {
                ProcessElement(evt.Card.Data.element);
            }
        }

        public void ProcessElement(Element element)
        {
            if (element == Element.None) return;

            recentElements.Add(element);
            
            if (recentElements.Count > RESONANCE_THRESHOLD)
            {
                recentElements.RemoveAt(0);
            }

            CheckResonance();
        }

        private void CheckResonance()
        {
            if (recentElements.Count < RESONANCE_THRESHOLD) return;

            Element target = recentElements[0];
            bool allMatch = true;
            foreach (var e in recentElements)
            {
                if (e != target)
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                SetFieldState(GetStateFromElement(target));
            }
        }

        private FieldState GetStateFromElement(Element e)
        {
            switch (e)
            {
                case Element.Logos: return FieldState.LogosDominance;
                case Element.Illogic: return FieldState.IllogicDominance;
                case Element.Nihil: return FieldState.NihilDominance;
                default: return FieldState.Neutral;
            }
        }

        private void SetFieldState(FieldState newState)
        {
            if (CurrentFieldState != newState)
            {
                CurrentFieldState = newState;
                Debug.Log($"<color=cyan>FIELD RESONANCE CHANGED: {newState}</color>");
                EventBus.Publish(new FieldStateChangedEvent(newState));
            }
        }

        private void OnTurnEnded(TurnEndedEvent evt)
        {
            // GDD: Nihil Dominance -> Reset State at End of Turn
            if (CurrentFieldState == FieldState.NihilDominance)
            {
                Debug.Log("<color=magenta>NIHIL DOMINANCE: The Void consumes all status effects.</color>");
                // Reset Logic: Clear Buffs/Debuffs (Placeholder)
                // Also maybe reset the field itself?
                // For MVP: Reset Field to Neutral after effect triggers?
                // Or keeps it until other elements overwrite? 
                // GDD says "Reset State". Let's assume it clears Buffs.
                
                // Also Decay logic for resonance
                recentElements.Clear();
                SetFieldState(FieldState.Neutral);
            }
        }

        public float GetDamageMultiplier(Element attackerElement)
        {
            if (attackerElement == Element.None) return 1.0f;

            // Simplified GDD Logic:
            // If Field matches Element -> +20% Damage (1.2x) (GDD says Element Advantage is 1.2)
            // But Field Resonance is distinct.
            // Let's keep a small bonus for matching field.
            
            if (CurrentFieldState == FieldState.LogosDominance && attackerElement == Element.Logos) return 1.2f;
            if (CurrentFieldState == FieldState.IllogicDominance && attackerElement == Element.Illogic) return 1.2f;
            if (CurrentFieldState == FieldState.NihilDominance && attackerElement == Element.Nihil) return 1.2f;

            return 1.0f;
        }
    }
}

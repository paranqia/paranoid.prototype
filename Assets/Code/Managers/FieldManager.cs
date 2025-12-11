using UnityEngine;
using Game.Core;
using Game.Gameplay;
using System.Collections.Generic;

namespace Game.Managers
{
    public class FieldManager : MonoBehaviour
    {
        public static FieldManager Instance { get; private set; }

        [Header("State")]
        public FieldState CurrentFieldState = FieldState.Neutral;
        public Element DominantElement = Element.None;

        [Header("Logic")]
        private List<Element> recentElements = new List<Element>();
        private const int RESONANCE_THRESHOLD = 3; // Needs 3 same elements to trigger

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            EventBus.Subscribe<CardPlayedEvent>(OnCardPlayed);
            EventBus.Subscribe<TurnEndedEvent>(OnTurnEnded); // Optional: Reset or decay?
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
                Debug.Log($"FieldManager: Processed Element {evt.Card.Data.element} from {evt.Card.Data.cardName}");
            }
        }

        public void ProcessElement(Element element)
        {
            if (element == Element.None) return;

            recentElements.Add(element);
            
            // Keep only last N elements
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
            // Optional: Decay logic (e.g., remove 1 element from history)
        }
    }
}

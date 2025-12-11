using UnityEngine;
using System.Collections.Generic;
using Game.Gameplay;
using Game.Gameplay.Cards;
using Game.Core;
using System.Linq;

namespace Game.Managers
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance { get; private set; }

        [Header("Config")]
        public int maxHandSize = 5;

        [Header("Runtime State")]
        public List<Card> hand = new List<Card>();
        // We no longer use drawPile/discardPile for the infinite draw system
        
        // The pool of all available skills from the party
        private List<SkillEntry> globalSkillPool = new List<SkillEntry>();

        private struct SkillEntry
        {
            public CardData data;
            public Unit owner;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void InitializeDeck(List<Unit> party)
        {
            globalSkillPool.Clear();
            hand.Clear();

            foreach (var unit in party)
            {
                if (unit.data != null && unit.data.skillPool != null)
                {
                    foreach (var cardData in unit.data.skillPool)
                    {
                        globalSkillPool.Add(new SkillEntry { data = cardData, owner = unit });
                    }
                }
                else
                {
                    Debug.LogWarning($"Unit {unit.unitName} has no data or skill pool!");
                }
            }

            Debug.Log($"Deck Initialized with {globalSkillPool.Count} skills in the pool from {party.Count} units.");
        }

        // For backward compatibility / testing with single unit
        public void InitializeDeck(Unit singleUnit)
        {
            InitializeDeck(new List<Unit> { singleUnit });
        }

        public void DrawCard()
        {
            if (hand.Count >= maxHandSize)
            {
                Debug.Log("Hand is full!");
                return;
            }

            if (globalSkillPool.Count == 0)
            {
                Debug.LogWarning("Global Skill Pool is empty! Cannot draw.");
                return;
            }

            // Infinite Draw Logic: Pick random skill from pool
            // Constraint: No duplicate CardData in hand (Archetype/Skill duplication check)
            
            // Try to find a valid card to draw (up to a few attempts to avoid infinite loops if pool is small)
            SkillEntry pickedEntry = default;
            bool validPick = false;
            int attempts = 0;
            int maxAttempts = 20;

            while (!validPick && attempts < maxAttempts)
            {
                int randomIndex = Random.Range(0, globalSkillPool.Count);
                pickedEntry = globalSkillPool[randomIndex];

                // Check for duplicates in hand
                bool isDuplicate = hand.Any(c => c.Data == pickedEntry.data);
                
                // If we have enough variety in pool, enforce no duplicates. 
                // If pool is small (e.g. testing with 2 cards), allow duplicates to fill hand.
                if (!isDuplicate || globalSkillPool.Count < maxHandSize)
                {
                    validPick = true;
                }
                
                attempts++;
            }

            if (validPick)
            {
                // Create new instance
                Card newCard = new Card(pickedEntry.data, pickedEntry.owner);
                hand.Add(newCard);
                
                EventBus.Publish(new HandUpdatedEvent(hand));
                Debug.Log($"Drew: {newCard.Data.cardName} (Owner: {newCard.Owner.unitName})");
            }
            else
            {
                Debug.LogWarning("Could not find a non-duplicate card to draw (Pool too small or unlucky).");
            }
        }

        public void DrawFullHand()
        {
            while (hand.Count < maxHandSize)
            {
                DrawCard();
                // Safety break if we can't draw anymore (e.g. pool empty)
                if (globalSkillPool.Count == 0) break;
                // Safety break if we are stuck (hand not increasing) - logic in DrawCard handles this but good to be safe
            }
        }

        public void DiscardHand()
        {
            // TODO: Implement Lock mechanic here. 
            // For now, clear all cards (assuming no locks).
            // In the future: hand.RemoveAll(c => !c.IsLocked);
            
            hand.Clear();
            EventBus.Publish(new HandUpdatedEvent(hand));
            Debug.Log("Hand Discarded (Refreshed).");
        }

        public void PlayCard(Card card)
        {
            if (hand.Contains(card))
            {
                hand.Remove(card);
                // No discard pile in infinite draw system, just remove from hand
                EventBus.Publish(new HandUpdatedEvent(hand));
            }
        }
    }
}

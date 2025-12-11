using UnityEngine;
using System.Collections.Generic;
using Game.Gameplay;
using Game.Gameplay.Cards;
using Game.Core;

namespace Game.Managers
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance { get; private set; }

        [Header("Config")]
        public List<CardData> initialDeckConfig = new List<CardData>();
        public int maxHandSize = 5;

        [Header("Runtime State")]
        public List<Card> drawPile = new List<Card>();
        public List<Card> hand = new List<Card>();
        public List<Card> discardPile = new List<Card>();

        private Unit playerUnit;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void InitializeDeck(Unit owner)
        {
            playerUnit = owner;
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();

            // Instantiate runtime cards
            foreach (var data in initialDeckConfig)
            {
                Card newCard = new Card(data, playerUnit);
                drawPile.Add(newCard);
            }

            ShuffleDrawPile();
            Debug.Log($"Deck Initialized with {drawPile.Count} cards.");
        }

        public void ShuffleDrawPile()
        {
            // Fisher-Yates Shuffle
            for (int i = 0; i < drawPile.Count; i++)
            {
                Card temp = drawPile[i];
                int randomIndex = Random.Range(i, drawPile.Count);
                drawPile[i] = drawPile[randomIndex];
                drawPile[randomIndex] = temp;
            }
            Debug.Log("Draw Pile Shuffled.");
        }

        public void DrawCard()
        {
            if (hand.Count >= maxHandSize)
            {
                Debug.Log("Hand is full!");
                return;
            }

            if (drawPile.Count == 0)
            {
                if (discardPile.Count > 0)
                {
                    ReshuffleDiscardToDraw();
                }
                else
                {
                    Debug.Log("No cards left to draw!");
                    return;
                }
            }

            Card card = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(card);
            
            // Notify UI (Event or direct call)
            EventBus.Publish(new HandUpdatedEvent(hand));
            Debug.Log($"Drew: {card.Data.cardName}");
        }

        public void DrawFullHand()
        {
            while (hand.Count < maxHandSize)
            {
                if (drawPile.Count == 0 && discardPile.Count == 0) break;
                DrawCard();
            }
        }

        public void DiscardHand()
        {
            foreach (var card in hand)
            {
                discardPile.Add(card);
            }
            hand.Clear();
            EventBus.Publish(new HandUpdatedEvent(hand));
            Debug.Log("Hand Discarded.");
        }

        public void PlayCard(Card card)
        {
            if (hand.Contains(card))
            {
                // Logic to play is handled by PlayerTurnState usually, 
                // but here we remove it from hand.
                hand.Remove(card);
                discardPile.Add(card);
                EventBus.Publish(new HandUpdatedEvent(hand));
            }
        }

        private void ReshuffleDiscardToDraw()
        {
            Debug.Log("Reshuffling Discard into Draw...");
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDrawPile();
        }
    }
}
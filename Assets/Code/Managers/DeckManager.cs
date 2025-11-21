using UnityEngine;
using System.Collections.Generic;
using Game.Gameplay;
using Game.Core;

namespace Game.Managers
{
    public class DeckManager : MonoBehaviour
    {
        [Header("Config")]
        public List<CardData> allDeck = new List<CardData>();

        [Header("Current State")]
        public List<CardData> currentHand = new List<CardData>();

        public void DrawCard()
        {
            if (allDeck.Count > 0)
            {
                int randomIndex = Random.Range(0, allDeck.Count);
                CardData randomCard = allDeck[randomIndex];

                currentHand.Add(randomCard);

                Debug.Log($"Drew card: {randomCard.cardName}");
            }
        }

        public void DrawFullHand()
        {
            while (currentHand.Count < 5)
            {
                DrawCard();
            }
        }
    }
}
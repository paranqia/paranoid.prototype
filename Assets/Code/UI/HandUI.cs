using UnityEngine;
using System.Collections.Generic;
using Game.Gameplay.Cards;
using Game.Core;
using Game.Gameplay; // For GameEvents

namespace Game.UI
{
    public class HandUI : MonoBehaviour
    {
        [Header("Config")]
        public GameObject cardPrefab;
        public Transform handContainer;

        private void Awake()
        {
            EventBus.Subscribe<HandUpdatedEvent>(OnHandUpdated);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<HandUpdatedEvent>(OnHandUpdated);
        }

        private void OnHandUpdated(HandUpdatedEvent evt)
        {
            UpdateHandVisuals(evt.Hand);
        }

        private void UpdateHandVisuals(List<Card> hand)
        {
            // Clear existing
            foreach (Transform child in handContainer)
            {
                Destroy(child.gameObject);
            }

            // Spawn new
            foreach (var card in hand)
            {
                GameObject cardObj = Instantiate(cardPrefab, handContainer);
                CardUI ui = cardObj.GetComponent<CardUI>();
                if (ui != null)
                {
                    ui.Setup(card);
                }
            }
        }
    }
}

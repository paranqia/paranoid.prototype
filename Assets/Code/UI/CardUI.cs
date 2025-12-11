using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay.Cards;
using Game.Managers;
using Game.Core; // For EventBus

namespace Game.UI
{
    public class CardUI : MonoBehaviour
    {
        [Header("UI References")]
        public Text nameText;
        public Text costText;
        public Text descriptionText;
        public Text ownerText; // New: Show who owns this skill
        public Image artworkImage;
        public Image lockIcon; // New: Visual for lock state
        public Button cardButton;
        public Button lockButton; // New: Specific button to toggle lock

        private Card currentCard;

        public void Setup(Card card)
        {
            currentCard = card;

            if (nameText) nameText.text = card.Data.cardName;
            if (costText) costText.text = card.Data.cost.ToString();
            if (descriptionText) descriptionText.text = card.Data.description;
            if (artworkImage) artworkImage.sprite = card.Data.artwork;
            
            // Owner Indicator
            if (ownerText && card.Owner != null)
            {
                ownerText.text = card.Owner.unitName;
                // Optional: color code based on owner?
            }

            // Lock State
            UpdateLockVisual();

            // Interactivity
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);

            if (lockButton)
            {
                lockButton.onClick.RemoveAllListeners();
                lockButton.onClick.AddListener(OnLockClicked);
            }
        }

        private void UpdateLockVisual()
        {
            if (lockIcon)
            {
                lockIcon.enabled = currentCard.IsLocked;
                // Or change color of the button/border
            }
        }

        private void OnLockClicked()
        {
            // Toggle Logic
            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.ToggleLockCard(currentCard);
                UpdateLockVisual(); // Optimistic update
            }
        }

        private void OnCardClicked()
        {
            Debug.Log($"Card Clicked: {currentCard.Data.cardName}");
            
            // Playing the card
            // For MVP: We assume the target is the selected Enemy (or auto-target)
            // But Card.Play() requires a target.
            
            // Logic:
            // 1. Check Sanity Cost
            if (currentCard.Owner.currentSanity < currentCard.Data.cost)
            {
                Debug.LogWarning("Not enough Sanity!");
                // Visual feedback?
                return;
            }

            // 2. Consume Sanity
            currentCard.Owner.SpendSanity(currentCard.Data.cost);

            // 3. Find Target (Simplified for MVP)
            // If Attack -> First Enemy
            // If Defend/Buff -> Self or Ally
            Game.Gameplay.Unit target = null;

            if (currentCard.Data.targetType == TargetType.SingleEnemy || currentCard.Data.targetType == TargetType.RandomEnemy || currentCard.Data.targetType == TargetType.AllEnemies)
            {
                // Find enemy
                target = BattleManager.Instance.Units.Find(u => !u.isPlayer);
            }
            else
            {
                // Self/Ally
                target = currentCard.Owner;
            }

            // 4. Play
            currentCard.Play(target);
            DeckManager.Instance.PlayCard(currentCard);
        }
    }
}

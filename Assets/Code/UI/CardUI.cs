using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay.Cards;
using Game.Managers;

namespace Game.UI
{
    public class CardUI : MonoBehaviour
    {
        [Header("UI References")]
        public Text nameText;
        public Text costText;
        public Text descriptionText;
        public Image artworkImage;
        public Button cardButton;

        private Card currentCard;

        public void Setup(Card card)
        {
            currentCard = card;

            if (nameText) nameText.text = card.Data.cardName;
            if (costText) costText.text = card.Data.cost.ToString();
            if (descriptionText) descriptionText.text = card.Data.description;
            if (artworkImage) artworkImage.sprite = card.Data.artwork;

            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }

        private void OnCardClicked()
        {
            Debug.Log($"Card Clicked: {currentCard.Data.cardName}");
            // Logic to play card
            // For MVP: Directly play it (assuming no target or auto-target)
            // In real game: Select Card -> Select Target -> Play
            
            // For now, let's assume we play it on a random enemy or self depending on type
            // Or just notify a "CardSelected" event and let a PlayerController handle it.
            
            // MVP Shortcut: Tell DeckManager to play it (remove from hand) and execute effect
            DeckManager.Instance.PlayCard(currentCard);
            currentCard.Play(null); // Target null for now
        }
    }
}

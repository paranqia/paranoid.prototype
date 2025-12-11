using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Gameplay;
using Game.Gameplay.AI; // For BossIntentDeclaredEvent
using System.Collections.Generic;

namespace Game.UI
{
    public class TelegraphUI : MonoBehaviour
    {
        [Header("UI References")]
        public Transform intentContainer; // A panel above Boss head
        public GameObject intentIconPrefab;
        public Text tooltipText; // Optional: Show text when hovering intent

        [Header("Icons")]
        public Sprite attackIcon;
        public Sprite defendIcon;
        public Sprite buffIcon;
        public Sprite ultimateIcon;
        public Sprite unknownIcon;

        private void OnEnable()
        {
            EventBus.Subscribe<BossIntentDeclaredEvent>(OnBossIntentDeclared);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<BossIntentDeclaredEvent>(OnBossIntentDeclared);
        }

        private void OnBossIntentDeclared(BossIntentDeclaredEvent evt)
        {
            if (intentContainer == null) return;

            // Clear old intents
            foreach (Transform child in intentContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new icons
            foreach (var intent in evt.Intents)
            {
                CreateIntentIcon(intent);
            }
        }

        private void CreateIntentIcon(BossIntent intent)
        {
            if (intentIconPrefab == null) return;

            GameObject iconObj = Instantiate(intentIconPrefab, intentContainer);
            Image img = iconObj.GetComponent<Image>();
            
            if (img != null)
            {
                img.sprite = GetIconForType(intent.type);
            }

            // Optional: Add tooltip logic component here if needed
        }

        private Sprite GetIconForType(IntentType type)
        {
            switch (type)
            {
                case IntentType.Attack: return attackIcon;
                case IntentType.StrongAttack: return attackIcon; // Use same icon or specialized
                case IntentType.Defend: return defendIcon;
                case IntentType.Buff: return buffIcon;
                case IntentType.Ultimate: return ultimateIcon;
                default: return unknownIcon;
            }
        }
    }
}


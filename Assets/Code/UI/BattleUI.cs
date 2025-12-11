using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Managers;
using Game.Gameplay;

namespace Game.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Controls")]
        public Button endTurnButton;

        private void Awake()
        {
            if (endTurnButton)
            {
                endTurnButton.onClick.AddListener(OnEndTurnClicked);
            }
            
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            if (endTurnButton)
            {
                endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            }
            
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnEndTurnClicked()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.EndTurn();
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            // Only enable End Turn button during Planning Phase
            if (endTurnButton)
            {
                endTurnButton.interactable = (evt.NewState == GameState.PlanningPhase);
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using Game.Gameplay;
using Game.Managers.States;
using System.Linq;

namespace Game.Managers
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        public BattleState CurrentState { get; private set; }

        [Header("References")]
        public DeckManager deckManager;
        public List<Unit> Units = new List<Unit>(); // Public field for Inspector assignment

        // Property to access the Player Unit specifically
        public Unit PlayerUnit { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Auto-find units if empty (Safety fallback)
            // Moved to Awake to ensure readiness for UI (Fixes Race Condition)
            if (Units.Count == 0)
            {
                Units = FindObjectsByType<Unit>(FindObjectsSortMode.None).ToList();
            }

            // Auto-find DeckManager if missing
            if (deckManager == null)
            {
                deckManager = GetComponent<DeckManager>();
                if (deckManager == null) deckManager = FindAnyObjectByType<DeckManager>();
            }

            // Identify Player Unit
            PlayerUnit = Units.Find(u => u.isPlayer);
            if (PlayerUnit == null && Units.Count > 0)
            {
                // Fallback: Assume first unit is player if no flag set
                PlayerUnit = Units[0];
                Debug.LogWarning($"BattleManager: No Unit marked as 'isPlayer'. Defaulting to {PlayerUnit.unitName}");
            }
        }

        public void SetUnits(List<Unit> units)
        {
            Units = units;
            
            // Re-identify Player Unit
            PlayerUnit = Units.Find(u => u.isPlayer);
            if (PlayerUnit == null && Units.Count > 0)
            {
                PlayerUnit = Units[0];
                Debug.LogWarning($"BattleManager: No Unit marked as 'isPlayer'. Defaulting to {PlayerUnit.unitName}");
            }
            
            // Re-Initialize Deck if needed (if Start() already ran could be tricky, but usually this is called before Start)
            if (deckManager != null && PlayerUnit != null)
            {
                deckManager.InitializeDeck(PlayerUnit);
            }
        }

        private void Start()
        {
            // Initialize Deck
            if (deckManager != null && PlayerUnit != null)
            {
                deckManager.InitializeDeck(PlayerUnit);
            }
            else
            {
                Debug.LogWarning("BattleManager: No PlayerUnit or DeckManager found!");
            }

            // Start with Setup State
            ChangeState(new SetupState(this));
        }

        private void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void ChangeState(BattleState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = newState;

            if (CurrentState != null)
            {
                CurrentState.Enter();
            }
        }

        public void EndTurn()
        {
            if (CurrentState is PlayerTurnState playerState)
            {
                playerState.EndTurn();
            }
            else
            {
                Debug.LogWarning("Cannot end turn: Not in PlayerTurnState");
            }
        }
    }
}
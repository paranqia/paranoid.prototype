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

        // Property to access the Main Player Unit (Leader/First)
        public Unit PlayerUnit { get; private set; }
        
        // Property to access all Player Units (Party)
        public List<Unit> PlayerParty { get; private set; } = new List<Unit>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Auto-find units if empty (Safety fallback)
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

            IdentifyUnits();
        }

        private void IdentifyUnits()
        {
            // Identify Player Party
            PlayerParty = Units.Where(u => u.isPlayer).ToList();
            
            // Identify Main Player Unit (first one found)
            if (PlayerParty.Count > 0)
            {
                PlayerUnit = PlayerParty[0];
            }
            else if (Units.Count > 0)
            {
                // Fallback: Assume first unit is player if no flag set
                PlayerUnit = Units[0];
                PlayerParty.Add(PlayerUnit);
                Debug.LogWarning($"BattleManager: No Unit marked as 'isPlayer'. Defaulting to {PlayerUnit.unitName}");
            }
        }

        public void SetUnits(List<Unit> units)
        {
            Units = units;
            IdentifyUnits();
            
            // Re-Initialize Deck if needed
            if (deckManager != null && PlayerParty.Count > 0)
            {
                deckManager.InitializeDeck(PlayerParty);
            }
        }

        private void Start()
        {
            // Initialize Deck with Party
            if (deckManager != null && PlayerParty.Count > 0)
            {
                deckManager.InitializeDeck(PlayerParty);
            }
            else
            {
                Debug.LogWarning("BattleManager: No PlayerParty or DeckManager found!");
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

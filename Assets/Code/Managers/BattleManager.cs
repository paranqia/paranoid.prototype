using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using Game.Gameplay;
using Game.Managers.States;

namespace Game.Managers
{
    public class BattleManager : MonoBehaviour
    {
        public BattleState CurrentState { get; private set; }

        [Header("References")]
        public TurnManager turnManager; // Keep for now, might replace with TimelineManager later
        public List<Unit> Units; // Renamed from testUnits
        public DeckManager deckManager;

        private void Start()
        {
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
    }
}
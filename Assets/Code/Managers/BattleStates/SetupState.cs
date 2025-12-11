using UnityEngine;
using Game.Managers;
using Game.Core; // For GameState enum
using Game.Gameplay; // For GameEvents

namespace Game.Managers.States
{
    public class SetupState : BattleState
    {
        public SetupState(BattleManager owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Entering Setup State...");
            EventBus.Publish(new GameStateChangedEvent(GameState.BattleStart)); // Or SetupPhase

            // 1. Reset / Prepare Units
            foreach (var unit in owner.Units)
            {
                // Clear old commands from previous turn
                unit.ClearCommands();
                
                // Regenerate Sanity/Shields if needed?
                // Trigger Start of Turn Effects
            }
            
            // 2. Draw Cards
            if (DeckManager.Instance != null)
            {
                // GDD: Draw until full (5)
                DeckManager.Instance.DrawFullHand();
            }

            // 3. Transition to Player Turn
            owner.ChangeState(new PlayerTurnState(owner));
        }
    }
}

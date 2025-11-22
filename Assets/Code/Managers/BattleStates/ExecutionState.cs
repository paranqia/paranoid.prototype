using UnityEngine;
using Game.Managers;
using Game.Core; // For GameState enum
using Game.Gameplay; // For GameEvents

namespace Game.Managers.States
{
    public class ExecutionState : BattleState
    {
        public ExecutionState(BattleManager owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Entering Execution State...");
            EventBus.Publish(new GameStateChangedEvent(GameState.ExecutionPhase));
            
            // Start Timeline Execution
            owner.StartCoroutine(ExecuteSequence());
        }

        private System.Collections.IEnumerator ExecuteSequence()
        {
            if (TimelineManager.Instance != null)
            {
                yield return TimelineManager.Instance.ExecuteQueue();
            }
            
            // Check Win/Loss or Loop back
            // For MVP, loop back to Setup (New Round) or PlayerTurn
            owner.ChangeState(new SetupState(owner)); // Loop for now
        }
    }
}

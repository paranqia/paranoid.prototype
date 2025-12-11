using UnityEngine;
using Game.Managers;
using Game.Core; // For GameState enum
using Game.Gameplay; // For Unit, ICommand, BattleActions

namespace Game.Managers.States
{
    public class PlayerTurnState : BattleState
    {
        public PlayerTurnState(BattleManager owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Entering Player Turn (Planning Phase)...");
            EventBus.Publish(new GameStateChangedEvent(GameState.PlanningPhase));
            
            // MVP: Generate Enemy Actions here (or in Setup)
            GenerateEnemyActions();
            
            // Enable UI interaction
        }

        private void GenerateEnemyActions()
        {
            foreach (var unit in owner.Units)
            {
                // Simple check: if not player, add random actions
                // For MVP, assume index 0 is player, others are enemies
                if (owner.Units.IndexOf(unit) != 0) 
                {
                    unit.ClearCommands();
                    // Add 3 random actions
                    unit.AddCommand(new Game.Gameplay.BattleActions.AttackCommand(unit, owner.Units[0])); // Attack player
                    unit.AddCommand(new Game.Gameplay.BattleActions.DefendCommand(unit));
                    unit.AddCommand(new Game.Gameplay.BattleActions.AnalysisCommand(unit));
                }
            }
        }

        public override void Update()
        {
            // Check for "End Turn" input
            // For MVP debug, use Spacebar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }
        }

        public void SelectCommand(ICommand command)
        {
            if (owner.PlayerUnit == null) return;
            owner.PlayerUnit.AddCommand(command);
        }

        // Helper for UI
        public void QueueAttack()
        {
            // For MVP: Auto-target first enemy
            Unit target = owner.Units.Find(u => !u.isPlayer);
            if (target != null)
            {
                SelectCommand(new Game.Gameplay.BattleActions.AttackCommand(owner.PlayerUnit, target));
            }
        }

        public void QueueDefend()
        {
            SelectCommand(new Game.Gameplay.BattleActions.DefendCommand(owner.PlayerUnit));
        }

        public void QueueAnalysis()
        {
            SelectCommand(new Game.Gameplay.BattleActions.AnalysisCommand(owner.PlayerUnit));
        }

        public void EndTurn()
        {
            Debug.Log("Player ended turn. Submitting actions...");
            SubmitActionsToTimeline();
            owner.ChangeState(new ExecutionState(owner));
        }

        private void SubmitActionsToTimeline()
        {
            if (TimelineManager.Instance == null) return;

            TimelineManager.Instance.ClearQueue();

            // Collect all commands from all units
            foreach (var unit in owner.Units)
            {
                foreach (var cmd in unit.plannedCommands)
                {
                    TimelineManager.Instance.AddCommand(cmd);
                }
                unit.ClearCommands(); // Clear after submitting
            }
        }
    }
}

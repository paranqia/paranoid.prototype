using UnityEngine;
using Game.Managers;
using Game.Core;
using Game.Gameplay;
using System.Collections.Generic;

namespace Game.Managers.States
{
    public class PlayerTurnState : BattleState
    {
        private int currentUnitIndex = 0;
        private Unit currentActiveUnit;

        public PlayerTurnState(BattleManager owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Entering Player Turn (Planning Phase)...");
            EventBus.Publish(new GameStateChangedEvent(GameState.PlanningPhase));

            // Generate Enemy Actions (Simulated for now)
            GenerateEnemyActions();

            // Setup Player Party Interaction
            currentUnitIndex = 0;
            if (owner.PlayerParty.Count > 0)
            {
                SelectUnit(owner.PlayerParty[0]);
            }
            else
            {
                Debug.LogError("No Player Units in Party!");
            }
        }

        private void GenerateEnemyActions()
        {
            // In a real implementation, this would call an EnemyAI system
            foreach (var unit in owner.Units)
            {
                if (!unit.isPlayer)
                {
                    unit.ClearCommands();
                    // Basic AI: Attack Player
                    // Need to find a target (Random player unit)
                    Unit target = owner.PlayerParty.Count > 0 
                        ? owner.PlayerParty[Random.Range(0, owner.PlayerParty.Count)] 
                        : null;

                    if (target != null)
                    {
                        unit.AddCommand(new Game.Gameplay.BattleActions.AttackCommand(unit, target));
                        unit.AddCommand(new Game.Gameplay.BattleActions.DefendCommand(unit));
                        unit.AddCommand(new Game.Gameplay.BattleActions.AnalysisCommand(unit));
                    }
                }
            }
        }

        public override void Update()
        {
            // Input Handling for Switching Units (Tab or UI buttons)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CycleNextUnit();
            }

            // Confirm / End Turn (Space)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) // Changed to Return/Enter for "Commit" feel
            {
                // Check if all units have commands?
                // For MVP, just End Turn
                EndTurn();
            }
        }

        private void SelectUnit(Unit unit)
        {
            currentActiveUnit = unit;
            Debug.Log($"<color=green>Selected Unit: {unit.unitName}</color>");
            // TODO: Publish UnitSelectedEvent for UI to update (show this unit's command queue)
            // EventBus.Publish(new UnitSelectedEvent(unit));
        }

        private void CycleNextUnit()
        {
            if (owner.PlayerParty.Count <= 1) return;

            currentUnitIndex = (currentUnitIndex + 1) % owner.PlayerParty.Count;
            SelectUnit(owner.PlayerParty[currentUnitIndex]);
        }

        // --- Command Selection Interface ---

        public void SelectCommand(ICommand command)
        {
            if (currentActiveUnit == null) return;
            currentActiveUnit.AddCommand(command);
        }

        // Helper for UI/Buttons
        public void QueueAttack()
        {
            // Auto-target first enemy for now
            Unit target = owner.Units.Find(u => !u.isPlayer);
            if (target != null)
            {
                SelectCommand(new Game.Gameplay.BattleActions.AttackCommand(currentActiveUnit, target, Game.Core.Element.Logos));
            }
        }

        public void QueueDefend()
        {
            SelectCommand(new Game.Gameplay.BattleActions.DefendCommand(currentActiveUnit));
        }

        public void QueueAnalysis()
        {
            SelectCommand(new Game.Gameplay.BattleActions.AnalysisCommand(currentActiveUnit));
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

            // Collect all commands from ALL units (Player + Enemy)
            foreach (var unit in owner.Units)
            {
                foreach (var cmd in unit.plannedCommands)
                {
                    TimelineManager.Instance.AddCommand(cmd);
                }
                // Don't clear unit.plannedCommands yet? 
                // Actually, we should clear them AFTER execution or HERE.
                // If we clear here, Unit UI might go blank during execution.
                // Better to clear at Start of next Turn (SetupState).
            }
        }
        
        public override void Exit()
        {
            // Cleanup if needed
        }
    }
}

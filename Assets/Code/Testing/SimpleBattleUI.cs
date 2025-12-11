using UnityEngine;
using Game.Managers;
using Game.Managers.States;
using Game.Gameplay;
using Game.Core;

namespace Game.Testing
{
    // Simple IMGUI for testing without Canvas overhead
    public class SimpleBattleUI : MonoBehaviour
    {
        private void OnGUI()
        {
            if (BattleManager.Instance == null || BattleManager.Instance.CurrentState is not PlayerTurnState)
            {
                // Only show UI during Player Turn
                GUI.Label(new Rect(10, 10, 200, 20), $"State: {BattleManager.Instance?.CurrentState?.GetType().Name ?? "Null"}");
                return;
            }

            PlayerTurnState state = (PlayerTurnState)BattleManager.Instance.CurrentState;
            Unit player = BattleManager.Instance.PlayerUnit;

            GUI.Label(new Rect(10, 10, 300, 20), $"PLAYER TURN - Sanity: {player?.currentSanity ?? 0}");

            // Action Buttons
            if (GUI.Button(new Rect(10, 40, 100, 30), "Attack"))
            {
                state.QueueAttack();
            }

            if (GUI.Button(new Rect(120, 40, 100, 30), "Defend"))
            {
                state.QueueDefend();
            }

            if (GUI.Button(new Rect(230, 40, 100, 30), "Analyze"))
            {
                state.QueueAnalysis();
            }

            // Queue Display
            string queueStr = "Queue: ";
            if (player != null)
            {
                foreach (var cmd in player.plannedCommands)
                {
                    queueStr += $"[{cmd.GetType().Name.Replace("Command", "")}] ";
                }
            }
            GUI.Label(new Rect(10, 80, 500, 20), queueStr);

            // End Turn Button
            if (GUI.Button(new Rect(10, 120, 320, 40), "END TURN (Execute)"))
            {
                state.EndTurn();
            }

            // --- DEBUG SECTION ---
            GUI.Label(new Rect(10, 170, 300, 20), $"FIELD: {FieldManager.Instance?.CurrentFieldState}");
            
            if (GUI.Button(new Rect(10, 200, 100, 30), "Debug: Logos"))
            {
                SimulateCard(Element.Logos);
            }
            if (GUI.Button(new Rect(120, 200, 100, 30), "Debug: Illogic"))
            {
                SimulateCard(Element.Illogic);
            }
            if (GUI.Button(new Rect(230, 200, 100, 30), "Debug: Nihil"))
            {
                SimulateCard(Element.Nihil);
            }
        }

        private void SimulateCard(Element element)
        {
            if (BattleManager.Instance?.PlayerUnit == null) return;

            // Fake card data
            Game.Gameplay.Cards.CardData fakeData = ScriptableObject.CreateInstance<Game.Gameplay.Cards.CardData>();
            fakeData.cardName = $"Debug {element}";
            fakeData.element = element;

            Game.Gameplay.Cards.Card fakeCard = new Game.Gameplay.Cards.Card(fakeData, BattleManager.Instance.PlayerUnit);
            
            // Manually fire event as if played
            EventBus.Publish(new CardPlayedEvent(BattleManager.Instance.PlayerUnit, fakeCard));
        }
    }
}

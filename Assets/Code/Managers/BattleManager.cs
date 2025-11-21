using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using Game.Gameplay;
using Game.Managers;

namespace GameManagers
{
    public class BattleManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; }

        [Header("Referances")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private List<Unit> testUnits;
        [SerializeField] private DeckManager deckManager;

        private void Start()
        {
            ChangeState(GameState.BattleStart);
        }

        private void Update()
        {
            if (CurrentState == GameState.PlanningPhase)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Player submitted actions! (Simulated)");

                    var playerUnit = testUnits[0];
                    playerUnit.ClearActions();
                    playerUnit.AddAction(ActionType.Attack);
                    playerUnit.AddAction(ActionType.Attack);
                    playerUnit.AddAction(ActionType.Attack);

                    ChangeState(GameState.ExcutionPhase);
                }
            }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            EventBus.PublishGameStateChange(newState);

            switch (newState)
            {
                case GameState.BattleStart:
                    Debug.Log("Battle Started! Setting up. . .");
                    
                    if (turnManager != null)
                    {
                        turnManager.SetupTurnOrder(testUnits);
                    }

                    if (deckManager != null)
                    {
                        deckManager.DrawFullHand();
                    }

                    ChangeState(GameState.PlanningPhase);
                    break;
                
                case GameState.PlanningPhase:
                    Debug.Log("Waiting for Player input. . .");
                    break;
                
                case GameState.ExcutionPhase:
                    Debug.Log("Watching action moving. . .");
                    break;

                case GameState.Victory:
                    Debug.Log("Player Win.");
                    break;

                case GameState.Defeat:
                    Debug.Log("Enemies Win.");
                    break;
            }
        }
    }
}
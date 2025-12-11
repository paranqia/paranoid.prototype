using UnityEngine;
using Game.Managers;

namespace Game.Managers.States
{
    public class SetupState : BattleState
    {
        public SetupState(BattleManager owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Entering Setup State...");
            
            // 1. Setup Turn Order (via TurnManager? Or TimelineManager?)
            // TimelineManager.Instance.Setup(owner.Units); // Hypothetical
            
            // 2. Draw Cards (via DeckManager)
            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.DrawFullHand();
            }
            
            // 3. Transition to Planning
            owner.ChangeState(new PlayerTurnState(owner));
        }
    }
}

using UnityEngine;
using Game.Core;
using Game.Gameplay;
using System.Collections.Generic;

namespace Game.Gameplay.AI
{
    public enum IntentType
    {
        Attack,
        StrongAttack,
        Defend,
        Buff,
        Debuff,
        Ultimate,
        Unknown
    }

    [System.Serializable]
    public struct BossIntent
    {
        public IntentType type;
        public ICommand command;
        public string description; // For Telegraph UI
    }

    public class BossAIController : MonoBehaviour
    {
        [Header("Config")]
        public BossUnit bossUnit;
        
        // MVP: Simple hardcoded patterns based on Phase
        // In full game, use ScriptableObjects for Patterns

        private void Start()
        {
            if (bossUnit == null) bossUnit = GetComponent<BossUnit>();
        }

        /// <summary>
        /// Generates 3 actions for the upcoming turn based on current Phase.
        /// </summary>
        public void GenerateTurnActions(List<Unit> playerTargets)
        {
            if (bossUnit == null) return;

            bossUnit.ClearCommands();
            int phase = bossUnit.currentPhaseIndex;

            // Simple Logic: 3 Actions per turn
            // Phase 0: Atk, Def, Atk
            // Phase 1: StrongAtk, Atk, Buff
            // Phase 2: Ultimate, Def, StrongAtk

            // In MVP, we just add commands directly. 
            // In future, we broadcast "Intent" first for Telegraph.

            List<BossIntent> intents = new List<BossIntent>();

            for (int i = 0; i < 3; i++)
            {
                BossIntent intent = SelectAction(phase, i, playerTargets);
                intents.Add(intent);
                bossUnit.AddCommand(intent.command);
            }

            // Publish Telegraph Event (so UI can show what's coming)
            EventBus.Publish(new BossIntentDeclaredEvent(bossUnit, intents));
        }

        private BossIntent SelectAction(int phase, int slotIndex, List<Unit> targets)
        {
            BossIntent intent = new BossIntent();
            Unit randomTarget = targets[Random.Range(0, targets.Count)];

            // Logic Tree
            if (phase == 0) // Phase 1: Normal
            {
                if (slotIndex == 1) // Middle action = Defend
                {
                    intent.type = IntentType.Defend;
                    intent.command = new BattleActions.DefendCommand(bossUnit);
                    intent.description = "Defends";
                }
                else
                {
                    intent.type = IntentType.Attack;
                    intent.command = new BattleActions.AttackCommand(bossUnit, randomTarget);
                    intent.description = $"Attacks {randomTarget.unitName}";
                }
            }
            else if (phase == 1) // Phase 2: Aggressive
            {
                if (slotIndex == 0)
                {
                    intent.type = IntentType.StrongAttack;
                    // Strong attack = Attack with modifier
                    var cmd = new BattleActions.AttackCommand(bossUnit, randomTarget, Element.Logos); // Example element
                    cmd.DamageMultiplier = 1.5f;
                    intent.command = cmd;
                    intent.description = "Heavy Strike";
                }
                else if (slotIndex == 2)
                {
                    intent.type = IntentType.Buff;
                    intent.command = new BattleActions.AnalysisCommand(bossUnit); // Reuse Analysis as Buff
                    intent.description = "Charges Power";
                }
                else
                {
                    intent.type = IntentType.Attack;
                    intent.command = new BattleActions.AttackCommand(bossUnit, randomTarget);
                    intent.description = "Quick Strike";
                }
            }
            else // Phase 3+: Desperation
            {
                // Ultimate every turn in first slot?
                if (slotIndex == 0)
                {
                    intent.type = IntentType.Ultimate;
                    // TODO: Create Ultimate Command. For now, massive attack.
                    var cmd = new BattleActions.AttackCommand(bossUnit, randomTarget, Element.Nihil);
                    cmd.DamageMultiplier = 2.5f;
                    intent.command = cmd;
                    intent.description = "VOID CRUSHER";
                }
                else
                {
                    intent.type = IntentType.Attack;
                    intent.command = new BattleActions.AttackCommand(bossUnit, randomTarget);
                    intent.description = "Flailing Strike";
                }
            }

            return intent;
        }
    }
}


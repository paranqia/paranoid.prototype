using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Game.Gameplay.BattleActions; // For specific commands
using System;
using System.Linq;

namespace Game.Gameplay.Combos
{
    public class ComboResolver : MonoBehaviour
    {
        public static ComboResolver Instance { get; private set; }

        [SerializeField] private List<ComboDefinition> advancedCombos;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        /// <summary>
        /// Analyzes a list of 3 commands and applies Combo bonuses (Basic or Advanced).
        /// Returns a modified list of commands (could be replacements).
        /// </summary>
        public List<ICommand> ResolveCombos(List<ICommand> inputCommands, Unit owner)
        {
            if (inputCommands == null || inputCommands.Count != 3)
            {
                return inputCommands; // Only support 3-hit combos for now
            }

            // 1. Convert to ActionTypes for pattern matching
            List<ActionType> types = inputCommands.Select(cmd => GetActionType(cmd)).ToList();

            // 2. Check Advanced Combos (Class Specific) - REPLACEMENT
            // (Only if configured in Inspector)
            if (advancedCombos != null)
            {
                foreach (var combo in advancedCombos)
                {
                    if (combo.RequiredClass == CharacterClass.None || combo.RequiredClass == owner.data.characterClass)
                    {
                        if (IsSequenceMatch(types, combo.InputSequence))
                        {
                            Debug.Log($"<color=orange>Advanced Combo Triggered: {combo.ComboName}</color>");
                            // Create the special command that replaces the sequence
                            ICommand specialCmd = CreateCommandFromCombo(combo, owner);
                            if (specialCmd != null)
                            {
                                return new List<ICommand> { specialCmd }; // Replace 3 with 1
                            }
                        }
                    }
                }
            }

            // 3. Check Basic Combos (Stat Modifiers) - MODIFICATION
            ApplyBasicCombos(inputCommands, types);

            return inputCommands;
        }

        private void ApplyBasicCombos(List<ICommand> cmds, List<ActionType> types)
        {
            string pattern = string.Join("", types.Select(t => t.ToString()[0])); // e.g. "AAA", "ADA"

            // GDD 4.2 Basic Combos
            switch (pattern)
            {
                case "AAA": // Attack Focus
                    if (cmds[2] is AttackCommand lastAtk)
                    {
                        lastAtk.DamageMultiplier += 0.18f;
                        Debug.Log("Combo: AAA -> Last Attack +18% DMG");
                    }
                    break;

                case "AAD":
                case "ADA":
                case "DAA": // Defensive Hybrid
                    foreach (var cmd in cmds)
                    {
                        if (cmd is DefendCommand def)
                        {
                            def.ShieldMultiplier += 0.08f;
                        }
                    }
                    Debug.Log($"Combo: {pattern} -> Defense +8% Shield");
                    break;

                case "AAN":
                case "ANA":
                case "NAA": // Sanity Hybrid
                    foreach (var cmd in cmds)
                    {
                        if (cmd is AnalysisCommand ana)
                        {
                            ana.SanityMultiplier += 0.24f;
                        }
                    }
                    Debug.Log($"Combo: {pattern} -> Analysis +24% Restore");
                    break;

                case "DDD": // Full Defense
                    foreach (var cmd in cmds)
                    {
                        if (cmd is DefendCommand def)
                        {
                            // GDD says 30% Dmg Red. For MVP, huge shield bonus.
                            def.ShieldMultiplier += 0.50f;
                        }
                    }
                    Debug.Log("Combo: DDD -> Massive Shield Bonus");
                    break;

                case "NNN": // Deep Analysis
                    if (cmds[2] is AnalysisCommand lastAna)
                    {
                        lastAna.SanityMultiplier += 0.20f;
                        Debug.Log("Combo: NNN -> Last Analysis +20% Restore");
                    }
                    break;

                // Balance (One of each)
                case "ADN":
                case "DAN":
                case "NAD":
                case "AND":
                case "DNA":
                case "NDA": 
                    foreach (var cmd in cmds)
                    {
                        if (cmd is AttackCommand atk) atk.DamageMultiplier += 0.04f;
                        else if (cmd is DefendCommand def) def.ShieldMultiplier += 0.04f;
                        else if (cmd is AnalysisCommand ana) ana.SanityMultiplier += 0.08f;
                    }
                    Debug.Log($"Combo: {pattern} -> Balance Bonus (+4% DMG/Shield, +8% Sanity)");
                    break;
            }
        }

        private ActionType GetActionType(ICommand cmd)
        {
            if (cmd is AttackCommand) return ActionType.Attack;
            if (cmd is DefendCommand) return ActionType.Defend;
            if (cmd is AnalysisCommand) return ActionType.Analyze;
            // Cards count as Skill or what? GDD says "Card (Neutral in 3-slot)".
            // If it's a Card, we might treat it as Skill.
            // But Card command is currently just Attack/Defend/Analysis wrapped?
            // Wait, Card.Play() creates standard commands. 
            // So if a card creates an AttackCommand, it counts as Attack.
            
            return ActionType.Skill;
        }

        private bool IsSequenceMatch(List<ActionType> input, List<ActionType> pattern)
        {
            if (input.Count != pattern.Count) return false;
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] != pattern[i]) return false;
            }
            return true;
        }

        private ICommand CreateCommandFromCombo(ComboDefinition combo, Unit owner)
        {
            string fullTypeName = "Game.Gameplay.BattleActions." + combo.ResultCommandClassName;
            Type type = Type.GetType(fullTypeName);
            
            if (type != null && typeof(ICommand).IsAssignableFrom(type))
            {
                return (ICommand)Activator.CreateInstance(type, owner);
            }
            
            Debug.LogError($"Could not create command: {fullTypeName}");
            return null;
        }
    }
}

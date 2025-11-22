using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Game.Gameplay; // For ICommand
using System;
using System.Linq;

namespace Game.Gameplay.Combos
{
    public class ComboResolver : MonoBehaviour
    {
        public static ComboResolver Instance { get; private set; }

        [SerializeField] private List<ComboDefinition> allCombos;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public ICommand ResolveCombo(List<ActionType> inputs, Unit owner)
        {
            // 1. Filter combos by Class
            var validCombos = allCombos.Where(c => c.RequiredClass == CharacterClass.None || c.RequiredClass == owner.data.characterClass).ToList();

            // 2. Check for sequence match
            foreach (var combo in validCombos)
            {
                if (IsSequenceMatch(inputs, combo.InputSequence))
                {
                    Debug.Log($"Combo Detected: {combo.ComboName} for {owner.unitName}");
                    return CreateCommandFromCombo(combo, owner);
                }
            }

            return null; // No combo found
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
            // Reflection to create command instance
            string fullTypeName = "Game.Gameplay.BattleActions." + combo.ResultCommandClassName;
            Type type = Type.GetType(fullTypeName);
            
            if (type != null && typeof(ICommand).IsAssignableFrom(type))
            {
                // Assume constructor takes (Unit owner) or (Unit owner, Unit target)
                // This is tricky because we don't know the target yet if it's a queue.
                // For MVP, let's assume Combo Commands are self-contained or find their target during Execute.
                // OR we pass the original target of the first action?
                
                // Let's assume constructor is (Unit owner)
                return (ICommand)Activator.CreateInstance(type, owner);
            }
            
            Debug.LogError($"Could not create command: {fullTypeName}");
            return null;
        }
    }
}

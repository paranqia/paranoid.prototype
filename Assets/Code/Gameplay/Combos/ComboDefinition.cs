using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.Combos
{
    [CreateAssetMenu(fileName = "NewCombo", menuName = "Paranoid/Combo Definition")]
    public class ComboDefinition : ScriptableObject
    {
        [Header("Requirements")]
        public CharacterClass RequiredClass = CharacterClass.None;
        public List<ActionType> InputSequence = new List<ActionType>(); // e.g. Attack, Attack, Attack

        [Header("Result")]
        public string ComboName;
        [Tooltip("The Class Name of the Command to execute (e.g. 'ExecutionStrikeCommand')")]
        public string ResultCommandClassName;
        
        // Optional: Override specific slots? 
        // For MVP, we assume the combo replaces the *entire* sequence or the *last* action?
        // GDD says: "AAA -> Last attack gets stronger" OR "DAA -> Counter Stance (Replaces all?)"
        // Let's assume it replaces the *entire* sequence with a single "Combo Command" 
        // OR it modifies the queue.
        
        // For simplicity in MVP: A Combo produces a SINGLE special command that replaces the 3 inputs.
        // If the combo is "AAA", the result is "TripleSlashCommand" which does the 3 hits + bonus.
    }
}

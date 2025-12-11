using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.BattleActions
{
    public class AnalysisCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public CommandPriority Priority => CommandPriority.Normal;
        public CommandTags Tags => CommandTags.None;

        // Modifiers
        public float SanityMultiplier { get; set; } = 1.0f;

        public AnalysisCommand(Unit owner)
        {
            Owner = owner;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{Owner.unitName} analyzes the battlefield...");
            
            yield return new WaitForSeconds(0.5f);

            // Base Sanity Restore (GDD 7.2.5: Base = 20)
            int baseRestore = 20;
            
            // Anomaly Modifier (TODO: Implement Anomaly Stat in Unit)
            // For now use basic multiplier
            int finalRestore = Mathf.RoundToInt(baseRestore * SanityMultiplier);

            Owner.RestoreSanity(finalRestore);
            
            // Publish event if needed, though Unit.RestoreSanity handles it
            
            Debug.Log($"{Owner.unitName} restored {finalRestore} Sanity.");
            yield return null;
        }
    }
}

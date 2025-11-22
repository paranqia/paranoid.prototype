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

        public AnalysisCommand(Unit owner)
        {
            Owner = owner;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{Owner.unitName} analyzes the battlefield...");
            
            yield return new WaitForSeconds(0.5f);

            int restoreAmount = 20; // Base
            Owner.RestoreSanity(restoreAmount);
            
            EventBus.Publish(new SanityChangedEvent(Owner, Owner.currentSanity, Owner.maxSanity));
            
            Debug.Log($"{Owner.unitName} restored {restoreAmount} Sanity.");
            yield return null;
        }
    }
}

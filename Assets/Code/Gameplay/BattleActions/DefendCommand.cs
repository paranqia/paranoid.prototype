using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.BattleActions
{
    public class DefendCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public CommandPriority Priority => CommandPriority.Normal;
        public CommandTags Tags => CommandTags.Defensive;

        public DefendCommand(Unit owner)
        {
            Owner = owner;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{Owner.unitName} assumes defensive stance.");
            
            // TODO: Play Animation
            yield return new WaitForSeconds(0.5f);

            // Add Shield logic
            // Owner.AddShield(50); // Placeholder
            
            Debug.Log($"{Owner.unitName} gained Shield!");
            yield return null;
        }
    }
}

using System.Collections;
using UnityEngine;
using Game.Core;
// using Game.Managers; // Removed to fix circular dependency

namespace Game.Gameplay.BattleActions
{
    public class InterruptCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public Unit Target { get; private set; }
        public CommandPriority Priority => CommandPriority.Immediate; // Fast!
        public CommandTags Tags => CommandTags.Interrupt;

        public InterruptCommand(Unit owner, Unit target)
        {
            Owner = owner;
            Target = target;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{Owner.unitName} attempts to INTERRUPT {Target.unitName}!");
            
            yield return new WaitForSeconds(0.2f); // Very fast

            // Decoupled Logic: Publish RequestInterruptEvent
            EventBus.Publish(new RequestInterruptEvent(Target, Owner));
            
            yield return new WaitForSeconds(0.3f);
        }
    }
}

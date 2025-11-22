using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.BattleActions
{
    public class AttackCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public Unit Target { get; private set; }
        public CommandPriority Priority => CommandPriority.Normal;
        public CommandTags Tags => CommandTags.Melee;

        public AttackCommand(Unit owner, Unit target)
        {
            Owner = owner;
            Target = target;
        }

        public IEnumerator Execute()
        {
            if (Target == null || !Target.gameObject.activeInHierarchy)
            {
                Debug.Log($"{Owner.unitName}'s attack failed (Target missing/dead).");
                yield break;
            }

            Debug.Log($"{Owner.unitName} attacks {Target.unitName}!");
            
            // TODO: Play Animation
            yield return new WaitForSeconds(0.5f); // Simulate animation time

            // Calculate Damage (Placeholder for now, will use Formula v2 later)
            int damage = 100; // Base
            bool isCrit = false;

            // Publish Event
            EventBus.Publish(new UnitDamagedEvent(Target, Owner, damage, isCrit));
            
            // Apply Damage Logic
            Target.TakeDamage(damage);
            
            Debug.Log($"{Target.unitName} takes {damage} damage!");
            yield return new WaitForSeconds(0.5f); // Post-hit delay
        }
    }
}

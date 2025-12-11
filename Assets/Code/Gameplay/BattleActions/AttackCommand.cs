using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.BattleActions
{
    public class AttackCommand : ICommand
    {
        public Unit Owner { get; private set; }
        public Unit Target { get; private set; }
        public Element attackElement { get; private set; }
        public CommandPriority Priority => CommandPriority.Normal;
        public CommandTags Tags => CommandTags.Melee;

        public AttackCommand(Unit owner, Unit target, Element element = Element.None)
        {
            Owner = owner;
            Target = target;
            attackElement = element;
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
            yield return new WaitForSeconds(0.5f); 

            // Calculate Damage
            int baseDamage = 100;
            float multiplier = 1.0f;

            // Apply Field Modifier
            if (FieldManager.Instance != null)
            {
                multiplier = FieldManager.Instance.GetDamageMultiplier(attackElement);
                if (multiplier != 1.0f)
                {
                    Debug.Log($"<color=yellow>Field Modifier Applied! x{multiplier}</color>");
                }
            }
            
            int damage = Mathf.RoundToInt(baseDamage * multiplier);
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

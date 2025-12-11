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

        // Modifiers
        public float DamageMultiplier { get; set; } = 1.0f;

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

            // --- DAMAGE CALCULATION ---
            // 1. Base ATK
            float atk = Owner.currentPower;
            float skillMult = 1.0f * DamageMultiplier; // Apply Combo/Bonus Multiplier here
            float rawDamage = atk * skillMult;

            // 2. Defense Mitigation
            float mitigation = Target.GetDefenseMitigation();
            float damageAfterDef = rawDamage * (1.0f - mitigation);

            // 3. Element Multiplier
            float elementMult = 1.0f;
            if (FieldManager.Instance != null)
            {
                elementMult = FieldManager.Instance.GetDamageMultiplier(attackElement);
                if (elementMult != 1.0f)
                {
                    Debug.Log($"<color=yellow>Field Modifier Applied! x{elementMult}</color>");
                }
            }
            
            // 4. Crit Multiplier (TODO: Luck stat)
            float critMult = 1.0f;
            bool isCrit = false;

            int finalDamage = Mathf.RoundToInt(damageAfterDef * elementMult * critMult);
            finalDamage = Mathf.Max(1, finalDamage);

            // Publish Event
            EventBus.Publish(new UnitDamagedEvent(Target, Owner, finalDamage, isCrit));
            
            // Apply Damage Logic
            Target.TakeDamage(finalDamage);
            
            Debug.Log($"{Target.unitName} takes {finalDamage} damage! (Mitigation: {mitigation*100:F1}%)");
            yield return new WaitForSeconds(0.5f);
        }
    }
}

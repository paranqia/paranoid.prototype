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
            
            yield return new WaitForSeconds(0.5f); 

            // --- DAMAGE CALCULATION ---
            float atk = Owner.currentPower;
            float skillMult = 1.0f * DamageMultiplier;
            float rawDamage = atk * skillMult;

            float mitigation = Target.GetDefenseMitigation();
            float damageAfterDef = rawDamage * (1.0f - mitigation);

            float elementMult = 1.0f;
            if (FieldManager.Instance != null)
            {
                elementMult = FieldManager.Instance.GetDamageMultiplier(attackElement);
            }
            
            // CRITICAL CALCULATION (Updated for Field/Luck)
            float critMult = 1.0f;
            bool isCrit = false;
            
            float chance = Owner.critChance; // Base + Field Mods
            if (Random.value < chance)
            {
                isCrit = true;
                critMult = Owner.critDamage; // 150% Default
                Debug.Log($"<color=red>CRITICAL HIT!</color>");
            }

            int finalDamage = Mathf.RoundToInt(damageAfterDef * elementMult * critMult);
            finalDamage = Mathf.Max(1, finalDamage);

            EventBus.Publish(new UnitDamagedEvent(Target, Owner, finalDamage, isCrit));
            Target.TakeDamage(finalDamage);
            
            Debug.Log($"{Target.unitName} takes {finalDamage} damage! (Mit: {mitigation*100:F0}%)");
            yield return new WaitForSeconds(0.5f);
        }
    }
}

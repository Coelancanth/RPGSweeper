using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Processor that applies resistance-based damage reduction
    /// </summary>
    public class ResistanceProcessor : IDamageProcessor
    {
        public DamageInfo Process(DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.Target == null)
            {
                return damageInfo;
            }
            
            // Get the appropriate resistance attribute type for this damage type
            AttributeType resistanceType = AttributeTypes.GetResistanceType(damageInfo.Type);
            if (resistanceType == null)
            {
                // No resistance type for this damage, no reduction
                return damageInfo;
            }
            
            // Get the resistance value from the target
            var resistanceAttr = damageInfo.Target.GetAttribute(resistanceType);
            if (resistanceAttr == null)
            {
                // No resistance attribute, no reduction
                return damageInfo;
            }
            
            // Calculate resistance reduction (0-100%)
            // Formula: Damage reduction = Resistance / (Resistance + 100)
            // This gives diminishing returns as resistance increases
            float resistanceValue = resistanceAttr.CurrentValue;
            damageInfo.ResistanceReduction = resistanceValue / (resistanceValue + 100f);
            
            // Apply the resistance reduction to the damage
            // Damage after resistance = Damage * (1 - Reduction%)
            damageInfo.ModifiedDamage *= (1f - damageInfo.ResistanceReduction);
            
            return damageInfo;
        }
    }
} 
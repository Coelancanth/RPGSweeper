using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Processor that handles critical hit calculation and damage modification
    /// </summary>
    public class CriticalHitProcessor : IDamageProcessor
    {
        public DamageInfo Process(DamageInfo damageInfo)
        {
            if (damageInfo == null)
            {
                return damageInfo;
            }
            
            // Skip critical hit calculations for non-player damage
            // (monsters don't make critical hits in our implementation)
            if (!damageInfo.IsPlayerDamage && damageInfo.Source is not PlayerEntity)
            {
                return damageInfo;
            }
            
            // Get critical chance from source entity if available
            float critChance = 0.05f; // Default 5% chance
            float critMultiplier = 1.5f; // Default 50% extra damage
            
            if (damageInfo.Source != null)
            {
                var critChanceAttr = damageInfo.Source.GetAttribute(AttributeTypes.CRITICAL_CHANCE);
                if (critChanceAttr != null)
                {
                    critChance = critChanceAttr.CurrentValue / 100f; // Convert from percentage to 0-1 range
                }
                
                var critDamageAttr = damageInfo.Source.GetAttribute(AttributeTypes.CRITICAL_DAMAGE);
                if (critDamageAttr != null)
                {
                    critMultiplier = critDamageAttr.CurrentValue / 100f; // Convert from percentage to multiplier
                }
            }
            
            // Roll for critical hit
            if (Random.value <= critChance)
            {
                damageInfo.IsCritical = true;
                damageInfo.CriticalMultiplier = critMultiplier;
                
                // Apply critical hit multiplier to damage
                damageInfo.ModifiedDamage *= damageInfo.CriticalMultiplier;
            }
            
            return damageInfo;
        }
    }
} 
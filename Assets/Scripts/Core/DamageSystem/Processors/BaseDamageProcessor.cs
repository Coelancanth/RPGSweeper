using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Processor that handles the initial damage calculation
    /// This should be the first processor in the pipeline
    /// </summary>
    public class BaseDamageProcessor : IDamageProcessor
    {
        public DamageInfo Process(DamageInfo damageInfo)
        {
            if (damageInfo == null)
            {
                Debug.LogWarning("Null damage info in BaseDamageProcessor");
                return damageInfo;
            }
            
            // If BaseDamage is already set, use that directly
            if (damageInfo.BaseDamage > 0)
            {
                // Copy base damage to modified damage as starting point
                damageInfo.ModifiedDamage = damageInfo.BaseDamage;
                return damageInfo;
            }
            
            // Otherwise, try to get base damage from source entity
            if (damageInfo.Source != null)
            {
                var baseDamageAttr = damageInfo.Source.GetAttribute(AttributeTypes.BASE_DAMAGE);
                if (baseDamageAttr != null)
                {
                    damageInfo.BaseDamage = baseDamageAttr.CurrentValue;
                    damageInfo.ModifiedDamage = damageInfo.BaseDamage;
                }
                else
                {
                    // Default fallback damage
                    damageInfo.BaseDamage = 1;
                    damageInfo.ModifiedDamage = 1;
                    Debug.LogWarning($"No base damage attribute found for entity {damageInfo.Source.Name}, using default");
                }
            }
            else if (damageInfo.IsPlayerDamage)
            {
                // Default player damage if no override was provided
                damageInfo.BaseDamage = 10;
                damageInfo.ModifiedDamage = 10;
            }
            else
            {
                // Default fallback for environmental damage
                damageInfo.BaseDamage = 1;
                damageInfo.ModifiedDamage = 1;
                Debug.LogWarning("No source entity or base damage in damage info, using default");
            }
            
            return damageInfo;
        }
    }
} 
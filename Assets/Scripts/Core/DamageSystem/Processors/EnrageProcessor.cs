using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Processor that handles enrage damage multipliers
    /// </summary>
    public class EnrageProcessor : IDamageProcessor
    {
        public DamageInfo Process(DamageInfo damageInfo)
        {
            if (damageInfo == null || !damageInfo.IsEnraged)
            {
                return damageInfo;
            }
            
            // Get the enrage multiplier from the source entity if available
            if (damageInfo.Source != null)
            {
                var enrageAttr = damageInfo.Source.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER);
                if (enrageAttr != null)
                {
                    damageInfo.EnrageMultiplier = enrageAttr.CurrentValue;
                }
                else
                {
                    // Default multiplier if not found on entity
                    damageInfo.EnrageMultiplier = 1.5f;
                }
            }
            
            // Apply the enrage multiplier to the damage
            damageInfo.ModifiedDamage *= damageInfo.EnrageMultiplier;
            
            return damageInfo;
        }
    }
} 
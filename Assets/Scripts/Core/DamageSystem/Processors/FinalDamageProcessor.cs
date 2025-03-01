using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Final processor in the damage calculation pipeline
    /// Ensures damage is never negative and copies modified damage to final damage
    /// </summary>
    public class FinalDamageProcessor : IDamageProcessor
    {
        public DamageInfo Process(DamageInfo damageInfo)
        {
            if (damageInfo == null)
            {
                return damageInfo;
            }
            
            // Ensure damage is never negative
            damageInfo.ModifiedDamage = Mathf.Max(0, damageInfo.ModifiedDamage);
            
            // Copy modified damage to final damage
            damageInfo.FinalDamage = damageInfo.ModifiedDamage;
            
            // We don't typically round damage in internal calculations to avoid precision loss,
            // but for display purposes it might be rounded later
            
            return damageInfo;
        }
    }
} 
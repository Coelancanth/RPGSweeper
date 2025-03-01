using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Processors
{
    /// <summary>
    /// Applies status effects associated with damage
    /// </summary>
    public class StatusEffectApplier : IDamageApplier
    {
        public bool ApplyDamage(DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.Target == null || damageInfo.StatusEffects.Count == 0)
            {
                return false;
            }
            
            bool anyEffectApplied = false;
            
            // Process each status effect
            foreach (var effectInfo in damageInfo.StatusEffects)
            {
                // Roll for chance to apply
                if (Random.value <= effectInfo.ApplyChance)
                {
                    // For now, we just log the effect application
                    // In a full implementation, we would create and apply the actual effect
                    Debug.Log($"Applied status effect {effectInfo.EffectType} to {damageInfo.Target.Name} " +
                              $"for {effectInfo.Duration}s with strength {effectInfo.Strength}");
                    
                    anyEffectApplied = true;
                    
                    // TODO: Implement actual status effect application logic
                    // This would typically involve adding a modifier to an attribute
                    // or adding a component to a game object
                }
            }
            
            return anyEffectApplied;
        }
    }
} 
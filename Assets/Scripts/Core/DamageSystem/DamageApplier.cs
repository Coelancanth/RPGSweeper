using System;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Applies damage to entities
    /// </summary>
    public class DamageApplier : IDamageApplier
    {
        private readonly IDamageCalculator _calculator;
        
        // Event for monitoring damage application
        public event Action<DamageInfo> OnDamageApplied;
        
        /// <summary>
        /// Creates a new damage applier with the specified calculator
        /// </summary>
        /// <param name="calculator">The calculator to use for damage calculation</param>
        public DamageApplier(IDamageCalculator calculator)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }
        
        /// <summary>
        /// Apply damage to the target entity
        /// </summary>
        public void ApplyDamage(DamageInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
                
            if (info.Target == null)
                return;
            
            // Calculate final damage if not already calculated
            if (info.FinalDamage <= 0 && _calculator != null)
            {
                info = _calculator.Calculate(info);
            }
            
            // Apply damage to HP
            Attribute hpAttribute = null;
            
            // Different handling for different entity types
            if (info.Target is MonsterEntity)
            {
                hpAttribute = info.Target.GetAttribute(AttributeTypes.CURRENT_HP);
            }
            else
            {
                // For now, assuming all entities use the same HP attribute type
                hpAttribute = info.Target.GetAttribute(AttributeTypes.CURRENT_HP);
            }
            
            if (hpAttribute != null)
            {
                float currentHp = hpAttribute.CurrentValue;
                float newHp = currentHp - info.FinalDamage;
                hpAttribute.SetBaseValue(newHp);
                
                // Check if target was defeated
                info.TargetDefeated = newHp <= 0;
            }
            
            // Notify listeners
            OnDamageApplied?.Invoke(info);
        }
    }
} 
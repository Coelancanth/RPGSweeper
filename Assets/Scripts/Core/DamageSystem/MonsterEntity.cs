using System;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Entity class specialized for monsters with additional functionality
    /// for handling monster-specific attributes and behaviors
    /// </summary>
    public class MonsterEntity : Entity
    {
        private bool _isEnraged = false;
        
        /// <summary>
        /// Creates a new monster entity with the given name
        /// </summary>
        /// <param name="name">The name of the monster entity</param>
        public MonsterEntity(string name) : base(name)
        {
        }
        
        /// <summary>
        /// Calculates and returns the current HP percentage of the monster
        /// </summary>
        /// <returns>HP percentage between 0 and 1</returns>
        public float GetHpPercentage()
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp == null || maxHp == null || maxHp.CurrentValue <= 0)
            {
                return 0f;
            }
            
            return Mathf.Clamp01(currentHp.CurrentValue / maxHp.CurrentValue);
        }
        
        /// <summary>
        /// Checks if the monster is currently enraged
        /// </summary>
        /// <returns>True if the monster is enraged, false otherwise</returns>
        public bool IsEnraged()
        {
            return _isEnraged;
        }
        
        /// <summary>
        /// Updates the enrage state based on current HP percentage
        /// </summary>
        /// <returns>True if the enrage state changed, false otherwise</returns>
        public bool UpdateEnrageState()
        {
            // Only check for enrage if the monster has an enrage multiplier attribute
            if (!HasAttribute(AttributeTypes.ENRAGE_MULTIPLIER))
            {
                return false;
            }
            
            bool wasEnraged = _isEnraged;
            float hpPercentage = GetHpPercentage();
            
            // Enrage when HP drops below 30%
            _isEnraged = hpPercentage <= 0.3f;
            
            // Return true if the enrage state changed
            return wasEnraged != _isEnraged;
        }
        
        /// <summary>
        /// Apply damage to this monster entity
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        /// <returns>Actual amount of damage applied</returns>
        public float ApplyDamage(float damage)
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            if (currentHp == null)
            {
                return 0f;
            }
            
            float oldHp = currentHp.CurrentValue;
            float newHp = Mathf.Max(0, oldHp - damage);
            
            currentHp.SetBaseValue(newHp);
            
            return oldHp - newHp; // Return actual damage dealt
        }
        
        /// <summary>
        /// Heal the monster by a specified amount
        /// </summary>
        /// <param name="amount">Amount to heal</param>
        /// <returns>Actual amount healed</returns>
        public float Heal(float amount)
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp == null || maxHp == null)
            {
                return 0f;
            }
            
            float oldHp = currentHp.CurrentValue;
            float newHp = Mathf.Min(maxHp.CurrentValue, oldHp + amount);
            
            currentHp.SetBaseValue(newHp);
            
            return newHp - oldHp; // Return actual amount healed
        }
    }
} 
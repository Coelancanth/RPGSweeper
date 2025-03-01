using System;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Entity representing a monster with attributes
    /// </summary>
    public class MonsterEntity : Entity, IDamageSource, IDamageTarget
    {
        private bool _isEnraged = false;
        
        /// <summary>
        /// Creates a new monster entity
        /// </summary>
        /// <param name="name">The name of the monster</param>
        public MonsterEntity(string name = "Monster") 
            : base(name)
        {
        }
        
        /// <summary>
        /// Get the HP percentage of this monster
        /// </summary>
        public float GetHpPercentage()
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp == null || maxHp == null || maxHp.CurrentValue == 0)
                return 0;
                
            return currentHp.CurrentValue / maxHp.CurrentValue;
        }
        
        /// <summary>
        /// Check if the monster should be enraged based on HP percentage
        /// </summary>
        /// <param name="enrageThreshold">The HP percentage threshold for enrage (default 0.3 = 30%)</param>
        /// <returns>True if the enrage state changed, false otherwise</returns>
        public bool UpdateEnrageState(float enrageThreshold = 0.3f)
        {
            // Check if this monster has the enrage multiplier attribute
            if (!HasAttribute(AttributeTypes.ENRAGE_MULTIPLIER))
                return false;
                
            bool shouldBeEnraged = GetHpPercentage() <= enrageThreshold;
            
            if (shouldBeEnraged && !_isEnraged)
            {
                _isEnraged = true;
                return true; // Indicates state changed
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if monster is currently enraged
        /// </summary>
        public bool IsEnraged()
        {
            return _isEnraged;
        }
        
        /// <summary>
        /// Check if the monster is defeated
        /// </summary>
        public bool IsDefeated()
        {
            var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
            return currentHp == null || currentHp.CurrentValue <= 0;
        }
        
        /// <summary>
        /// Create damage info targeting an entity
        /// </summary>
        public DamageInfo CreateDamageInfo(IEntity target)
        {
            return DamageInfo.Factory.CreateMonsterDamage(this, target);
        }
        
        /// <summary>
        /// Deal damage to a target using the damage system
        /// </summary>
        public void DealDamageTo(IEntity target)
        {
            var damageInfo = CreateDamageInfo(target);
            DamageSystem.CalculateAndApplyDamage(damageInfo);
        }
        
        /// <summary>
        /// Receive damage from a damage info
        /// </summary>
        public void ReceiveDamage(DamageInfo damageInfo)
        {
            // Damage info is already targeted at this entity
            DamageSystem.ApplyDamage(damageInfo);
            
            // Update enrage state
            UpdateEnrageState();
        }
    }
} 
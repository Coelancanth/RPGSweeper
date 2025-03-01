using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Factory for creating standardized DamageInfo objects for different scenarios
    /// </summary>
    public static class DamageInfoFactory
    {
        /// <summary>
        /// Create standard monster-to-player damage info
        /// </summary>
        /// <param name="monsterEntity">Source monster entity</param>
        /// <param name="playerComponent">Target player component (optional)</param>
        /// <param name="damageType">Type of damage being dealt</param>
        /// <returns>Configured DamageInfo object</returns>
        public static DamageInfo CreateMonsterToPlayerDamage(
            MonsterEntity monsterEntity, 
            PlayerComponent playerComponent = null, 
            DamageType damageType = DamageType.Physical)
        {
            // Early validation
            if (monsterEntity == null)
            {
                Debug.LogWarning("Null monster entity in CreateMonsterToPlayerDamage");
                return null;
            }
            
            // Get base damage from monster entity
            float baseDamage = monsterEntity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0;
            
            // Create damage info
            var damageInfo = new DamageInfo
            {
                Source = monsterEntity,
                Target = null, // No direct entity target for PlayerComponent
                Type = damageType,
                BaseDamage = baseDamage,
                IsEnraged = monsterEntity.IsEnraged()
            };
            
            return damageInfo;
        }
        
        /// <summary>
        /// Create damage info for player attacking a monster
        /// </summary>
        /// <param name="playerComponent">Source player component</param>
        /// <param name="targetEntity">Target monster entity</param>
        /// <param name="damageAmount">Override damage amount (0 means use player's attack attribute)</param>
        /// <param name="damageType">Type of damage to deal</param>
        /// <returns>Configured DamageInfo object</returns>
        public static DamageInfo CreatePlayerToMonsterDamage(
            PlayerComponent playerComponent,
            MonsterEntity targetEntity,
            float damageAmount = 0,
            DamageType damageType = DamageType.Physical)
        {
            // Early validation
            if (targetEntity == null)
            {
                Debug.LogWarning("Null target entity in CreatePlayerToMonsterDamage");
                return null;
            }
            
            // Create damage info
            var damageInfo = new DamageInfo
            {
                Source = null, // No direct entity for PlayerComponent
                Target = targetEntity,
                Type = damageType,
                BaseDamage = damageAmount, // Use provided amount or will be set by processor
                IsPlayerDamage = true
            };
            
            return damageInfo;
        }
        
        /// <summary>
        /// Create damage info for entity-to-entity damage
        /// </summary>
        /// <param name="sourceEntity">Source entity</param>
        /// <param name="targetEntity">Target entity</param>
        /// <param name="damageAmount">Override damage amount (0 means use source's attack attribute)</param>
        /// <param name="damageType">Type of damage to deal</param>
        /// <returns>Configured DamageInfo object</returns>
        public static DamageInfo CreateEntityToEntityDamage(
            Entity sourceEntity,
            Entity targetEntity,
            float damageAmount = 0,
            DamageType damageType = DamageType.Physical)
        {
            // Early validation
            if (sourceEntity == null || targetEntity == null)
            {
                Debug.LogWarning("Null entity in CreateEntityToEntityDamage");
                return null;
            }
            
            // Get base damage from source entity if not specified
            if (damageAmount <= 0)
            {
                damageAmount = sourceEntity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0;
            }
            
            // Create damage info
            var damageInfo = new DamageInfo
            {
                Source = sourceEntity,
                Target = targetEntity,
                Type = damageType,
                BaseDamage = damageAmount,
                IsEnraged = (sourceEntity is MonsterEntity monster) && monster.IsEnraged()
            };
            
            return damageInfo;
        }
    }
} 
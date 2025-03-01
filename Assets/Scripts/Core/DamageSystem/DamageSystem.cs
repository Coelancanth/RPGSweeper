using System;
using System.Collections.Generic;
using UnityEngine;
using Minesweeper.Core.DamageSystem.Processors;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Central system for calculating and applying damage in the game.
    /// Provides a consistent interface for all damage-related operations.
    /// </summary>
    public static class DamageSystem
    {
        // The chain of damage calculation processors
        private static readonly List<IDamageProcessor> DamageProcessors = new List<IDamageProcessor>
        {
            new BaseDamageProcessor(),
            new EnrageProcessor(),
            new ResistanceProcessor(),
            new CriticalHitProcessor(),
            new FinalDamageProcessor()
        };
        
        // The chain of damage application processors
        private static readonly List<Processors.IDamageApplier> DamageAppliers = new List<Processors.IDamageApplier>
        {
            new BasicDamageApplier(),
            new StatusEffectApplier()
        };
        
        /// <summary>
        /// Calculate damage based on source and other factors without applying it
        /// </summary>
        /// <param name="damageInfo">Information about the damage to calculate</param>
        /// <returns>Updated damage info with calculated values</returns>
        public static DamageInfo CalculateDamage(DamageInfo damageInfo)
        {
            // Process the damage through each processor in the chain
            foreach (var processor in DamageProcessors)
            {
                damageInfo = processor.Process(damageInfo);
            }
            
            return damageInfo;
        }
        
        /// <summary>
        /// Apply pre-calculated damage to a target entity
        /// </summary>
        /// <param name="damageInfo">Information about the damage to apply</param>
        /// <returns>True if damage was applied successfully, false otherwise</returns>
        public static bool ApplyDamage(DamageInfo damageInfo)
        {
            // Early exit if info or target is missing
            if (damageInfo == null || damageInfo.Target == null)
            {
                Debug.LogWarning("Invalid damage info or target in ApplyDamage");
                return false;
            }
            
            bool success = false;
            
            // Apply damage through each applier in the chain
            foreach (var applier in DamageAppliers)
            {
                bool result = applier.ApplyDamage(damageInfo);
                success = success || result; // Use logical OR instead of |=
            }
            
            return success;
        }
        
        /// <summary>
        /// Calculate and apply damage in a single operation
        /// </summary>
        /// <param name="damageInfo">Information about the damage</param>
        /// <returns>The final damage info after calculation and application</returns>
        public static DamageInfo CalculateAndApplyDamage(DamageInfo damageInfo)
        {
            // Calculate the damage
            damageInfo = CalculateDamage(damageInfo);
            
            // Apply the damage
            ApplyDamage(damageInfo);
            
            return damageInfo;
        }
        
        /// <summary>
        /// Create a damage info from one entity to another
        /// </summary>
        /// <param name="source">Source of the damage</param>
        /// <param name="target">Target of the damage</param>
        /// <param name="type">Type of damage</param>
        /// <param name="baseDamage">Base damage amount (0 means use entity's base damage)</param>
        /// <returns>A new DamageInfo object</returns>
        public static DamageInfo CreateDamageInfo(Entity source, Entity target, DamageType type = DamageType.Physical, float baseDamage = 0)
        {
            var damageInfo = new DamageInfo
            {
                Source = source,
                Target = target,
                Type = type,
                IsEnraged = (source is MonsterEntity monster) && monster.IsEnraged()
            };
            
            // Use provided base damage or get from source entity
            if (baseDamage > 0)
            {
                damageInfo.BaseDamage = baseDamage;
            }
            else
            {
                var baseAttr = source?.GetAttribute(AttributeTypes.BASE_DAMAGE);
                damageInfo.BaseDamage = baseAttr?.CurrentValue ?? 0;
            }
            
            return damageInfo;
        }
        
        /// <summary>
        /// Deal damage from a monster to a player component
        /// </summary>
        /// <param name="monsterEntity">Source monster entity</param>
        /// <param name="playerComponent">Target player component</param>
        /// <param name="damageAmount">Optional explicit damage amount (0 means use monster's base damage)</param>
        /// <returns>The final damage dealt</returns>
        public static float DealMonsterDamageToPlayer(MonsterEntity monsterEntity, PlayerComponent playerComponent, float damageAmount = 0)
        {
            if (monsterEntity == null || playerComponent == null)
            {
                Debug.LogWarning("Invalid monster or player in DealMonsterDamageToPlayer");
                return 0;
            }
            
            // Create damage info
            var damageInfo = new DamageInfo
            {
                Source = monsterEntity,
                Type = DamageType.Physical,
                BaseDamage = damageAmount > 0 ? damageAmount : monsterEntity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0,
                IsEnraged = monsterEntity.IsEnraged()
            };
            
            // Calculate the damage
            damageInfo = CalculateDamage(damageInfo);
            
            // Apply to player
            playerComponent.TakeDamage(Mathf.RoundToInt(damageInfo.FinalDamage));
            
            return damageInfo.FinalDamage;
        }
    }
} 
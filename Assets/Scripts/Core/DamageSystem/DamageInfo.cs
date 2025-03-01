using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Contains all information about a damage instance, including source, target, damage type,
    /// and calculated values throughout the damage pipeline.
    /// </summary>
    public class DamageInfo
    {
        #region Source and Target Information
        /// <summary>
        /// Entity causing the damage, can be null for environmental damage
        /// </summary>
        public Entity Source { get; set; }
        
        /// <summary>
        /// Entity receiving the damage
        /// </summary>
        public Entity Target { get; set; }
        
        /// <summary>
        /// Type of damage being dealt
        /// </summary>
        public DamageType Type { get; set; } = DamageType.Physical;
        #endregion
        
        #region Damage Calculation Values
        /// <summary>
        /// Base damage value before any modifiers
        /// </summary>
        public float BaseDamage { get; set; }
        
        /// <summary>
        /// Damage after applying modifiers but before applying resistances
        /// </summary>
        public float ModifiedDamage { get; set; }
        
        /// <summary>
        /// Final damage amount after all calculations
        /// </summary>
        public float FinalDamage { get; set; }
        
        /// <summary>
        /// Flat damage addition to be applied (added to BaseDamage)
        /// </summary>
        public float DamageAddition { get; set; }
        
        /// <summary>
        /// Damage multiplier to be applied to modified damage
        /// </summary>
        public float DamageMultiplier { get; set; } = 1.0f;
        
        /// <summary>
        /// Resistance-based damage multiplier (1.0 = no resistance)
        /// </summary>
        public float ResistanceMultiplier { get; set; } = 1.0f;
        
        /// <summary>
        /// Whether the damage is a critical hit
        /// </summary>
        public bool IsCritical { get; set; }
        
        /// <summary>
        /// Critical damage multiplier if this is a critical hit
        /// </summary>
        public float CriticalMultiplier { get; set; } = 1.5f;
        
        /// <summary>
        /// Whether the source is in an enraged state
        /// </summary>
        public bool IsEnraged { get; set; }
        
        /// <summary>
        /// Enrage damage multiplier if source is enraged
        /// </summary>
        public float EnrageMultiplier { get; set; } = 1.0f;
        
        /// <summary>
        /// Damage reduction from target's resistance, as a percentage (0-1)
        /// </summary>
        public float ResistanceReduction { get; set; }
        
        /// <summary>
        /// Whether this damage is from a player (special rules may apply)
        /// </summary>
        public bool IsPlayerDamage { get; set; }
        
        /// <summary>
        /// Indicates if the target was defeated by this damage
        /// </summary>
        public bool TargetDefeated { get; set; }
        #endregion
        
        #region Status Effect Information
        /// <summary>
        /// Additional status effects that may be applied with this damage
        /// </summary>
        public List<StatusEffectInfo> StatusEffects { get; } = new List<StatusEffectInfo>();
        #endregion
        
        #region Metadata
        /// <summary>
        /// Dictionary to store additional data that may be used by custom processors
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
        #endregion
        
        /// <summary>
        /// Creates a copy of this damage info
        /// </summary>
        public DamageInfo Clone()
        {
            var clone = new DamageInfo
            {
                Source = Source,
                Target = Target,
                Type = Type,
                BaseDamage = BaseDamage,
                ModifiedDamage = ModifiedDamage,
                FinalDamage = FinalDamage,
                DamageAddition = DamageAddition,
                DamageMultiplier = DamageMultiplier,
                ResistanceMultiplier = ResistanceMultiplier,
                IsCritical = IsCritical,
                CriticalMultiplier = CriticalMultiplier,
                IsEnraged = IsEnraged,
                EnrageMultiplier = EnrageMultiplier,
                ResistanceReduction = ResistanceReduction,
                IsPlayerDamage = IsPlayerDamage,
                TargetDefeated = TargetDefeated
            };
            
            // Copy status effects
            foreach (var effect in StatusEffects)
            {
                clone.StatusEffects.Add(effect);
            }
            
            // Copy metadata
            foreach (var pair in Metadata)
            {
                clone.Metadata[pair.Key] = pair.Value;
            }
            
            return clone;
        }
        
        /// <summary>
        /// Factory class for creating specific damage info types
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a damage info for a monster attacking a target
            /// </summary>
            public static DamageInfo CreateMonsterDamage(MonsterEntity monster, IEntity player)
            {
                var info = new DamageInfo
                {
                    Source = monster,
                    Target = player as Entity,
                    Type = DamageType.Physical,
                    BaseDamage = monster.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0,
                    IsEnraged = monster.IsEnraged()
                };
                
                return info;
            }
            
            /// <summary>
            /// Creates a damage info for a player attacking a monster
            /// </summary>
            public static DamageInfo CreatePlayerDamage(IEntity player, MonsterEntity monster, float damageAmount)
            {
                var info = new DamageInfo
                {
                    Source = player as Entity,
                    Target = monster,
                    Type = DamageType.Physical,
                    BaseDamage = damageAmount
                };
                
                return info;
            }
        }
    }
    
    /// <summary>
    /// Information about a status effect that may be applied with damage
    /// </summary>
    public class StatusEffectInfo
    {
        /// <summary>
        /// Type of status effect
        /// </summary>
        public string EffectType { get; set; }
        
        /// <summary>
        /// Duration of the effect in seconds
        /// </summary>
        public float Duration { get; set; }
        
        /// <summary>
        /// Strength/intensity of the effect
        /// </summary>
        public float Strength { get; set; }
        
        /// <summary>
        /// Chance to apply the effect (0-1)
        /// </summary>
        public float ApplyChance { get; set; } = 1.0f;
    }
} 
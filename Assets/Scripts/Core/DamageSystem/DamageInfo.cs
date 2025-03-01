using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Contains all information needed for damage calculation
    /// </summary>
    public class DamageInfo
    {
        // Core properties
        public IEntity Source { get; set; }          // Who's dealing the damage
        public IEntity Target { get; set; }          // Who's receiving the damage
        public DamageType Type { get; set; }        // Physical, magical, etc.
        public float BaseDamage { get; set; }       // Starting damage value
        
        // Calculation modifiers
        public float DamageMultiplier { get; set; } = 1.0f;
        public float DamageAddition { get; set; } = 0.0f;
        public float ResistanceMultiplier { get; set; } = 1.0f;
        
        // Special state flags
        public bool IsCritical { get; set; } = false;
        public bool IsEnraged { get; set; } = false;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        
        // Results (filled after calculation)
        public float FinalDamage { get; set; }
        public bool WasEvaded { get; set; } = false;
        public bool TargetDefeated { get; set; } = false;
        
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
                    Target = player,
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
                    Source = player,
                    Target = monster,
                    Type = DamageType.Physical,
                    BaseDamage = damageAmount
                };
                
                return info;
            }
        }
    }
} 
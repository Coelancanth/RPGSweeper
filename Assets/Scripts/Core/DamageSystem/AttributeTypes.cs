using System.Collections.Generic;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Static class providing predefined attribute types used throughout the damage system
    /// to ensure consistent naming and lookup
    /// </summary>
    public static class AttributeTypes
    {
        // Core attributes for all entities
        public static readonly AttributeType MAX_HP = new AttributeType("MaxHP");
        public static readonly AttributeType CURRENT_HP = new AttributeType("CurrentHP");
        public static readonly AttributeType BASE_DAMAGE = new AttributeType("BaseDamage");
        
        // Monster-specific attributes
        public static readonly AttributeType DAMAGE_PER_HIT = new AttributeType("DamagePerHit");
        public static readonly AttributeType ENRAGE_MULTIPLIER = new AttributeType("EnrageMultiplier");
        
        // Resistances and vulnerabilities
        public static readonly AttributeType PHYSICAL_RESISTANCE = new AttributeType("PhysicalResistance");
        public static readonly AttributeType MAGICAL_RESISTANCE = new AttributeType("MagicalResistance");
        public static readonly AttributeType FIRE_RESISTANCE = new AttributeType("FireResistance");
        public static readonly AttributeType ICE_RESISTANCE = new AttributeType("IceResistance");
        public static readonly AttributeType POISON_RESISTANCE = new AttributeType("PoisonResistance");
        
        // Player-specific attributes
        public static readonly AttributeType ATTACK_POWER = new AttributeType("AttackPower");
        public static readonly AttributeType DEFENSE = new AttributeType("Defense");
        public static readonly AttributeType CRITICAL_CHANCE = new AttributeType("CriticalChance");
        public static readonly AttributeType CRITICAL_DAMAGE = new AttributeType("CriticalDamage");
        
        // Status effect modifiers
        public static readonly AttributeType POISON_IMMUNITY = new AttributeType("PoisonImmunity");
        public static readonly AttributeType STUN_RESISTANCE = new AttributeType("StunResistance");
        public static readonly AttributeType BURN_RESISTANCE = new AttributeType("BurnResistance");

        // Collection of all resistance types
        public static readonly Dictionary<DamageType, AttributeType> ResistanceTypes = 
            new Dictionary<DamageType, AttributeType>
            {
                { DamageType.Physical, PHYSICAL_RESISTANCE },
                { DamageType.Magical, MAGICAL_RESISTANCE },
                { DamageType.Fire, FIRE_RESISTANCE },
                { DamageType.Ice, ICE_RESISTANCE }
            };
            
        /// <summary>
        /// Gets the appropriate resistance attribute type for a damage type
        /// </summary>
        /// <param name="damageType">The type of damage</param>
        /// <returns>The corresponding resistance attribute type, or null if none exists</returns>
        public static AttributeType GetResistanceType(DamageType damageType)
        {
            if (ResistanceTypes.TryGetValue(damageType, out var resistanceType))
            {
                return resistanceType;
            }
            
            return null;
        }
    }
} 
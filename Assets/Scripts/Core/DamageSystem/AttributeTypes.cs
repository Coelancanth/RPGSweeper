using System.Collections.Generic;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Central registry for all attribute types
    /// </summary>
    public static class AttributeTypes
    {
        // Core attributes
        public static readonly AttributeType MAX_HP = new AttributeType("MaxHP");
        public static readonly AttributeType CURRENT_HP = new AttributeType("CurrentHP");
        
        // Damage attributes
        public static readonly AttributeType BASE_DAMAGE = new AttributeType("BaseDamage");
        public static readonly AttributeType DAMAGE_PER_HIT = new AttributeType("DamagePerHit");
        
        // Special attributes
        public static readonly AttributeType ENRAGE_MULTIPLIER = new AttributeType("EnrageMultiplier");
        
        // Resistance attributes
        public static readonly AttributeType PHYSICAL_RESISTANCE = new AttributeType("PhysicalResistance");
        public static readonly AttributeType MAGICAL_RESISTANCE = new AttributeType("MagicalResistance");
        public static readonly AttributeType FIRE_RESISTANCE = new AttributeType("FireResistance");
        public static readonly AttributeType ICE_RESISTANCE = new AttributeType("IceResistance");
        
        // Collection of all resistance types
        public static readonly Dictionary<DamageType, AttributeType> ResistanceTypes = 
            new Dictionary<DamageType, AttributeType>
            {
                { DamageType.Physical, PHYSICAL_RESISTANCE },
                { DamageType.Magical, MAGICAL_RESISTANCE },
                { DamageType.Fire, FIRE_RESISTANCE },
                { DamageType.Ice, ICE_RESISTANCE }
            };
    }
} 
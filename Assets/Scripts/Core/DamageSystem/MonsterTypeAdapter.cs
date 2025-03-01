using System;
using System.Collections.Generic;
using Minesweeper.Core.DamageSystem.Initializers;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Adapter class to convert between game MonsterType and AttributeInitializerFactory.MonsterType
    /// </summary>
    public static class MonsterTypeAdapter
    {
        // Dictionary to map RPGMinesweeper.MonsterType to AttributeInitializerFactory.MonsterType
        private static readonly Dictionary<RPGMinesweeper.MonsterType, AttributeInitializerFactory.MonsterType> TypeMap = 
            new Dictionary<RPGMinesweeper.MonsterType, AttributeInitializerFactory.MonsterType>
            {
                { RPGMinesweeper.MonsterType.None, AttributeInitializerFactory.MonsterType.Default },
                { RPGMinesweeper.MonsterType.Slime, AttributeInitializerFactory.MonsterType.Slime },
                { RPGMinesweeper.MonsterType.Spider, AttributeInitializerFactory.MonsterType.Spider },
                { RPGMinesweeper.MonsterType.Dragon, AttributeInitializerFactory.MonsterType.Dragon },
                // Add more mappings as needed
            };
        
        /// <summary>
        /// Convert from RPGMinesweeper.MonsterType to AttributeInitializerFactory.MonsterType
        /// </summary>
        public static AttributeInitializerFactory.MonsterType Convert(RPGMinesweeper.MonsterType type)
        {
            if (TypeMap.TryGetValue(type, out var result))
            {
                return result;
            }
            
            // Default fallback
            return AttributeInitializerFactory.MonsterType.Default;
        }
        
        /// <summary>
        /// Get the appropriate attribute initializer for a monster type
        /// </summary>
        public static IAttributeInitializer GetInitializerForType(RPGMinesweeper.MonsterType type)
        {
            var convertedType = Convert(type);
            return AttributeInitializerFactory.CreateMonsterInitializer(convertedType);
        }
        
        /// <summary>
        /// Create a fully initialized monster entity for a game monster type
        /// </summary>
        public static MonsterEntity CreateMonsterEntity(RPGMinesweeper.MonsterType type, string name = null)
        {
            var convertedType = Convert(type);
            return AttributeInitializerFactory.CreateMonsterEntity(convertedType, name);
        }
    }
} 
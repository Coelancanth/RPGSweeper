using System;

namespace Minesweeper.Core.DamageSystem.Initializers
{
    /// <summary>
    /// Factory for creating attribute initializers
    /// </summary>
    public static class AttributeInitializerFactory
    {
        /// <summary>
        /// Monster types that can be created
        /// </summary>
        public enum MonsterType
        {
            Default,
            Slime,
            Spider,
            Dragon
        }
        
        /// <summary>
        /// Create an initializer for a specific monster type
        /// </summary>
        /// <param name="monsterType">The type of monster to create an initializer for</param>
        /// <returns>An attribute initializer for the specified monster type</returns>
        public static IAttributeInitializer CreateMonsterInitializer(MonsterType monsterType)
        {
            switch (monsterType)
            {
                case MonsterType.Slime:
                    return new SlimeAttributeInitializer();
                case MonsterType.Spider:
                    return new SpiderAttributeInitializer();
                case MonsterType.Dragon:
                    return new DragonAttributeInitializer();
                default:
                    return new DefaultMonsterAttributeInitializer();
            }
        }
        
        /// <summary>
        /// Create a monster entity with the specified type
        /// </summary>
        /// <param name="monsterType">The type of monster to create</param>
        /// <param name="name">The name for the monster (optional)</param>
        /// <returns>A fully initialized monster entity</returns>
        public static MonsterEntity CreateMonsterEntity(MonsterType monsterType, string name = null)
        {
            // Create default name if none provided
            string monsterName = name ?? $"Monster_{monsterType}";
            
            // Create the entity
            var entity = new MonsterEntity(monsterName);
            
            // Initialize attributes
            var initializer = CreateMonsterInitializer(monsterType);
            initializer.InitializeAttributes(entity);
            
            return entity;
        }
    }
} 
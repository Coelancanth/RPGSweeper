using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Defines different types of attributes that entities can have.
    /// Uses a flyweight pattern to ensure each attribute type is a singleton.
    /// </summary>
    public class AttributeType
    {
        // Common attribute types
        public static readonly AttributeType MaxHealth = new AttributeType("MaxHealth");
        public static readonly AttributeType CurrentHealth = new AttributeType("CurrentHealth");
        public static readonly AttributeType Attack = new AttributeType("Attack");
        public static readonly AttributeType Defense = new AttributeType("Defense");
        public static readonly AttributeType CriticalChance = new AttributeType("CriticalChance");
        public static readonly AttributeType CriticalDamage = new AttributeType("CriticalDamage");
        public static readonly AttributeType Speed = new AttributeType("Speed");
        public static readonly AttributeType MagicPower = new AttributeType("MagicPower");
        public static readonly AttributeType MagicResistance = new AttributeType("MagicResistance");
        public static readonly AttributeType Experience = new AttributeType("Experience");
        public static readonly AttributeType Level = new AttributeType("Level");
        
        // Dictionary to store all created attribute types
        private static readonly Dictionary<string, AttributeType> s_Types = new Dictionary<string, AttributeType>();
        
        /// <summary>
        /// The unique identifier for this attribute type
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// Private constructor to enforce the flyweight pattern
        /// </summary>
        /// <param name="id">The unique identifier for this attribute type</param>
        private AttributeType(string id)
        {
            Id = id;
            s_Types[id] = this;
        }
        
        /// <summary>
        /// Gets an existing attribute type by ID or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="id">The unique identifier for the attribute type</param>
        /// <returns>The attribute type with the specified ID</returns>
        public static AttributeType GetOrCreate(string id)
        {
            if (s_Types.TryGetValue(id, out var type))
            {
                return type;
            }
            
            return new AttributeType(id);
        }
        
        /// <summary>
        /// Checks if an attribute type with the specified ID exists
        /// </summary>
        /// <param name="id">The ID to check</param>
        /// <returns>True if the attribute type exists, false otherwise</returns>
        public static bool Exists(string id)
        {
            return s_Types.ContainsKey(id);
        }
        
        /// <summary>
        /// Gets all registered attribute types
        /// </summary>
        /// <returns>An enumerable of all attribute types</returns>
        public static IEnumerable<AttributeType> GetAll()
        {
            return s_Types.Values;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is AttributeType other)
            {
                return Id == other.Id;
            }
            
            return false;
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public override string ToString()
        {
            return Id;
        }
    }
} 
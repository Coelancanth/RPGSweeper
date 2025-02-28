using System;
using UnityEngine;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Represents a modifier that can be applied to an attribute
    /// </summary>
    public class AttributeModifier
    {
        /// <summary>
        /// Unique identifier for this modifier
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();
        
        /// <summary>
        /// The type of attribute this modifier affects
        /// </summary>
        public AttributeType AttributeType { get; }
        
        /// <summary>
        /// The value of the modification
        /// </summary>
        public float Value { get; }
        
        /// <summary>
        /// How the modifier is applied (flat, percent, etc.)
        /// </summary>
        public ModifierType Type { get; }
        
        /// <summary>
        /// The source of the modifier (e.g., a buff, item, or ability)
        /// </summary>
        public object Source { get; }
        
        /// <summary>
        /// Priority determines the order in which modifiers are applied
        /// Higher priority modifiers are applied last
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// Creates a new attribute modifier
        /// </summary>
        /// <param name="attributeType">The type of attribute to modify</param>
        /// <param name="value">The value of the modification</param>
        /// <param name="type">How the modifier is applied</param>
        /// <param name="source">The source of the modifier</param>
        /// <param name="priority">The priority of the modifier (higher = applied later)</param>
        public AttributeModifier(AttributeType attributeType, float value, ModifierType type, object source, int priority = 0)
        {
            AttributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
            Value = value;
            Type = type;
            Source = source;
            Priority = priority;
        }
        
        /// <summary>
        /// Creates a flat modifier
        /// </summary>
        /// <param name="attributeType">The type of attribute to modify</param>
        /// <param name="value">The flat value to add</param>
        /// <param name="source">The source of the modifier</param>
        /// <param name="priority">The priority of the modifier</param>
        /// <returns>A new flat attribute modifier</returns>
        public static AttributeModifier CreateFlat(AttributeType attributeType, float value, object source, int priority = 0)
        {
            return new AttributeModifier(attributeType, value, ModifierType.Flat, source, priority);
        }
        
        /// <summary>
        /// Creates a percentage modifier
        /// </summary>
        /// <param name="attributeType">The type of attribute to modify</param>
        /// <param name="percentValue">The percentage value to add (e.g., 0.1 for 10%)</param>
        /// <param name="source">The source of the modifier</param>
        /// <param name="priority">The priority of the modifier</param>
        /// <returns>A new percentage attribute modifier</returns>
        public static AttributeModifier CreatePercent(AttributeType attributeType, float percentValue, object source, int priority = 0)
        {
            return new AttributeModifier(attributeType, percentValue, ModifierType.Percent, source, priority);
        }
        
        /// <summary>
        /// Creates a multiplier modifier
        /// </summary>
        /// <param name="attributeType">The type of attribute to modify</param>
        /// <param name="multiplier">The multiplier value (e.g., 1.5 for 50% increase)</param>
        /// <param name="source">The source of the modifier</param>
        /// <param name="priority">The priority of the modifier</param>
        /// <returns>A new multiplier attribute modifier</returns>
        public static AttributeModifier CreateMultiplier(AttributeType attributeType, float multiplier, object source, int priority = 0)
        {
            return new AttributeModifier(attributeType, multiplier, ModifierType.Multiplier, source, priority);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is AttributeModifier other)
            {
                return Id.Equals(other.Id);
            }
            
            return false;
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public override string ToString()
        {
            string valueStr = Type == ModifierType.Percent ? $"{Value * 100}%" : Value.ToString();
            string typeStr = Type.ToString();
            
            return $"{AttributeType.Id} {typeStr} {valueStr} (Priority: {Priority})";
        }
    }
} 
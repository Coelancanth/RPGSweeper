using System;
using System.Collections.Generic;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Interface for objects that have attributes
    /// </summary>
    public interface IAttributeEntity
    {
        /// <summary>
        /// Gets the underlying entity object
        /// </summary>
        Entity Entity { get; }
        
        /// <summary>
        /// Gets an attribute by type
        /// </summary>
        /// <param name="type">The type of attribute to get</param>
        /// <returns>The attribute, or null if not found</returns>
        Attribute GetAttribute(AttributeType type);
        
        /// <summary>
        /// Sets the base value of an attribute
        /// </summary>
        /// <param name="type">The type of attribute to set</param>
        /// <param name="value">The new base value</param>
        /// <returns>The attribute that was set</returns>
        Attribute SetAttribute(AttributeType type, float value);
        
        /// <summary>
        /// Adds a modifier to an attribute
        /// </summary>
        /// <param name="modifier">The modifier to add</param>
        /// <returns>True if the modifier was added, false otherwise</returns>
        bool AddModifier(AttributeModifier modifier);
        
        /// <summary>
        /// Removes a modifier from an attribute
        /// </summary>
        /// <param name="modifier">The modifier to remove</param>
        /// <returns>True if the modifier was removed, false otherwise</returns>
        bool RemoveModifier(AttributeModifier modifier);
        
        /// <summary>
        /// Removes all modifiers from a specific source
        /// </summary>
        /// <param name="source">The source of the modifiers to remove</param>
        /// <returns>The number of modifiers removed</returns>
        int RemoveModifiersFromSource(object source);
        
        /// <summary>
        /// Applies a buff to this entity
        /// </summary>
        /// <param name="buff">The buff to apply</param>
        void ApplyBuff(IBuff buff);
        
        /// <summary>
        /// Removes a buff from this entity
        /// </summary>
        /// <param name="buff">The buff to remove</param>
        void RemoveBuff(IBuff buff);
        
        /// <summary>
        /// Called at the end of each turn to update attributes and modifiers
        /// </summary>
        void OnTurnEnd();
    }
    
    /// <summary>
    /// Interface for buffs that can be applied to entities
    /// </summary>
    public interface IBuff
    {
        /// <summary>
        /// Gets the name of this buff
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets whether this buff is active
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Gets all modifiers associated with this buff
        /// </summary>
        IEnumerable<AttributeModifier> Modifiers { get; }
        
        /// <summary>
        /// Applies this buff to an entity
        /// </summary>
        /// <param name="target">The entity to apply the buff to</param>
        void Apply(Entity target);
        
        /// <summary>
        /// Removes this buff from an entity
        /// </summary>
        /// <param name="target">The entity to remove the buff from</param>
        void Remove(Entity target);
        
        /// <summary>
        /// Updates this buff at the end of a turn
        /// </summary>
        /// <param name="target">The entity with the buff</param>
        /// <returns>True if the buff is still active, false if it should be removed</returns>
        bool OnTurnEnd(Entity target);
    }
} 
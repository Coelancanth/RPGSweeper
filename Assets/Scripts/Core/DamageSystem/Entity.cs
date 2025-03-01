using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Base class for all entities with attributes in the game.
    /// </summary>
    public class Entity : IEntity
    {
        private readonly Dictionary<AttributeType, Attribute> _attributes = new Dictionary<AttributeType, Attribute>();
        
        /// <summary>
        /// Event triggered when an attribute value changes.
        /// </summary>
        public event Action<Entity, Attribute, float, float> OnAttributeValueChanged;
        
        /// <summary>
        /// The name of the entity.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Creates a new entity with the specified name.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        public Entity(string name)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : "Unknown";
        }
        
        /// <summary>
        /// Gets an attribute by type.
        /// </summary>
        /// <param name="type">The type of attribute to get.</param>
        /// <returns>The attribute, or null if not found.</returns>
        public Attribute GetAttribute(AttributeType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            return _attributes.TryGetValue(type, out var attribute) ? attribute : null;
        }
        
        /// <summary>
        /// Checks if the entity has an attribute of the specified type.
        /// </summary>
        /// <param name="type">The type of attribute to check for.</param>
        /// <returns>True if the entity has the attribute, false otherwise.</returns>
        public bool HasAttribute(AttributeType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            return _attributes.ContainsKey(type);
        }
        
        /// <summary>
        /// Adds a new attribute to the entity.
        /// </summary>
        /// <param name="type">The type of attribute to add.</param>
        /// <param name="baseValue">The initial base value for the attribute.</param>
        /// <returns>The newly added attribute.</returns>
        public Attribute AddAttribute(AttributeType type, float baseValue = 0)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            if (_attributes.ContainsKey(type))
                throw new InvalidOperationException($"Attribute of type '{type}' already exists.");
                
            var attribute = new Attribute(type, baseValue);
            _attributes[type] = attribute;
            
            // Subscribe to attribute value changes
            attribute.OnValueChanged += (attr, oldValue, newValue) => 
            {
                OnAttributeValueChanged?.Invoke(this, attr, oldValue, newValue);
            };
            
            return attribute;
        }
        
        /// <summary>
        /// Sets or creates an attribute with the specified type and value.
        /// </summary>
        /// <param name="type">The type of attribute to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>The attribute that was set or created.</returns>
        public Attribute SetAttribute(AttributeType type, float value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            Attribute attribute;
            
            if (_attributes.TryGetValue(type, out attribute))
            {
                attribute.SetBaseValue(value);
            }
            else
            {
                attribute = AddAttribute(type, value);
            }
            
            return attribute;
        }
        
        /// <summary>
        /// Gets all attributes on this entity.
        /// </summary>
        /// <returns>An enumerable of all attributes.</returns>
        public IEnumerable<Attribute> GetAllAttributes()
        {
            return _attributes.Values;
        }
        
        /// <summary>
        /// Removes an attribute from the entity.
        /// </summary>
        /// <param name="type">The type of attribute to remove.</param>
        /// <returns>True if the attribute was removed, false if it didn't exist.</returns>
        public bool RemoveAttribute(AttributeType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            return _attributes.Remove(type);
        }
        
        /// <summary>
        /// Clears all attributes from the entity.
        /// </summary>
        public void ClearAttributes()
        {
            _attributes.Clear();
        }
    }
} 
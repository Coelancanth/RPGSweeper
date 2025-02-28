using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Base class for all entities in the game that have attributes
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// The name of this entity
        /// </summary>
        public string Name { get; protected set; }
        
        /// <summary>
        /// Event triggered when an attribute is added
        /// </summary>
        public event Action<Entity, Attribute> OnAttributeAdded;
        
        /// <summary>
        /// Event triggered when an attribute is removed
        /// </summary>
        public event Action<Entity, Attribute> OnAttributeRemoved;
        
        /// <summary>
        /// Event triggered when an attribute value changes
        /// </summary>
        public event Action<Entity, Attribute, float, float> OnAttributeValueChanged;
        
        /// <summary>
        /// Event triggered when a modifier is added to an attribute
        /// </summary>
        public event Action<Entity, Attribute, AttributeModifier> OnModifierAdded;
        
        /// <summary>
        /// Event triggered when a modifier is removed from an attribute
        /// </summary>
        public event Action<Entity, Attribute, AttributeModifier> OnModifierRemoved;
        
        /// <summary>
        /// Dictionary of all attributes on this entity
        /// </summary>
        protected readonly Dictionary<AttributeType, Attribute> _attributes = new Dictionary<AttributeType, Attribute>();
        
        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="name">The name of the entity</param>
        protected Entity(string name)
        {
            Name = name;
            InitializeAttributes();
        }
        
        /// <summary>
        /// Initializes the default attributes for this entity
        /// Override in derived classes to set up default attributes
        /// </summary>
        protected virtual void InitializeAttributes()
        {
            // Override in derived classes to set up default attributes
        }
        
        /// <summary>
        /// Gets an attribute by type
        /// </summary>
        /// <param name="type">The type of attribute to get</param>
        /// <returns>The attribute with the specified type, or null if not found</returns>
        public Attribute GetAttribute(AttributeType type)
        {
            return _attributes.TryGetValue(type, out var attribute) ? attribute : null;
        }
        
        /// <summary>
        /// Gets all attributes on this entity
        /// </summary>
        /// <returns>An enumerable of all attributes</returns>
        public IEnumerable<Attribute> GetAllAttributes()
        {
            return _attributes.Values;
        }
        
        /// <summary>
        /// Checks if this entity has an attribute of the specified type
        /// </summary>
        /// <param name="type">The type of attribute to check for</param>
        /// <returns>True if the entity has the attribute, false otherwise</returns>
        public bool HasAttribute(AttributeType type)
        {
            return _attributes.ContainsKey(type);
        }
        
        /// <summary>
        /// Sets the base value of an attribute
        /// If the attribute doesn't exist, it will be created
        /// </summary>
        /// <param name="type">The type of attribute to set</param>
        /// <param name="baseValue">The new base value</param>
        /// <returns>The attribute that was set</returns>
        public Attribute SetAttribute(AttributeType type, float baseValue)
        {
            if (_attributes.TryGetValue(type, out var attribute))
            {
                attribute.SetBaseValue(baseValue);
                return attribute;
            }
            else
            {
                return AddAttribute(type, baseValue);
            }
        }
        
        /// <summary>
        /// Adds a new attribute to this entity
        /// </summary>
        /// <param name="type">The type of attribute to add</param>
        /// <param name="baseValue">The base value of the attribute</param>
        /// <returns>The newly added attribute</returns>
        public Attribute AddAttribute(AttributeType type, float baseValue)
        {
            if (_attributes.ContainsKey(type))
            {
                throw new InvalidOperationException($"Entity {Name} already has an attribute of type {type.Id}");
            }
            
            var attribute = new Attribute(type, baseValue);
            _attributes[type] = attribute;
            
            // Subscribe to attribute events
            attribute.OnValueChanged += HandleAttributeValueChanged;
            attribute.OnModifierAdded += HandleModifierAdded;
            attribute.OnModifierRemoved += HandleModifierRemoved;
            
            OnAttributeAdded?.Invoke(this, attribute);
            
            return attribute;
        }
        
        /// <summary>
        /// Removes an attribute from this entity
        /// </summary>
        /// <param name="type">The type of attribute to remove</param>
        /// <returns>True if the attribute was removed, false otherwise</returns>
        public bool RemoveAttribute(AttributeType type)
        {
            if (_attributes.TryGetValue(type, out var attribute))
            {
                // Unsubscribe from attribute events
                attribute.OnValueChanged -= HandleAttributeValueChanged;
                attribute.OnModifierAdded -= HandleModifierAdded;
                attribute.OnModifierRemoved -= HandleModifierRemoved;
                
                _attributes.Remove(type);
                
                OnAttributeRemoved?.Invoke(this, attribute);
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Adds a modifier to an attribute
        /// </summary>
        /// <param name="modifier">The modifier to add</param>
        /// <returns>True if the modifier was added, false if the attribute doesn't exist</returns>
        public bool AddModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));
                
            if (_attributes.TryGetValue(modifier.AttributeType, out var attribute))
            {
                attribute.AddModifier(modifier);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Removes a modifier from an attribute
        /// </summary>
        /// <param name="modifier">The modifier to remove</param>
        /// <returns>True if the modifier was removed, false otherwise</returns>
        public bool RemoveModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                return false;
                
            if (_attributes.TryGetValue(modifier.AttributeType, out var attribute))
            {
                return attribute.RemoveModifier(modifier);
            }
            
            return false;
        }
        
        /// <summary>
        /// Removes all modifiers from a specific source
        /// </summary>
        /// <param name="source">The source of the modifiers to remove</param>
        /// <returns>The number of modifiers removed</returns>
        public int RemoveModifiersFromSource(object source)
        {
            int count = 0;
            
            foreach (var attribute in _attributes.Values)
            {
                count += attribute.RemoveModifiersFromSource(source);
            }
            
            return count;
        }
        
        /// <summary>
        /// Removes all modifiers from all attributes
        /// </summary>
        public void ClearAllModifiers()
        {
            foreach (var attribute in _attributes.Values)
            {
                attribute.ClearModifiers();
            }
        }
        
        /// <summary>
        /// Modifies the health of this entity
        /// </summary>
        /// <param name="amount">The amount to modify by (positive = heal, negative = damage)</param>
        public virtual void ModifyHealth(int amount)
        {
            if (amount == 0)
                return;
                
            var currentHealth = GetAttribute(AttributeType.CurrentHealth);
            if (currentHealth != null)
            {
                float newHealth = currentHealth.CurrentValue + amount;
                
                // Clamp to max health
                var maxHealth = GetAttribute(AttributeType.MaxHealth);
                if (maxHealth != null)
                {
                    newHealth = Mathf.Min(newHealth, maxHealth.CurrentValue);
                }
                
                // Ensure health doesn't go below 0
                newHealth = Mathf.Max(0, newHealth);
                
                currentHealth.SetBaseValue(newHealth);
            }
        }
        
        /// <summary>
        /// Called at the end of each turn to update attributes and modifiers
        /// </summary>
        public virtual void OnTurnEnd()
        {
            // Override in derived classes to handle turn-based effects
        }
        
        /// <summary>
        /// Handles attribute value changes
        /// </summary>
        private void HandleAttributeValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            OnAttributeValueChanged?.Invoke(this, attribute, oldValue, newValue);
        }
        
        /// <summary>
        /// Handles modifier additions
        /// </summary>
        private void HandleModifierAdded(Attribute attribute, AttributeModifier modifier)
        {
            OnModifierAdded?.Invoke(this, attribute, modifier);
        }
        
        /// <summary>
        /// Handles modifier removals
        /// </summary>
        private void HandleModifierRemoved(Attribute attribute, AttributeModifier modifier)
        {
            OnModifierRemoved?.Invoke(this, attribute, modifier);
        }
        
        public override string ToString()
        {
            return $"{Name} (Attributes: {_attributes.Count})";
        }
    }
} 
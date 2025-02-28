using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Represents a modifiable attribute with a base value and various modifiers
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// The type of this attribute
        /// </summary>
        public AttributeType Type { get; }
        
        /// <summary>
        /// The base value of this attribute before any modifiers
        /// </summary>
        public float BaseValue { get; private set; }
        
        /// <summary>
        /// The current value of this attribute after all modifiers
        /// </summary>
        public float CurrentValue { get; private set; }
        
        /// <summary>
        /// Minimum value this attribute can have
        /// </summary>
        public float MinValue { get; set; } = float.MinValue;
        
        /// <summary>
        /// Maximum value this attribute can have
        /// </summary>
        public float MaxValue { get; set; } = float.MaxValue;
        
        /// <summary>
        /// Event triggered when the attribute value changes
        /// </summary>
        public event Action<Attribute, float, float> OnValueChanged;
        
        /// <summary>
        /// Event triggered when a modifier is added
        /// </summary>
        public event Action<Attribute, AttributeModifier> OnModifierAdded;
        
        /// <summary>
        /// Event triggered when a modifier is removed
        /// </summary>
        public event Action<Attribute, AttributeModifier> OnModifierRemoved;
        
        /// <summary>
        /// All modifiers currently applied to this attribute
        /// </summary>
        public IReadOnlyList<AttributeModifier> Modifiers => _modifiers.AsReadOnly();
        
        private readonly List<AttributeModifier> _modifiers = new List<AttributeModifier>();
        private bool _isDirty = false;
        
        /// <summary>
        /// Creates a new attribute
        /// </summary>
        /// <param name="type">The type of attribute</param>
        /// <param name="baseValue">The base value of the attribute</param>
        public Attribute(AttributeType type, float baseValue)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            BaseValue = baseValue;
            CurrentValue = baseValue;
        }
        
        /// <summary>
        /// Sets the base value of this attribute and recalculates the current value
        /// </summary>
        /// <param name="value">The new base value</param>
        public void SetBaseValue(float value)
        {
            if (Mathf.Approximately(BaseValue, value))
                return;
                
            float oldValue = CurrentValue;
            BaseValue = value;
            RecalculateValue();
            
            if (!Mathf.Approximately(oldValue, CurrentValue))
            {
                OnValueChanged?.Invoke(this, oldValue, CurrentValue);
            }
        }
        
        /// <summary>
        /// Adds a modifier to this attribute and recalculates the current value
        /// </summary>
        /// <param name="modifier">The modifier to add</param>
        public void AddModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));
                
            if (modifier.AttributeType != Type)
                throw new ArgumentException($"Modifier attribute type {modifier.AttributeType.Id} does not match this attribute's type {Type.Id}");
                
            _modifiers.Add(modifier);
            _isDirty = true;
            
            float oldValue = CurrentValue;
            RecalculateValue();
            
            OnModifierAdded?.Invoke(this, modifier);
            
            if (!Mathf.Approximately(oldValue, CurrentValue))
            {
                OnValueChanged?.Invoke(this, oldValue, CurrentValue);
            }
        }
        
        /// <summary>
        /// Removes a modifier from this attribute and recalculates the current value
        /// </summary>
        /// <param name="modifier">The modifier to remove</param>
        /// <returns>True if the modifier was removed, false otherwise</returns>
        public bool RemoveModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                return false;
                
            bool removed = _modifiers.Remove(modifier);
            
            if (removed)
            {
                _isDirty = true;
                
                float oldValue = CurrentValue;
                RecalculateValue();
                
                OnModifierRemoved?.Invoke(this, modifier);
                
                if (!Mathf.Approximately(oldValue, CurrentValue))
                {
                    OnValueChanged?.Invoke(this, oldValue, CurrentValue);
                }
            }
            
            return removed;
        }
        
        /// <summary>
        /// Removes all modifiers from a specific source
        /// </summary>
        /// <param name="source">The source of the modifiers to remove</param>
        /// <returns>The number of modifiers removed</returns>
        public int RemoveModifiersFromSource(object source)
        {
            if (source == null)
                return 0;
                
            var modifiersToRemove = _modifiers.Where(m => m.Source == source).ToList();
            
            if (modifiersToRemove.Count == 0)
                return 0;
                
            foreach (var modifier in modifiersToRemove)
            {
                _modifiers.Remove(modifier);
                OnModifierRemoved?.Invoke(this, modifier);
            }
            
            _isDirty = true;
            
            float oldValue = CurrentValue;
            RecalculateValue();
            
            if (!Mathf.Approximately(oldValue, CurrentValue))
            {
                OnValueChanged?.Invoke(this, oldValue, CurrentValue);
            }
            
            return modifiersToRemove.Count;
        }
        
        /// <summary>
        /// Removes all modifiers from this attribute
        /// </summary>
        public void ClearModifiers()
        {
            if (_modifiers.Count == 0)
                return;
                
            var oldModifiers = new List<AttributeModifier>(_modifiers);
            _modifiers.Clear();
            
            _isDirty = true;
            
            float oldValue = CurrentValue;
            RecalculateValue();
            
            foreach (var modifier in oldModifiers)
            {
                OnModifierRemoved?.Invoke(this, modifier);
            }
            
            if (!Mathf.Approximately(oldValue, CurrentValue))
            {
                OnValueChanged?.Invoke(this, oldValue, CurrentValue);
            }
        }
        
        /// <summary>
        /// Recalculates the current value based on the base value and all modifiers
        /// </summary>
        public void RecalculateValue()
        {
            if (!_isDirty && _modifiers.Count == 0)
                return;
                
            float flatBonus = 0;
            float percentBonus = 0;
            float multiplier = 1;
            
            // Sort modifiers by priority
            var sortedModifiers = _modifiers.OrderBy(m => m.Priority).ToList();
            
            // Apply modifiers based on type
            foreach (var modifier in sortedModifiers)
            {
                switch (modifier.Type)
                {
                    case ModifierType.Flat:
                        flatBonus += modifier.Value;
                        break;
                    case ModifierType.Percent:
                        percentBonus += modifier.Value;
                        break;
                    case ModifierType.Multiplier:
                        multiplier *= modifier.Value;
                        break;
                }
            }
            
            // Calculate new value: (base + flat) * (1 + percent) * multiplier
            float newValue = (BaseValue + flatBonus) * (1 + percentBonus) * multiplier;
            
            // Clamp to min/max values
            newValue = Mathf.Clamp(newValue, MinValue, MaxValue);
            
            CurrentValue = newValue;
            _isDirty = false;
        }
        
        /// <summary>
        /// Gets a modifier by its ID
        /// </summary>
        /// <param name="id">The ID of the modifier to get</param>
        /// <returns>The modifier with the specified ID, or null if not found</returns>
        public AttributeModifier GetModifier(Guid id)
        {
            return _modifiers.FirstOrDefault(m => m.Id == id);
        }
        
        /// <summary>
        /// Gets all modifiers from a specific source
        /// </summary>
        /// <param name="source">The source of the modifiers to get</param>
        /// <returns>An enumerable of modifiers from the specified source</returns>
        public IEnumerable<AttributeModifier> GetModifiersFromSource(object source)
        {
            return _modifiers.Where(m => m.Source == source);
        }
        
        public override string ToString()
        {
            return $"{Type.Id}: {CurrentValue} (Base: {BaseValue}, Modifiers: {_modifiers.Count})";
        }
    }
} 
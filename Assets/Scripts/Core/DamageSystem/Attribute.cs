using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Represents an attribute with a base value and modifiers.
    /// </summary>
    public class Attribute
    {
        public AttributeType Type { get; }
        
        // Base value (without modifiers)
        private float _baseValue;
        
        // Calculated value (base + modifiers)
        private float _currentValue;
        
        // Min/Max constraints
        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;
        
        // Collection of modifiers
        private readonly List<AttributeModifier> _modifiers = new List<AttributeModifier>();
        
        // Event triggered when the value changes
        public event Action<Attribute, float, float> OnValueChanged;

        /// <summary>
        /// The current value of the attribute (base value + modifiers).
        /// </summary>
        public float CurrentValue 
        { 
            get => _currentValue;
            private set
            {
                float clampedValue = Mathf.Clamp(value, MinValue, MaxValue);
                
                if (_currentValue != clampedValue)
                {
                    float oldValue = _currentValue;
                    _currentValue = clampedValue;
                    OnValueChanged?.Invoke(this, oldValue, _currentValue);
                }
            }
        }

        /// <summary>
        /// Creates a new attribute with the specified type and base value.
        /// </summary>
        /// <param name="type">The type of attribute.</param>
        /// <param name="baseValue">The initial base value.</param>
        public Attribute(AttributeType type, float baseValue = 0)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            SetBaseValue(baseValue);
        }

        /// <summary>
        /// Sets the base value of the attribute and recalculates the current value.
        /// </summary>
        /// <param name="value">The new base value.</param>
        public void SetBaseValue(float value)
        {
            if (_baseValue != value)
            {
                _baseValue = value;
                RecalculateValue();
            }
        }

        /// <summary>
        /// Adds a modifier to the attribute.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        /// <returns>The added modifier.</returns>
        public AttributeModifier AddModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));
                
            _modifiers.Add(modifier);
            RecalculateValue();
            return modifier;
        }

        /// <summary>
        /// Removes a modifier from the attribute.
        /// </summary>
        /// <param name="modifier">The modifier to remove.</param>
        /// <returns>True if the modifier was removed, false otherwise.</returns>
        public bool RemoveModifier(AttributeModifier modifier)
        {
            bool removed = _modifiers.Remove(modifier);
            
            if (removed)
            {
                RecalculateValue();
            }
            
            return removed;
        }

        /// <summary>
        /// Removes all modifiers from the attribute.
        /// </summary>
        public void ClearModifiers()
        {
            if (_modifiers.Count > 0)
            {
                _modifiers.Clear();
                RecalculateValue();
            }
        }

        /// <summary>
        /// Recalculates the current value based on the base value and modifiers.
        /// </summary>
        private void RecalculateValue()
        {
            float flatAddition = 0;
            float percentageMultiplier = 1;
            
            // Apply modifiers in order based on their type
            foreach (var modifier in _modifiers)
            {
                switch (modifier.ModifierType)
                {
                    case AttributeModifierType.Flat:
                        flatAddition += modifier.Value;
                        break;
                    case AttributeModifierType.Percentage:
                        percentageMultiplier += modifier.Value;
                        break;
                }
            }
            
            // Calculate new value
            CurrentValue = (_baseValue + flatAddition) * percentageMultiplier;
        }
    }

    /// <summary>
    /// Represents a modifier applied to an attribute.
    /// </summary>
    public class AttributeModifier
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Source { get; }
        public float Value { get; }
        public AttributeModifierType ModifierType { get; }

        public AttributeModifier(string source, float value, AttributeModifierType modifierType)
        {
            Source = source;
            Value = value;
            ModifierType = modifierType;
        }
    }

    /// <summary>
    /// The type of attribute modifier.
    /// </summary>
    public enum AttributeModifierType
    {
        Flat,       // Adds a flat value to the base
        Percentage  // Multiplies by a percentage (1.0 = 100%)
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Represents a buff that can be applied to an entity
    /// </summary>
    public class Buff : IBuff
    {
        /// <summary>
        /// The name of this buff
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Description of what this buff does
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// The icon for this buff
        /// </summary>
        public Sprite Icon { get; }
        
        /// <summary>
        /// Whether this buff is a debuff (negative effect)
        /// </summary>
        public bool IsDebuff { get; }
        
        /// <summary>
        /// The current duration of this buff in turns
        /// </summary>
        public int Duration { get; private set; }
        
        /// <summary>
        /// The maximum number of stacks this buff can have
        /// </summary>
        public int MaxStacks { get; }
        
        /// <summary>
        /// The current number of stacks this buff has
        /// </summary>
        public int CurrentStacks { get; private set; } = 1;
        
        /// <summary>
        /// How this buff stacks with itself
        /// </summary>
        public BuffStackType StackType { get; }
        
        /// <summary>
        /// Whether this buff is currently active
        /// </summary>
        public bool IsActive => Duration > 0;
        
        /// <summary>
        /// The modifiers applied by this buff
        /// </summary>
        public IEnumerable<AttributeModifier> Modifiers => _modifiers.AsReadOnly();
        
        private readonly List<AttributeModifier> _modifiers = new List<AttributeModifier>();
        private readonly Action<Entity> _onApply;
        private readonly Action<Entity> _onRemove;
        private readonly Action<Entity> _onTurnEnd;
        
        /// <summary>
        /// Creates a new buff
        /// </summary>
        /// <param name="name">The name of the buff</param>
        /// <param name="description">Description of what the buff does</param>
        /// <param name="icon">The icon for the buff</param>
        /// <param name="isDebuff">Whether this is a debuff (negative effect)</param>
        /// <param name="duration">The duration in turns</param>
        /// <param name="maxStacks">The maximum number of stacks</param>
        /// <param name="stackType">How this buff stacks with itself</param>
        /// <param name="modifiers">The modifiers to apply</param>
        /// <param name="onApply">Action to perform when the buff is applied</param>
        /// <param name="onRemove">Action to perform when the buff is removed</param>
        /// <param name="onTurnEnd">Action to perform at the end of each turn</param>
        public Buff(
            string name,
            string description,
            Sprite icon,
            bool isDebuff,
            int duration,
            int maxStacks,
            BuffStackType stackType,
            IEnumerable<AttributeModifier> modifiers = null,
            Action<Entity> onApply = null,
            Action<Entity> onRemove = null,
            Action<Entity> onTurnEnd = null)
        {
            Name = name;
            Description = description;
            Icon = icon;
            IsDebuff = isDebuff;
            Duration = duration;
            MaxStacks = maxStacks;
            StackType = stackType;
            _onApply = onApply;
            _onRemove = onRemove;
            _onTurnEnd = onTurnEnd;
            
            if (modifiers != null)
            {
                _modifiers.AddRange(modifiers);
            }
        }
        
        /// <summary>
        /// Applies this buff to an entity
        /// </summary>
        public void Apply(Entity target)
        {
            // Apply all modifiers
            foreach (var modifier in _modifiers)
            {
                target.AddModifier(modifier);
            }
            
            // Execute on-apply action
            _onApply?.Invoke(target);
        }
        
        /// <summary>
        /// Removes this buff from an entity
        /// </summary>
        public void Remove(Entity target)
        {
            // Remove all modifiers
            foreach (var modifier in _modifiers)
            {
                target.RemoveModifier(modifier);
            }
            
            // Execute on-remove action
            _onRemove?.Invoke(target);
        }
        
        /// <summary>
        /// Updates this buff at the end of a turn
        /// </summary>
        /// <returns>True if the buff is still active, false if it expired</returns>
        public bool OnTurnEnd(Entity target)
        {
            // Reduce duration
            Duration--;
            
            // Execute on-turn-end action
            _onTurnEnd?.Invoke(target);
            
            // Return whether the buff is still active
            return Duration > 0;
        }
        
        /// <summary>
        /// Attempts to add a stack to this buff
        /// </summary>
        /// <returns>True if a stack was added, false otherwise</returns>
        public bool AddStack()
        {
            if (StackType == BuffStackType.None || CurrentStacks >= MaxStacks)
            {
                return false;
            }
            
            CurrentStacks++;
            return true;
        }
        
        /// <summary>
        /// Refreshes the duration of this buff
        /// </summary>
        /// <param name="duration">The new duration</param>
        public void RefreshDuration(int duration)
        {
            Duration = duration;
        }
    }
    
    /// <summary>
    /// Defines how buffs stack with themselves
    /// </summary>
    public enum BuffStackType
    {
        /// <summary>
        /// Doesn't stack, just refreshes duration
        /// </summary>
        None,
        
        /// <summary>
        /// Stacks multiple instances (applies modifiers multiple times)
        /// </summary>
        Stack,
        
        /// <summary>
        /// Increases intensity (effect value) when stacked
        /// </summary>
        Intensity
    }
} 
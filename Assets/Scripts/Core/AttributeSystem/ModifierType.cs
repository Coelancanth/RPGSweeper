using System;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Defines how attribute modifiers are applied to base values
    /// </summary>
    public enum ModifierType
    {
        /// <summary>
        /// Adds a flat value to the base value
        /// </summary>
        Flat,
        
        /// <summary>
        /// Adds a percentage of the base value
        /// </summary>
        Percent,
        
        /// <summary>
        /// Multiplies the final value after all other modifiers
        /// </summary>
        Multiplier
    }
} 
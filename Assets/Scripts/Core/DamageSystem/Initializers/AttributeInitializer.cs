using System;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem.Initializers
{
    /// <summary>
    /// Base class for monster attribute initializers
    /// </summary>
    public abstract class BaseMonsterAttributeInitializer : IAttributeInitializer
    {
        /// <summary>
        /// Initialize attributes for an entity
        /// </summary>
        public virtual void InitializeAttributes(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            // Set up core attributes with default values
            entity.AddAttribute(AttributeTypes.MAX_HP, 10);
            entity.AddAttribute(AttributeTypes.CURRENT_HP, 10);
            entity.AddAttribute(AttributeTypes.BASE_DAMAGE, 1);
            entity.AddAttribute(AttributeTypes.DAMAGE_PER_HIT, 1);
            
            // Set up resistances (defaults to 0)
            entity.AddAttribute(AttributeTypes.PHYSICAL_RESISTANCE, 0);
            entity.AddAttribute(AttributeTypes.MAGICAL_RESISTANCE, 0);
            entity.AddAttribute(AttributeTypes.FIRE_RESISTANCE, 0);
            entity.AddAttribute(AttributeTypes.ICE_RESISTANCE, 0);
            
            // Setup health constraints
            var currentHp = entity.GetAttribute(AttributeTypes.CURRENT_HP);
            var maxHp = entity.GetAttribute(AttributeTypes.MAX_HP);
            
            if (currentHp != null && maxHp != null)
            {
                currentHp.MinValue = 0;
                currentHp.MaxValue = maxHp.CurrentValue;
                
                // Listen for max HP changes to update current HP limit
                maxHp.OnValueChanged += (attr, oldVal, newVal) => {
                    currentHp.MaxValue = newVal;
                };
            }
        }
    }

    /// <summary>
    /// Default monster attribute initializer
    /// </summary>
    public class DefaultMonsterAttributeInitializer : BaseMonsterAttributeInitializer
    {
    }

    /// <summary>
    /// Slime-specific attribute initializer
    /// </summary>
    public class SlimeAttributeInitializer : BaseMonsterAttributeInitializer
    {
        public override void InitializeAttributes(Entity entity)
        {
            base.InitializeAttributes(entity);
            
            // Override default values
            entity.SetAttribute(AttributeTypes.MAX_HP, 5);
            entity.SetAttribute(AttributeTypes.CURRENT_HP, 5);
            entity.SetAttribute(AttributeTypes.BASE_DAMAGE, 1);
            
            // Slimes are resistant to physical damage
            entity.SetAttribute(AttributeTypes.PHYSICAL_RESISTANCE, 20);
            
            // Slimes are vulnerable to fire
            entity.SetAttribute(AttributeTypes.FIRE_RESISTANCE, -20);
        }
    }

    /// <summary>
    /// Spider-specific attribute initializer
    /// </summary>
    public class SpiderAttributeInitializer : BaseMonsterAttributeInitializer
    {
        public override void InitializeAttributes(Entity entity)
        {
            base.InitializeAttributes(entity);
            
            // Override default values
            entity.SetAttribute(AttributeTypes.MAX_HP, 8);
            entity.SetAttribute(AttributeTypes.CURRENT_HP, 8);
            entity.SetAttribute(AttributeTypes.BASE_DAMAGE, 2);
            
            // Spiders have higher evasion
            entity.AddAttribute(new AttributeType("Evasion"), 15);
        }
    }

    /// <summary>
    /// Dragon-specific attribute initializer
    /// </summary>
    public class DragonAttributeInitializer : BaseMonsterAttributeInitializer
    {
        public override void InitializeAttributes(Entity entity)
        {
            base.InitializeAttributes(entity);
            
            // Override default values
            entity.SetAttribute(AttributeTypes.MAX_HP, 30);
            entity.SetAttribute(AttributeTypes.CURRENT_HP, 30);
            entity.SetAttribute(AttributeTypes.BASE_DAMAGE, 5);
            entity.SetAttribute(AttributeTypes.ENRAGE_MULTIPLIER, 2.0f);
            
            // Dragons are resistant to all elemental damage
            entity.SetAttribute(AttributeTypes.FIRE_RESISTANCE, 50);
            entity.SetAttribute(AttributeTypes.ICE_RESISTANCE, 25);
            
            // Add unique attribute for dragons
            entity.AddAttribute(new AttributeType("BreathDamage"), 100);
        }
    }
} 
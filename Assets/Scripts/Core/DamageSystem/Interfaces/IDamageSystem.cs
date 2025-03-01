using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Interface for damage calculation
    /// </summary>
    public interface IDamageCalculator
    {
        DamageInfo Calculate(DamageInfo damageInfo);
    }

    /// <summary>
    /// Interface for damage application
    /// </summary>
    public interface IDamageApplier
    {
        void ApplyDamage(DamageInfo damageInfo);
    }

    /// <summary>
    /// Interface for attribute initialization
    /// </summary>
    public interface IAttributeInitializer
    {
        void InitializeAttributes(Entity entity);
    }

    /// <summary>
    /// Interface for damage sources
    /// </summary>
    public interface IDamageSource
    {
        DamageInfo CreateDamageInfo(IEntity target);
    }

    /// <summary>
    /// Interface for damage targets
    /// </summary>
    public interface IDamageTarget
    {
        void ReceiveDamage(DamageInfo damageInfo);
    }

    /// <summary>
    /// Interface for entities with attributes
    /// </summary>
    public interface IEntity
    {
        string Name { get; }
        Attribute GetAttribute(AttributeType type);
        bool HasAttribute(AttributeType type);
        Attribute SetAttribute(AttributeType type, float value);
        IEnumerable<Attribute> GetAllAttributes();
    }
} 
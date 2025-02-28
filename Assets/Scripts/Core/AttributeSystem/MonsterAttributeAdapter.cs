using System;
using System.Collections.Generic;
using UnityEngine;
using RPGMinesweeper;

namespace Minesweeper.Core.AttributeSystem
{
    /// <summary>
    /// Adapter class that implements IAttributeEntity for MonsterMine
    /// </summary>
    public class MonsterAttributeAdapter : IAttributeEntity
    {
        /// <summary>
        /// The underlying entity
        /// </summary>
        public Entity Entity { get; }
        
        /// <summary>
        /// The monster this adapter wraps
        /// </summary>
        private readonly object _monster;
        
        /// <summary>
        /// List of active buffs on this monster
        /// </summary>
        private readonly List<IBuff> _activeBuffs = new List<IBuff>();
        
        /// <summary>
        /// Creates a new monster attribute adapter
        /// </summary>
        /// <param name="monster">The monster to adapt</param>
        /// <param name="name">The name of the monster</param>
        /// <param name="monsterType">The type of monster</param>
        public MonsterAttributeAdapter(object monster, string name, MonsterType monsterType)
        {
            _monster = monster;
            Entity = new MonsterEntity(name, monsterType, monster);
        }
        
        /// <summary>
        /// Initializes monster attributes
        /// </summary>
        /// <param name="maxHp">Maximum HP</param>
        /// <param name="baseDamage">Base damage</param>
        /// <param name="defense">Defense value</param>
        public void InitializeAttributes(int maxHp, int baseDamage, int defense)
        {
            // Initialize core attributes
            Entity.SetAttribute(AttributeType.MaxHealth, maxHp);
            Entity.SetAttribute(AttributeType.CurrentHealth, maxHp);
            Entity.SetAttribute(AttributeType.Attack, baseDamage);
            Entity.SetAttribute(AttributeType.Defense, defense);
            
            // Initialize secondary attributes
            Entity.SetAttribute(AttributeType.Speed, 5); // Default speed
            
            // Apply monster type modifiers
            if (Entity is MonsterEntity monsterEntity)
            {
                //monsterEntity.ApplyMonsterTypeAttributes(monsterEntity.MonsterType);
            }
        }
        
        /// <summary>
        /// Gets the monster's current health
        /// </summary>
        public int CurrentHealth
        {
            get 
            {
                var attr = GetAttribute(AttributeType.CurrentHealth);
                return attr != null ? Mathf.RoundToInt(attr.CurrentValue) : 0;
            }
        }
        
        /// <summary>
        /// Gets the monster's maximum health
        /// </summary>
        public int MaxHealth
        {
            get 
            {
                var attr = GetAttribute(AttributeType.MaxHealth);
                return attr != null ? Mathf.RoundToInt(attr.CurrentValue) : 0;
            }
        }
        
        /// <summary>
        /// Gets the monster's HP percentage
        /// </summary>
        public float HpPercentage => (float)CurrentHealth / MaxHealth;
        
        /// <summary>
        /// Gets the monster's attack value
        /// </summary>
        public int Attack
        {
            get
            {
                var attr = GetAttribute(AttributeType.Attack);
                return attr != null ? Mathf.RoundToInt(attr.CurrentValue) : 0;
            }
        }
        
        /// <summary>
        /// Gets the monster's defense value
        /// </summary>
        public int Defense
        {
            get
            {
                var attr = GetAttribute(AttributeType.Defense);
                return attr != null ? Mathf.RoundToInt(attr.CurrentValue) : 0;
            }
        }
        
        /// <summary>
        /// Applies the enrage state to the monster
        /// </summary>
        /// <param name="damageMultiplier">The damage multiplier for enrage</param>
        public void ApplyEnrageState(float damageMultiplier)
        {
            if (Entity is MonsterEntity monsterEntity)
            {
                monsterEntity.ApplyEnrageState(damageMultiplier);
            }
        }
        
        /// <summary>
        /// Removes the enrage state from the monster
        /// </summary>
        public void RemoveEnrageState()
        {
            if (Entity is MonsterEntity monsterEntity)
            {
                monsterEntity.RemoveEnrageState();
            }
        }
        
        /// <summary>
        /// Gets an attribute by type
        /// </summary>
        public Attribute GetAttribute(AttributeType type)
        {
            return Entity.GetAttribute(type);
        }
        
        /// <summary>
        /// Sets the base value of an attribute
        /// </summary>
        public Attribute SetAttribute(AttributeType type, float value)
        {
            return Entity.SetAttribute(type, value);
        }
        
        /// <summary>
        /// Adds a modifier to an attribute
        /// </summary>
        public bool AddModifier(AttributeModifier modifier)
        {
            return Entity.AddModifier(modifier);
        }
        
        /// <summary>
        /// Removes a modifier from an attribute
        /// </summary>
        public bool RemoveModifier(AttributeModifier modifier)
        {
            return Entity.RemoveModifier(modifier);
        }
        
        /// <summary>
        /// Removes all modifiers from a specific source
        /// </summary>
        public int RemoveModifiersFromSource(object source)
        {
            return Entity.RemoveModifiersFromSource(source);
        }
        
        /// <summary>
        /// Applies a buff to this entity
        /// </summary>
        public void ApplyBuff(IBuff buff)
        {
            buff.Apply(Entity);
            _activeBuffs.Add(buff);
        }
        
        /// <summary>
        /// Removes a buff from this entity
        /// </summary>
        public void RemoveBuff(IBuff buff)
        {
            buff.Remove(Entity);
            _activeBuffs.Remove(buff);
        }
        
        /// <summary>
        /// Called at the end of each turn to update attributes and modifiers
        /// </summary>
        public void OnTurnEnd()
        {
            // Update all active buffs
            for (int i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[i];
                bool isStillActive = buff.OnTurnEnd(Entity);
                
                if (!isStillActive)
                {
                    buff.Remove(Entity);
                    _activeBuffs.RemoveAt(i);
                }
            }
            
            Entity.OnTurnEnd();
        }
        
        /// <summary>
        /// Modifies the health of this entity
        /// </summary>
        /// <param name="amount">The amount to modify (positive = heal, negative = damage)</param>
        public void ModifyHealth(int amount)
        {
            Entity.ModifyHealth(amount);
        }
    }
} 
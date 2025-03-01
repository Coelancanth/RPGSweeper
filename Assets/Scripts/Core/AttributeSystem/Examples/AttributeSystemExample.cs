using System;
using System.Collections.Generic;
using UnityEngine;
using Minesweeper.Core.DamageSystem;
using Minesweeper.Core.DamageSystem.Initializers;
using Attribute = Minesweeper.Core.DamageSystem.Attribute;

namespace Minesweeper.Core.AttributeSystem.Examples
{
    /// <summary>
    /// Example class demonstrating how to use the attribute system
    /// </summary>
    public class AttributeSystemExample : MonoBehaviour
    {
        // Example entity classes
        [Serializable]
        public class PlayerEntity : Entity
        {
            public PlayerEntity(string name) : base(name)
            {
                InitializeAttributes();
            }
            
            private void InitializeAttributes()
            {
                // Initialize player attributes with default values
                AddAttribute(new AttributeType("MaxHealth"), 100);
                AddAttribute(new AttributeType("CurrentHealth"), 100);
                AddAttribute(new AttributeType("Attack"), 10);
                AddAttribute(new AttributeType("Defense"), 5);
                AddAttribute(new AttributeType("CriticalChance"), 5); // 5% critical chance
                AddAttribute(new AttributeType("CriticalDamage"), 150); // 150% critical damage
                AddAttribute(new AttributeType("Speed"), 10);
                AddAttribute(new AttributeType("Level"), 1);
                AddAttribute(new AttributeType("Experience"), 0);
            }
            
            // Convenience property for common attribute types
            public AttributeType MaxHealthType => new AttributeType("MaxHealth");
            public AttributeType CurrentHealthType => new AttributeType("CurrentHealth");
            public AttributeType AttackType => new AttributeType("Attack");
            public AttributeType DefenseType => new AttributeType("Defense");
            public AttributeType CriticalChanceType => new AttributeType("CriticalChance");
            public AttributeType CriticalDamageType => new AttributeType("CriticalDamage");
            public AttributeType SpeedType => new AttributeType("Speed");
            public AttributeType LevelType => new AttributeType("Level");
            public AttributeType ExperienceType => new AttributeType("Experience");
            
            public void ModifyHealth(float amount)
            {
                var currentHealth = GetAttribute(CurrentHealthType);
                if (currentHealth != null)
                {
                    float newHealth = currentHealth.CurrentValue + amount;
                    var maxHealth = GetAttribute(MaxHealthType);
                    if (maxHealth != null)
                    {
                        newHealth = Mathf.Min(newHealth, maxHealth.CurrentValue);
                    }
                    
                    newHealth = Mathf.Max(0, newHealth);
                    currentHealth.SetBaseValue(newHealth);
                }
            }
        }
        
        [Serializable]
        public class MonsterEntity : Entity
        {
            public enum MonsterType
            {
                Normal,
                Elite,
                Boss
            }
            
            public MonsterType Type { get; }
            
            public MonsterEntity(string name, MonsterType type) : base(name)
            {
                Type = type;
                InitializeAttributes();
            }
            
            private void InitializeAttributes()
            {
                // Base attributes for all monster types
                AddAttribute(new AttributeType("MaxHealth"), 50);
                AddAttribute(new AttributeType("CurrentHealth"), 50);
                AddAttribute(new AttributeType("Attack"), 5);
                AddAttribute(new AttributeType("Defense"), 2);
                AddAttribute(new AttributeType("Speed"), 5);
                
                // Adjust attributes based on monster type
                switch (Type)
                {
                    case MonsterType.Elite:
                        // Elite monsters are stronger
                        GetAttribute(new AttributeType("MaxHealth")).SetBaseValue(100);
                        GetAttribute(new AttributeType("CurrentHealth")).SetBaseValue(100);
                        GetAttribute(new AttributeType("Attack")).SetBaseValue(10);
                        GetAttribute(new AttributeType("Defense")).SetBaseValue(5);
                        break;
                    case MonsterType.Boss:
                        // Bosses are much stronger
                        GetAttribute(new AttributeType("MaxHealth")).SetBaseValue(200);
                        GetAttribute(new AttributeType("CurrentHealth")).SetBaseValue(200);
                        GetAttribute(new AttributeType("Attack")).SetBaseValue(15);
                        GetAttribute(new AttributeType("Defense")).SetBaseValue(10);
                        break;
                }
            }
            
            // Convenience property for common attribute types
            public AttributeType MaxHealthType => new AttributeType("MaxHealth");
            public AttributeType CurrentHealthType => new AttributeType("CurrentHealth");
            public AttributeType AttackType => new AttributeType("Attack");
            public AttributeType DefenseType => new AttributeType("Defense");
            public AttributeType SpeedType => new AttributeType("Speed");
            
            public void ModifyHealth(float amount)
            {
                var currentHealth = GetAttribute(CurrentHealthType);
                if (currentHealth != null)
                {
                    float newHealth = currentHealth.CurrentValue + amount;
                    var maxHealth = GetAttribute(MaxHealthType);
                    if (maxHealth != null)
                    {
                        newHealth = Mathf.Min(newHealth, maxHealth.CurrentValue);
                    }
                    
                    newHealth = Mathf.Max(0, newHealth);
                    currentHealth.SetBaseValue(newHealth);
                }
            }
        }
        
        // Example buff class
        public class Buff
        {
            public string Name { get; }
            public float Duration { get; private set; }
            private readonly List<AttributeModifierEntry> _modifiers = new List<AttributeModifierEntry>();
            
            public Buff(string name, float duration)
            {
                Name = name;
                Duration = duration;
            }
            
            // Helper class to store the attribute type with the modifier
            private class AttributeModifierEntry
            {
                public AttributeType Type { get; }
                public AttributeModifier Modifier { get; }
                
                public AttributeModifierEntry(AttributeType type, AttributeModifier modifier)
                {
                    Type = type;
                    Modifier = modifier;
                }
            }
            
            public void AddModifier(AttributeType type, AttributeModifier modifier)
            {
                _modifiers.Add(new AttributeModifierEntry(type, modifier));
            }
            
            public void Apply(Entity target)
            {
                foreach (var entry in _modifiers)
                {
                    target.GetAttribute(entry.Type)?.AddModifier(entry.Modifier);
                }
                
                Debug.Log($"Applied buff {Name} to {target.Name}");
            }
            
            public void Remove(Entity target)
            {
                foreach (var entry in _modifiers)
                {
                    target.GetAttribute(entry.Type)?.RemoveModifier(entry.Modifier);
                }
                
                Debug.Log($"Removed buff {Name} from {target.Name}");
            }
            
            public void ReduceDuration(float amount)
            {
                Duration -= amount;
            }
        }
        
        private PlayerEntity _player;
        private MonsterEntity _monster;
        private readonly List<Buff> _activeBuffs = new List<Buff>();
        
        private void Start()
        {
            // Create entities
            _player = new PlayerEntity("Hero");
            _monster = new MonsterEntity("Goblin", MonsterEntity.MonsterType.Normal);
            
            // Subscribe to attribute change events
            _player.OnAttributeValueChanged += HandleAttributeValueChanged;
            _monster.OnAttributeValueChanged += HandleAttributeValueChanged;
            
            // Log initial stats
            LogEntityStats(_player);
            LogEntityStats(_monster);
            
            // Example: Apply a strength buff to the player
            ApplyStrengthBuff();
            
            // Example: Apply a defense debuff to the monster
            ApplyDefenseDebuff();
            
            // Example: Simulate combat
            SimulateCombat();
            
            // Example: End turn and update buffs
            EndTurn();
        }
        
        private void ApplyStrengthBuff()
        {
            // Create a strength buff that increases attack by 50% for 3 turns
            var strengthBuff = new Buff("Strength", 3);
            
            // Create a modifier for the buff
            var attackModifier = new AttributeModifier(
                "StrengthBuff",
                0.5f, // 50% increase
                AttributeModifierType.Percentage
            );
            
            strengthBuff.AddModifier(_player.AttackType, attackModifier);
            
            // Apply the buff to the player
            strengthBuff.Apply(_player);
            _activeBuffs.Add(strengthBuff);
            
            // Log the player's stats after the buff
            LogEntityStats(_player);
        }
        
        private void ApplyDefenseDebuff()
        {
            // Create a weakness debuff that decreases defense by 30% for 2 turns
            var weaknessBuff = new Buff("Weakness", 2);
            
            // Create a modifier for the debuff
            var defenseModifier = new AttributeModifier(
                "WeaknessDebuff",
                -0.3f, // 30% decrease
                AttributeModifierType.Percentage
            );
            
            weaknessBuff.AddModifier(_monster.DefenseType, defenseModifier);
            
            // Apply the debuff to the monster
            weaknessBuff.Apply(_monster);
            _activeBuffs.Add(weaknessBuff);
            
            // Log the monster's stats after the debuff
            LogEntityStats(_monster);
        }
        
        private void SimulateCombat()
        {
            // Player attacks monster
            float playerAttack = _player.GetAttribute(_player.AttackType).CurrentValue;
            float monsterDefense = _monster.GetAttribute(_monster.DefenseType).CurrentValue;
            
            // Simple damage formula: damage = attack * (100 / (100 + defense))
            float damageReduction = 100f / (100f + monsterDefense);
            int damage = Mathf.RoundToInt(playerAttack * damageReduction);
            
            Debug.Log($"{_player.Name} attacks {_monster.Name} for {damage} damage!");
            _monster.ModifyHealth(-damage);
            
            // Monster attacks player if still alive
            if (_monster.GetAttribute(_monster.CurrentHealthType).CurrentValue > 0)
            {
                float monsterAttack = _monster.GetAttribute(_monster.AttackType).CurrentValue;
                float playerDefense = _player.GetAttribute(_player.DefenseType).CurrentValue;
                
                damageReduction = 100f / (100f + playerDefense);
                damage = Mathf.RoundToInt(monsterAttack * damageReduction);
                
                Debug.Log($"{_monster.Name} attacks {_player.Name} for {damage} damage!");
                _player.ModifyHealth(-damage);
            }
            
            // Log final stats
            LogEntityStats(_player);
            LogEntityStats(_monster);
        }
        
        private void EndTurn()
        {
            Debug.Log("--- End Turn ---");
            
            // Update buff durations
            for (int i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[i];
                buff.ReduceDuration(1);
                
                if (buff.Duration <= 0)
                {
                    // Remove expired buffs
                    if (buff.Name == "Strength")
                    {
                        buff.Remove(_player);
                    }
                    else if (buff.Name == "Weakness")
                    {
                        buff.Remove(_monster);
                    }
                    
                    _activeBuffs.RemoveAt(i);
                }
            }
            
            // Log stats after buffs update
            LogEntityStats(_player);
            LogEntityStats(_monster);
        }
        
        private void HandleAttributeValueChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
        {
            Debug.Log($"{entity.Name}'s {attribute.Type.Name} changed from {oldValue} to {newValue}");
        }
        
        private void LogEntityStats(Entity entity)
        {
            Debug.Log($"--- {entity.Name} Stats ---");
            
            foreach (var attribute in entity.GetAllAttributes())
            {
                Debug.Log($"{attribute.Type.Name}: {attribute.CurrentValue}");
            }
        }
    }
} 
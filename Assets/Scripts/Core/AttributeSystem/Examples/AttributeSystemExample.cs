using System;
using System.Collections.Generic;
using UnityEngine;

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
            }
            
            protected override void InitializeAttributes()
            {
                // Initialize player attributes with default values
                AddAttribute(AttributeType.MaxHealth, 100);
                AddAttribute(AttributeType.CurrentHealth, 100);
                AddAttribute(AttributeType.Attack, 10);
                AddAttribute(AttributeType.Defense, 5);
                AddAttribute(AttributeType.CriticalChance, 5); // 5% critical chance
                AddAttribute(AttributeType.CriticalDamage, 150); // 150% critical damage
                AddAttribute(AttributeType.Speed, 10);
                AddAttribute(AttributeType.Level, 1);
                AddAttribute(AttributeType.Experience, 0);
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
            }
            
            protected override void InitializeAttributes()
            {
                // Base attributes for all monster types
                AddAttribute(AttributeType.MaxHealth, 50);
                AddAttribute(AttributeType.CurrentHealth, 50);
                AddAttribute(AttributeType.Attack, 5);
                AddAttribute(AttributeType.Defense, 2);
                AddAttribute(AttributeType.Speed, 5);
                
                // Adjust attributes based on monster type
                switch (Type)
                {
                    case MonsterType.Elite:
                        // Elite monsters are stronger
                        GetAttribute(AttributeType.MaxHealth).SetBaseValue(100);
                        GetAttribute(AttributeType.CurrentHealth).SetBaseValue(100);
                        GetAttribute(AttributeType.Attack).SetBaseValue(10);
                        GetAttribute(AttributeType.Defense).SetBaseValue(5);
                        break;
                    case MonsterType.Boss:
                        // Bosses are much stronger
                        GetAttribute(AttributeType.MaxHealth).SetBaseValue(200);
                        GetAttribute(AttributeType.CurrentHealth).SetBaseValue(200);
                        GetAttribute(AttributeType.Attack).SetBaseValue(15);
                        GetAttribute(AttributeType.Defense).SetBaseValue(10);
                        break;
                }
            }
        }
        
        // Example buff class
        public class Buff
        {
            public string Name { get; }
            public float Duration { get; private set; }
            private readonly List<AttributeModifier> _modifiers = new List<AttributeModifier>();
            
            public Buff(string name, float duration)
            {
                Name = name;
                Duration = duration;
            }
            
            public void AddModifier(AttributeModifier modifier)
            {
                _modifiers.Add(modifier);
            }
            
            public void Apply(Entity target)
            {
                foreach (var modifier in _modifiers)
                {
                    target.AddModifier(modifier);
                }
                
                Debug.Log($"Applied buff {Name} to {target.Name}");
            }
            
            public void Remove(Entity target)
            {
                foreach (var modifier in _modifiers)
                {
                    target.RemoveModifier(modifier);
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
            var attackModifier = AttributeModifier.CreatePercent(
                AttributeType.Attack,
                0.5f, // 50% increase
                strengthBuff
            );
            
            strengthBuff.AddModifier(attackModifier);
            
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
            var defenseModifier = AttributeModifier.CreatePercent(
                AttributeType.Defense,
                -0.3f, // 30% decrease
                weaknessBuff
            );
            
            weaknessBuff.AddModifier(defenseModifier);
            
            // Apply the debuff to the monster
            weaknessBuff.Apply(_monster);
            _activeBuffs.Add(weaknessBuff);
            
            // Log the monster's stats after the debuff
            LogEntityStats(_monster);
        }
        
        private void SimulateCombat()
        {
            // Player attacks monster
            float playerAttack = _player.GetAttribute(AttributeType.Attack).CurrentValue;
            float monsterDefense = _monster.GetAttribute(AttributeType.Defense).CurrentValue;
            
            // Simple damage formula: damage = attack * (100 / (100 + defense))
            float damageReduction = 100f / (100f + monsterDefense);
            int damage = Mathf.RoundToInt(playerAttack * damageReduction);
            
            Debug.Log($"{_player.Name} attacks {_monster.Name} for {damage} damage!");
            _monster.ModifyHealth(-damage);
            
            // Monster attacks player if still alive
            if (_monster.GetAttribute(AttributeType.CurrentHealth).CurrentValue > 0)
            {
                float monsterAttack = _monster.GetAttribute(AttributeType.Attack).CurrentValue;
                float playerDefense = _player.GetAttribute(AttributeType.Defense).CurrentValue;
                
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
            
            // Log stats after buffs are updated
            LogEntityStats(_player);
            LogEntityStats(_monster);
        }
        
        private void HandleAttributeValueChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
        {
            Debug.Log($"{entity.Name}'s {attribute.Type.Id} changed from {oldValue} to {newValue}");
        }
        
        private void LogEntityStats(Entity entity)
        {
            Debug.Log($"--- {entity.Name} Stats ---");
            
            foreach (var attribute in entity.GetAllAttributes())
            {
                Debug.Log($"{attribute.Type.Id}: {attribute.CurrentValue} (Base: {attribute.BaseValue})");
            }
        }
    }
} 
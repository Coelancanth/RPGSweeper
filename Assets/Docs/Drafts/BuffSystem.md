# Buff and Damage Calculation System

## Overview

This document outlines the design for a comprehensive buff system and damage calculation framework for the RPG Minesweeper game. The system will handle various modifiers to character attributes, abilities, and damage calculations while integrating with existing game systems.

## Core Concepts and Definitions

- **Attribute**: A base numeric property of an entity (e.g., strength, health, defense)
- **Buff**: A temporary positive modifier to attributes or abilities
- **Debuff**: A temporary negative modifier to attributes or abilities
- **Trait**: A permanent characteristic that affects gameplay in specific ways
- **Passive**: An always-active ability that provides constant benefits
- **Status Effect**: A temporary condition affecting gameplay (e.g., stun, confusion)

## System Architecture

### 1. Attribute System

The attribute system forms the foundation for all entity statistics and capabilities.

```csharp
public class AttributeType
{
    public static readonly AttributeType MaxHealth = new("MaxHealth");
    public static readonly AttributeType Attack = new("Attack");
    public static readonly AttributeType Defense = new("Defense");
    public static readonly AttributeType CriticalChance = new("CriticalChance");
    public static readonly AttributeType Speed = new("Speed");
    // More attributes as needed
    
    public string Id { get; }
    
    private AttributeType(string id)
    {
        Id = id;
    }
}

public class Attribute
{
    public AttributeType Type { get; }
    public float BaseValue { get; private set; }
    public float CurrentValue { get; private set; }
    
    private readonly List<AttributeModifier> _modifiers = new();
    
    public Attribute(AttributeType type, float baseValue)
    {
        Type = type;
        BaseValue = baseValue;
        CurrentValue = baseValue;
    }
    
    public void SetBaseValue(float value)
    {
        BaseValue = value;
        RecalculateValue();
    }
    
    public void AddModifier(AttributeModifier modifier)
    {
        _modifiers.Add(modifier);
        RecalculateValue();
    }
    
    public void RemoveModifier(AttributeModifier modifier)
    {
        _modifiers.Remove(modifier);
        RecalculateValue();
    }
    
    public void RemoveModifiersFromSource(object source)
    {
        _modifiers.RemoveAll(m => m.Source == source);
        RecalculateValue();
    }
    
    private void RecalculateValue()
    {
        float flatBonus = 0;
        float percentBonus = 0;
        
        foreach (var modifier in _modifiers)
        {
            switch (modifier.Type)
            {
                case ModifierType.Flat:
                    flatBonus += modifier.Value;
                    break;
                case ModifierType.Percent:
                    percentBonus += modifier.Value;
                    break;
            }
        }
        
        CurrentValue = (BaseValue + flatBonus) * (1 + percentBonus);
    }
}
```

### 2. Modifier System

The modifier system handles all temporary and permanent changes to attributes.

```csharp
public enum ModifierType
{
    Flat,       // Adds a flat value to the base
    Percent     // Adds a percentage of the base value
}

public class AttributeModifier
{
    public Guid Id { get; } = Guid.NewGuid();
    public AttributeType AttributeType { get; }
    public float Value { get; }
    public ModifierType Type { get; }
    public object Source { get; }
    public int Priority { get; } // Higher priority modifiers are applied last
    
    public AttributeModifier(AttributeType attributeType, float value, ModifierType type, object source, int priority = 0)
    {
        AttributeType = attributeType;
        Value = value;
        Type = type;
        Source = source;
        Priority = priority;
    }
}
```

### 3. Buff System

The buff system manages time-limited modifiers affecting entities.

```csharp
public enum BuffStackType
{
    None,       // Doesn't stack, refreshes duration
    Stack,      // Stacks multiple instances
    Intensity   // Increases intensity (effect) when stacked
}

public class Buff
{
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    public bool IsDebuff { get; }
    public int Duration { get; private set; }  // In turns
    public int MaxStacks { get; }
    public int CurrentStacks { get; private set; } = 1;
    public BuffStackType StackType { get; }
    
    private readonly List<AttributeModifier> _modifiers = new();
    private readonly Action<Entity> _onApply;
    private readonly Action<Entity> _onRemove;
    private readonly Action<Entity> _onTurnEnd;
    
    public Buff(string name, string description, Sprite icon, bool isDebuff, int duration, 
                int maxStacks, BuffStackType stackType, 
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
    
    public void Apply(Entity target)
    {
        foreach (var modifier in _modifiers)
        {
            target.AddModifier(modifier);
        }
        
        _onApply?.Invoke(target);
    }
    
    public void Remove(Entity target)
    {
        foreach (var modifier in _modifiers)
        {
            target.RemoveModifier(modifier);
        }
        
        _onRemove?.Invoke(target);
    }
    
    public void OnTurnEnd(Entity target)
    {
        Duration--;
        _onTurnEnd?.Invoke(target);
    }
    
    public bool AddStack()
    {
        if (StackType == BuffStackType.None || CurrentStacks >= MaxStacks)
            return false;
            
        CurrentStacks++;
        return true;
    }
    
    public void RefreshDuration(int duration)
    {
        Duration = duration;
    }
}
```

### 4. Entity System

The entity system manages attributes and buffs for all game actors.

```csharp
public abstract class Entity
{
    public string Name { get; protected set; }
    
    private readonly Dictionary<AttributeType, Attribute> _attributes = new();
    private readonly List<Buff> _buffs = new();
    
    public Entity(string name)
    {
        Name = name;
        InitializeAttributes();
    }
    
    protected virtual void InitializeAttributes()
    {
        // Override in derived classes to set up default attributes
    }
    
    public Attribute GetAttribute(AttributeType type)
    {
        return _attributes.TryGetValue(type, out var attribute) ? attribute : null;
    }
    
    public void SetAttribute(AttributeType type, float baseValue)
    {
        if (_attributes.TryGetValue(type, out var attribute))
        {
            attribute.SetBaseValue(baseValue);
        }
        else
        {
            _attributes[type] = new Attribute(type, baseValue);
        }
    }
    
    public void AddModifier(AttributeModifier modifier)
    {
        if (_attributes.TryGetValue(modifier.AttributeType, out var attribute))
        {
            attribute.AddModifier(modifier);
        }
    }
    
    public void RemoveModifier(AttributeModifier modifier)
    {
        if (_attributes.TryGetValue(modifier.AttributeType, out var attribute))
        {
            attribute.RemoveModifier(modifier);
        }
    }
    
    public void ApplyBuff(Buff buff)
    {
        var existingBuff = _buffs.FirstOrDefault(b => b.Name == buff.Name);
        
        if (existingBuff != null)
        {
            switch (buff.StackType)
            {
                case BuffStackType.None:
                    existingBuff.RefreshDuration(buff.Duration);
                    break;
                case BuffStackType.Stack:
                case BuffStackType.Intensity:
                    if (existingBuff.AddStack())
                    {
                        existingBuff.RefreshDuration(buff.Duration);
                        buff.Apply(this);
                    }
                    break;
            }
        }
        else
        {
            _buffs.Add(buff);
            buff.Apply(this);
        }
    }
    
    public void RemoveBuff(Buff buff)
    {
        if (_buffs.Remove(buff))
        {
            buff.Remove(this);
        }
    }
    
    public void RemoveAllBuffs()
    {
        foreach (var buff in _buffs.ToList())
        {
            RemoveBuff(buff);
        }
    }
    
    public virtual void OnTurnEnd()
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnTurnEnd(this);
            
            if (buff.Duration <= 0)
            {
                RemoveBuff(buff);
            }
        }
    }
}
```

### 5. Damage Calculation System

The damage calculation system handles all aspects of combat damage computation.

```csharp
public enum DamageType
{
    Physical,
    Magical,
    True        // Ignores defense
}

public class DamageInfo
{
    public Entity Source { get; }
    public Entity Target { get; }
    public float Amount { get; set; }
    public DamageType Type { get; }
    public bool IsCritical { get; set; }
    public Dictionary<string, object> Metadata { get; } = new();
    
    public DamageInfo(Entity source, Entity target, float amount, DamageType type)
    {
        Source = source;
        Target = target;
        Amount = amount;
        Type = type;
    }
}

public static class DamageCalculator
{
    // Chain of responsibility pattern for damage calculation
    private static readonly List<IDamageModifier> _modifiers = new();
    
    public static void RegisterModifier(IDamageModifier modifier)
    {
        _modifiers.Add(modifier);
        _modifiers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }
    
    public static void UnregisterModifier(IDamageModifier modifier)
    {
        _modifiers.Remove(modifier);
    }
    
    public static float CalculateDamage(DamageInfo damageInfo)
    {
        // Apply base calculations
        switch (damageInfo.Type)
        {
            case DamageType.Physical:
                ApplyPhysicalDamageCalculation(damageInfo);
                break;
            case DamageType.Magical:
                ApplyMagicalDamageCalculation(damageInfo);
                break;
            case DamageType.True:
                // True damage ignores defense
                break;
        }
        
        // Apply critical hit calculation
        ApplyCriticalHitCalculation(damageInfo);
        
        // Apply registered damage modifiers
        foreach (var modifier in _modifiers)
        {
            modifier.ModifyDamage(damageInfo);
        }
        
        return Mathf.Max(0, damageInfo.Amount);
    }
    
    private static void ApplyPhysicalDamageCalculation(DamageInfo damageInfo)
    {
        var attackValue = damageInfo.Source.GetAttribute(AttributeType.Attack)?.CurrentValue ?? 0;
        var defenseValue = damageInfo.Target.GetAttribute(AttributeType.Defense)?.CurrentValue ?? 0;
        
        // Simple formula: damage = attack * (100 / (100 + defense))
        float damageReduction = 100f / (100f + defenseValue);
        damageInfo.Amount = attackValue * damageReduction;
    }
    
    private static void ApplyMagicalDamageCalculation(DamageInfo damageInfo)
    {
        var magicValue = damageInfo.Source.GetAttribute(AttributeType.MagicPower)?.CurrentValue ?? 0;
        var resistanceValue = damageInfo.Target.GetAttribute(AttributeType.MagicResistance)?.CurrentValue ?? 0;
        
        // Similar formula but for magic
        float damageReduction = 100f / (100f + resistanceValue);
        damageInfo.Amount = magicValue * damageReduction;
    }
    
    private static void ApplyCriticalHitCalculation(DamageInfo damageInfo)
    {
        var critChance = damageInfo.Source.GetAttribute(AttributeType.CriticalChance)?.CurrentValue ?? 0;
        var critDamage = damageInfo.Source.GetAttribute(AttributeType.CriticalDamage)?.CurrentValue ?? 150;
        
        // Check for critical hit (percentage chance)
        if (UnityEngine.Random.Range(0f, 100f) < critChance)
        {
            damageInfo.IsCritical = true;
            damageInfo.Amount *= critDamage / 100f;
        }
    }
}

public interface IDamageModifier
{
    int Priority { get; }
    void ModifyDamage(DamageInfo damageInfo);
}
```

### 6. Trait System

The trait system handles permanent characteristics that affect gameplay.

```csharp
public class Trait
{
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    
    private readonly List<AttributeModifier> _modifiers = new();
    private readonly Action<Entity> _onApply;
    private readonly Action<Entity> _onRemove;
    
    public Trait(string name, string description, Sprite icon, 
                IEnumerable<AttributeModifier> modifiers = null,
                Action<Entity> onApply = null, 
                Action<Entity> onRemove = null)
    {
        Name = name;
        Description = description;
        Icon = icon;
        _onApply = onApply;
        _onRemove = onRemove;
        
        if (modifiers != null)
        {
            _modifiers.AddRange(modifiers);
        }
    }
    
    public void Apply(Entity target)
    {
        foreach (var modifier in _modifiers)
        {
            target.AddModifier(modifier);
        }
        
        _onApply?.Invoke(target);
    }
    
    public void Remove(Entity target)
    {
        foreach (var modifier in _modifiers)
        {
            target.RemoveModifier(modifier);
        }
        
        _onRemove?.Invoke(target);
    }
}
```

## Integration with Existing Systems

### Integration with Effect System

The existing Effect system should be extended to support the new Buff system:

```csharp
public class BuffEffect : BaseEffect, IPersistentEffect
{
    private readonly Buff _buff;
    private readonly Entity _affectedEntity;
    private bool _isActive;
    
    public bool IsActive => _isActive;
    
    public BuffEffect(Buff buff, Entity target)
    {
        _buff = buff;
        _affectedEntity = target;
        m_CurrentMode = EffectType.Persistent;
    }
    
    protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
    {
        _affectedEntity.ApplyBuff(_buff);
        _isActive = true;
    }
    
    public void Update(float deltaTime)
    {
        // No additional update logic needed - handled by Entity.OnTurnEnd
    }
    
    public void Remove(GameObject target)
    {
        if (_isActive)
        {
            _affectedEntity.RemoveBuff(_buff);
            _isActive = false;
        }
    }
}
```

### Integration with TurnSystem

The TurnSystem needs to trigger buff updates at the end of each turn:

```csharp
// Add to TurnManager.EndTurn method:
public void EndTurn()
{
    // Existing code...
    
    // Update all entities' buffs
    foreach (var entity in _entities)
    {
        entity.OnTurnEnd();
    }
    
    // Rest of existing code...
}
```

### Integration with PlayerStats

The existing PlayerStats class should be refactored to use the new Attribute system:

```csharp
public class PlayerStats
{
    private readonly Entity _playerEntity;
    
    public int CurrentHP => (int)_playerEntity.GetAttribute(AttributeType.CurrentHealth).CurrentValue;
    public int MaxHP => (int)_playerEntity.GetAttribute(AttributeType.MaxHealth).CurrentValue;
    public int Experience => (int)_playerEntity.GetAttribute(AttributeType.Experience).CurrentValue;
    public int Level => (int)_playerEntity.GetAttribute(AttributeType.Level).CurrentValue;

    public event Action<int> OnLevelUp;
    public event Action<int> OnHPChanged;
    public event Action<int> OnExperienceChanged;

    public PlayerStats(int initialMaxHP)
    {
        _playerEntity = new PlayerEntity("Player");
        _playerEntity.SetAttribute(AttributeType.MaxHealth, initialMaxHP);
        _playerEntity.SetAttribute(AttributeType.CurrentHealth, initialMaxHP);
        _playerEntity.SetAttribute(AttributeType.Level, 1);
        _playerEntity.SetAttribute(AttributeType.Experience, 0);
        
        // Subscribe to attribute changes
        // (Implementation depends on how attribute changes are exposed)
    }
    
    // Remaining methods would be refactored to use the _playerEntity attributes
}
```

### Integration with Existing Player and Monster Classes

Looking at the existing `Player.cs` and `MonsterMine.cs` classes, there are several approaches to integrate them with the Entity system:

#### Option 1: Composition Approach (Recommended)

This approach adds an Entity component to existing classes without requiring major refactoring.

```csharp
// Modified Player class with Entity composition
public class Player
{
    private PlayerStats m_Stats;
    private readonly int m_BaseMaxHP = 20;
    private readonly int m_HPIncreasePerLevel = 5;
    private readonly PlayerEntity m_Entity;

    public event Action OnDeath;

    // Public properties to expose stats
    public int CurrentHp => m_Stats.CurrentHP;
    public int MaxHp => m_Stats.MaxHP;
    public int Level => m_Stats.Level;
    public int Experience => m_Stats.Experience;
    
    // Expose Entity for buff and class systems
    public Entity Entity => m_Entity;

    public Player()
    {
        m_Stats = new PlayerStats(m_BaseMaxHP);
        m_Entity = new PlayerEntity(this, "Player");
        
        m_Stats.OnLevelUp += HandleLevelUp;
    }

    public void TakeDamage(int _damage)
    {
        if (_damage <= 0) return;

        // Apply damage modifiers through Entity system
        var damageInfo = new DamageInfo(null, m_Entity, _damage, DamageType.Physical);
        float finalDamage = DamageCalculator.CalculateDamage(damageInfo);
        
        m_Stats.ModifyHP(-Mathf.RoundToInt(finalDamage));

        if (m_Stats.CurrentHP <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private void HandleLevelUp(int _newLevel)
    {
        int newMaxHP = m_BaseMaxHP + (_newLevel - 1) * m_HPIncreasePerLevel;
        m_Stats.SetMaxHP(newMaxHP);
        m_Stats.RestoreFullHP();
        
        // Update entity attributes
        m_Entity.SetAttribute(AttributeType.Level, _newLevel);
    }

    public void GainExperience(int _amount)
    {
        if (_amount <= 0) return;
        m_Stats.AddExperience(_amount);
    }
    
    // Handle buffs through entity
    public void ApplyBuff(Buff buff)
    {
        m_Entity.ApplyBuff(buff);
    }
    
    public void RemoveBuff(Buff buff)
    {
        m_Entity.RemoveBuff(buff);
    }
    
    public void OnTurnEnd()
    {
        m_Entity.OnTurnEnd();
    }
}

// Entity implementation specific to Player
public class PlayerEntity : Entity
{
    private Player m_Player;
    
    public PlayerEntity(Player player, string name) : base(name)
    {
        m_Player = player;
    }
    
    protected override void InitializeAttributes()
    {
        // Initialize with player's stats
        SetAttribute(AttributeType.MaxHealth, m_Player.MaxHp);
        SetAttribute(AttributeType.CurrentHealth, m_Player.CurrentHp);
        SetAttribute(AttributeType.Level, m_Player.Level);
        SetAttribute(AttributeType.Attack, 10); // Default value
        SetAttribute(AttributeType.Defense, 5); // Default value
        // Additional attributes can be set here
    }
    
    // Synchronize HP changes with player
    public override void ModifyHealth(int amount)
    {
        if (amount != 0)
        {
            // We don't call the player's TakeDamage method to avoid infinite recursion
            // Instead, we update the attribute and let the player handle the actual health changes
            var currentHealth = GetAttribute(AttributeType.CurrentHealth);
            if (currentHealth != null)
            {
                currentHealth.SetBaseValue(currentHealth.CurrentValue + amount);
            }
        }
    }
}
```

Similarly for MonsterMine:

```csharp
// Add to MonsterMine class
public class MonsterMine : IDamagingMine 
{
    // Existing code...
    
    private MonsterEntity m_Entity;
    
    // Expose Entity for buff and class systems
    public Entity Entity => m_Entity;
    
    public MonsterMine(MonsterMineData _data, Vector2Int _position)
    {
        // Existing constructor code...
        
        m_Entity = new MonsterEntity(this, _data.Name, _data.MonsterType);
        InitializeEntityAttributes();
    }
    
    private void InitializeEntityAttributes()
    {
        m_Entity.SetAttribute(AttributeType.MaxHealth, m_MaxHp);
        m_Entity.SetAttribute(AttributeType.CurrentHealth, m_CurrentHp);
        m_Entity.SetAttribute(AttributeType.Attack, m_BaseDamage);
        m_Entity.SetAttribute(AttributeType.Defense, m_Data.Defense);
        // Additional attributes can be set here
    }
    
    // Modify OnTrigger to use damage calculation system
    public void OnTrigger(PlayerComponent _player)
    {
        // If already defeated but not yet collectable, mark as collectable
        if (m_IsDefeated && !m_IsCollectable)
        {
            m_IsCollectable = true;
            OnHpChanged?.Invoke(m_Position, 0);
            return;
        }
        
        // If already defeated and collectable, don't do anything here
        if (m_IsDefeated && m_IsCollectable)
        {
            return;
        }
        
        // Use damage calculation system to deal damage to player
        if (_player.Player != null && _player.Player.Entity != null)
        {
            var damageInfo = new DamageInfo(
                m_Entity, 
                _player.Player.Entity, 
                CalculateDamage(), 
                DamageType.Physical
            );
            
            float finalDamage = DamageCalculator.CalculateDamage(damageInfo);
            _player.TakeDamage(Mathf.RoundToInt(finalDamage));
        }
        else
        {
            // Fallback to direct damage if entity not available
            _player.TakeDamage(CalculateDamage());
        }
        
        // Take damage
        int previousHp = m_CurrentHp;
        m_CurrentHp -= m_DamagePerHit;
        m_Entity.SetAttribute(AttributeType.CurrentHealth, m_CurrentHp);
        
        // Rest of existing code...
    }
    
    // Additional methods to apply/remove buffs
    public void ApplyBuff(Buff buff)
    {
        m_Entity.ApplyBuff(buff);
    }
    
    public void RemoveBuff(Buff buff)
    {
        m_Entity.RemoveBuff(buff);
    }
    
    public void OnEntityTurnEnd()
    {
        m_Entity.OnTurnEnd();
    }
}

// Entity implementation specific to Monster
public class MonsterEntity : Entity
{
    private MonsterMine m_Monster;
    private MonsterType m_MonsterType;
    
    public MonsterType MonsterType => m_MonsterType;
    
    public MonsterEntity(MonsterMine monster, string name, MonsterType monsterType) : base(name)
    {
        m_Monster = monster;
        m_MonsterType = monsterType;
    }
    
    protected override void InitializeAttributes()
    {
        // Attributes will be initialized by MonsterMine.InitializeEntityAttributes()
    }
    
    // Override to sync health changes with monster
    public override void ModifyHealth(int amount)
    {
        // Update the attribute
        var currentHealth = GetAttribute(AttributeType.CurrentHealth);
        if (currentHealth != null)
        {
            float newHealth = currentHealth.CurrentValue + amount;
            currentHealth.SetBaseValue(newHealth);
            
            // Sync with monster's HP
            m_Monster.CurrentHp = (int)newHealth;
        }
    }
}
```

#### Option 2: Interface Adapter Approach

If composition is too invasive, we can create adapter interfaces to bridge the systems:

```csharp
// Define an adapter interface
public interface IEntityAdapter
{
    Entity Entity { get; }
}

// Implement for Player
public class PlayerEntityAdapter : IEntityAdapter
{
    private readonly Player m_Player;
    private readonly Entity m_Entity;
    
    public Entity Entity => m_Entity;
    
    public PlayerEntityAdapter(Player player)
    {
        m_Player = player;
        m_Entity = new PlayerEntity(player);
        
        // Set up synchronization between player and entity
        // This could involve event subscriptions or periodic sync
    }
}

// Similarly for MonsterMine
public class MonsterEntityAdapter : IEntityAdapter
{
    private readonly MonsterMine m_Monster;
    private readonly Entity m_Entity;
    
    public Entity Entity => m_Entity;
    
    public MonsterEntityAdapter(MonsterMine monster)
    {
        m_Monster = monster;
        m_Entity = new MonsterEntity(monster);
    }
}
```

#### Option 3: Registry Approach

For minimal code changes, maintain a central registry mapping existing objects to their entity representations:

```csharp
// Central registry to map game objects to entities
public static class EntityRegistry
{
    private static readonly Dictionary<object, Entity> s_EntityMap = new();
    
    public static void Register(object gameObject, Entity entity)
    {
        s_EntityMap[gameObject] = entity;
    }
    
    public static void Unregister(object gameObject)
    {
        s_EntityMap.Remove(gameObject);
    }
    
    public static Entity GetEntity(object gameObject)
    {
        return s_EntityMap.TryGetValue(gameObject, out var entity) ? entity : null;
    }
    
    public static T GetOrCreateEntity<T>(object gameObject) where T : Entity, new()
    {
        if (!s_EntityMap.TryGetValue(gameObject, out var entity))
        {
            entity = new T();
            s_EntityMap[gameObject] = entity;
        }
        
        return (T)entity;
    }
}

// Usage example:
// var playerEntity = EntityRegistry.GetOrCreateEntity<PlayerEntity>(player);
// playerEntity.ApplyBuff(buff);
```

## Recommended Implementation Approach

For the best balance of flexibility and integration effort, we recommend the Composition approach (Option 1). This approach:

1. Adds Entity objects to existing Player and MonsterMine classes
2. Updates damage calculation to use the new system when available
3. Maintains backward compatibility with existing code
4. Allows for gradual adoption of the buff and class systems

The composition approach minimizes the need for major refactoring while providing all the benefits of the new attribute, buff, and damage calculation systems.

## Examples

### Example Buffs

```csharp
// Strength buff: Increases attack by 20% for 3 turns
var strengthBuff = new Buff(
    "Strength", 
    "Increases attack by 20% for 3 turns", 
    strengthIcon, 
    false, 
    3, 
    1, 
    BuffStackType.None,
    new[] { new AttributeModifier(AttributeType.Attack, 0.2f, ModifierType.Percent, this) }
);

// Poison debuff: Deals 5 damage per turn for 3 turns
var poisonBuff = new Buff(
    "Poison",
    "Takes 5 damage at the end of each turn",
    poisonIcon,
    true,
    3,
    3,
    BuffStackType.Stack,
    null,
    null,
    null,
    entity => entity.ModifyHealth(-5)
);
```

### Example Damage Calculation

```csharp
// Player attacks enemy
var player = GetPlayer();
var enemy = GetEnemy();

var damageInfo = new DamageInfo(player, enemy, 0, DamageType.Physical);
float finalDamage = DamageCalculator.CalculateDamage(damageInfo);

enemy.TakeDamage(finalDamage);
```

## Implementation Plan

1. **Phase 1: Core Systems**
   - Implement Attribute system
   - Implement Entity system
   - Implement Modifier system

2. **Phase 2: Buff and Trait Systems**
   - Implement Buff system
   - Implement Trait system
   - Integrate with existing Effect system

3. **Phase 3: Damage Calculation**
   - Implement DamageInfo and DamageCalculator
   - Implement IDamageModifier interface and handlers
   - Create standard damage formulas

4. **Phase 4: Integration**
   - Create Entity implementations for Player and MonsterMine
   - Implement composition pattern to add Entity support to existing classes
   - Update damage interactions to use the new calculation system
   - Create adapter code for smooth transition

5. **Phase 5: UI and Visualization**
   - Create UI for displaying active buffs
   - Implement visual effects for buffs/debuffs
   - Add tooltips for buff information

## Conclusion

This buff and damage calculation system provides a flexible foundation for complex game mechanics. It integrates well with the existing architecture while introducing powerful new capabilities. The modular design allows for easy extension and modification as the game evolves.

Key advantages of this design:
- Clear separation of concerns between attributes, modifiers, and buffs
- Support for various stacking behaviors and duration mechanics
- Extensible damage calculation with modifier chain
- Integration with existing systems (Effects, TurnSystem)
- Flexible trait system for permanent character modifications
- Non-invasive integration with existing Player and MonsterMine classes

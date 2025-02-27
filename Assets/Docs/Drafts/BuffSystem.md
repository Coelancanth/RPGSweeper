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
   - Refactor PlayerStats to use new Attribute system
   - Update TurnManager for buff processing
   - Create standard library of buffs and traits

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

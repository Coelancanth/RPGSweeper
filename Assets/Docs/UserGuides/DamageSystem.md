# Damage System User Guide

## Table of Contents
1. [Introduction](#introduction)
2. [Architecture Overview](#architecture-overview)
3. [Core Components](#core-components)
4. [Data Flow](#data-flow)
5. [Control Flow](#control-flow)
6. [Basic Usage](#basic-usage)
7. [Advanced Usage](#advanced-usage)
8. [Integration Examples](#integration-examples)
9. [Extending the System](#extending-the-system)
10. [Debugging and Troubleshooting](#debugging-and-troubleshooting)
11. [Best Practices](#best-practices)
12. [API Reference](#api-reference)
13. [FAQ](#faq)

## Introduction

The Damage System provides a robust and extensible framework for calculating and applying damage between entities in your game. Designed with SOLID principles in mind, it separates concerns into distinct modules with well-defined responsibilities.

**Key Benefits:**

- **Modularity**: Easily extend the system with new damage types, calculation logic, or effects
- **Flexibility**: Configure different damage behaviors without modifying code
- **Consistency**: Centralized damage processing ensures uniform behavior across the game
- **Testability**: Isolated components are easy to test independently
- **Maintainability**: Clear separation of concerns simplifies maintenance and debugging

## Architecture Overview

The Damage System architecture is based on a pipeline pattern, where damage information flows through a series of processors that each handle a specific aspect of the calculation.

```
┌─────────────┐     ┌───────────────┐     ┌────────────────┐       ┌─────────────┐
│ Entity with │     │               │     │                │       │ Entity with │
│ Attributes  │───▶│ DamageInfo    │───▶│ Calculation    │───▶  │ Modified    │
│ (Source)    │     │ Creation      │     │ Pipeline       │       │ Attributes  │
└─────────────┘     └───────────────┘     └────────────────┘       │ (Target)    │
                                                                   └─────────────┘
```

For more complex interactions, the Combat Interaction System provides higher-level abstractions to handle bidirectional damage and complete combat encounters:

```
┌─────────────┐                                               ┌─────────────┐
│ Monster     │◄───┐                                    ┌────▶│ Player      │
│ Entity      │    │                                    │     │ Component   │
└─────────────┘    │                                    │     └─────────────┘
       │           │                                    │            │
       ▼           │                                    │            ▼
┌─────────────────────────┐                      ┌─────────────────────────┐
│ Monster to Player Damage │                      │ Player to Monster Damage │
└─────────────────────────┘                      └─────────────────────────┘
       │                                                       │
       └───────────────┬───────────────────────────────────────┘
                       │
                       ▼
          ┌───────────────────────┐
          │ CombatInteractionSystem│
          └───────────────────────┘
                       │
                       ▼
          ┌───────────────────────┐
          │     CombatResult      │
          └───────────────────────┘
```

### Main Subsystems

1. **Entity-Attribute System**: Manages entities and their attributes
2. **Damage Calculation Pipeline**: Processes damage calculations through modular steps
3. **Damage Application System**: Applies the calculated damage to target entities
4. **Combat Interaction System**: Manages complete interactions between entities 
5. **Initialization System**: Creates and initializes entities with appropriate attributes
6. **Factory System**: Simplifies creation of complex objects within the system

## Core Components

### Entity System

The foundation of the Damage System is the Entity-Attribute model:

- **Entity**: Base class representing any game object that can deal or receive damage
- **Attribute**: Class representing a single numeric attribute with modifiable value
- **AttributeType**: Class representing the type/identity of an attribute
- **AttributeModifier**: Class representing temporary or permanent modifications to attribute values

```csharp
// Core entity structure
public class Entity : IEntity
{
    private Dictionary<AttributeType, Attribute> _attributes;
    public string Name { get; }
    public event Action<Entity, Attribute, float, float> OnAttributeValueChanged;
    
    // Methods for attribute management
    public Attribute GetAttribute(AttributeType type);
    public bool HasAttribute(AttributeType type);
    public Attribute SetAttribute(AttributeType type, float value);
    public Attribute AddAttribute(AttributeType type, float baseValue = 0);
    public IEnumerable<Attribute> GetAllAttributes();
}
```

#### Specialized Entities

- **MonsterEntity**: Extended entity class for monsters with specific behaviors
- **PlayerEntity**: Extended entity class for the player character

### DamageInfo

The `DamageInfo` class is the central data object that flows through the system:

```csharp
public class DamageInfo
{
    // Source and Target
    public Entity Source { get; set; }
    public Entity Target { get; set; }
    public DamageType Type { get; set; }
    
    // Damage values at different calculation stages
    public float BaseDamage { get; set; }
    public float ModifiedDamage { get; set; }
    public float FinalDamage { get; set; }
    
    // Calculation modifiers
    public float DamageAddition { get; set; }
    public float DamageMultiplier { get; set; } = 1.0f;
    public float ResistanceMultiplier { get; set; } = 1.0f;
    
    // Special states
    public bool IsCritical { get; set; }
    public float CriticalMultiplier { get; set; } = 1.5f;
    public bool IsEnraged { get; set; }
    public float EnrageMultiplier { get; set; } = 1.0f;
    public float ResistanceReduction { get; set; }
    public bool IsPlayerDamage { get; set; }
    public bool TargetDefeated { get; set; }
    
    // Additional information
    public List<StatusEffectInfo> StatusEffects { get; }
    public Dictionary<string, object> Metadata { get; }
    
    // Methods
    public DamageInfo Clone();
}
```

### Damage Types

The `DamageType` enum defines different types of damage:

```csharp
public enum DamageType
{
    Physical,
    Magical,
    Fire,
    Ice,
    Lightning,
    Poison,
    True  // Ignores resistances
}
```

### Predefined Attribute Types

The `AttributeTypes` static class provides standardized attribute types used throughout the system:

```csharp
public static class AttributeTypes
{
    // Core attributes
    public static readonly AttributeType MAX_HP = new AttributeType("MaxHP");
    public static readonly AttributeType CURRENT_HP = new AttributeType("CurrentHP");
    public static readonly AttributeType BASE_DAMAGE = new AttributeType("BaseDamage");
    
    // Combat attributes
    public static readonly AttributeType ATTACK_POWER = new AttributeType("AttackPower");
    public static readonly AttributeType DEFENSE = new AttributeType("Defense");
    public static readonly AttributeType CRITICAL_CHANCE = new AttributeType("CriticalChance");
    public static readonly AttributeType CRITICAL_DAMAGE = new AttributeType("CriticalDamage");
    
    // Resistances
    public static readonly AttributeType PHYSICAL_RESISTANCE = new AttributeType("PhysicalResistance");
    public static readonly AttributeType MAGICAL_RESISTANCE = new AttributeType("MagicalResistance");
    public static readonly AttributeType FIRE_RESISTANCE = new AttributeType("FireResistance");
    public static readonly AttributeType ICE_RESISTANCE = new AttributeType("IceResistance");
    
    // Damage type to resistance mapping
    public static readonly Dictionary<DamageType, AttributeType> ResistanceTypes;
    
    // Helper method to get resistance type for a damage type
    public static AttributeType GetResistanceType(DamageType damageType);
}
```

### Damage Calculation Pipeline

The calculation pipeline consists of a series of processors, each handling a specific aspect of damage calculation:

```csharp
// Base interface for all damage processors
public interface IDamageProcessor
{
    DamageInfo Process(DamageInfo damageInfo);
}

// Standard processors
public class BaseDamageProcessor : IDamageProcessor { /* ... */ }
public class EnrageProcessor : IDamageProcessor { /* ... */ }
public class ResistanceProcessor : IDamageProcessor { /* ... */ }
public class CriticalHitProcessor : IDamageProcessor { /* ... */ }
public class FinalDamageProcessor : IDamageProcessor { /* ... */ }
```

### Damage Application

The application system applies calculated damage to targets:

```csharp
// Interface for damage appliers
public interface IDamageApplier
{
    void ApplyDamage(DamageInfo damageInfo);
}

// Standard applier implementation
public class DamageApplier : IDamageApplier { /* ... */ }

// Specialized appliers in Processors namespace
namespace Processors {
    public interface IDamageApplier
    {
        bool ApplyDamage(DamageInfo damageInfo);
    }
    
    public class BasicDamageApplier : IDamageApplier { /* ... */ }
    public class StatusEffectApplier : IDamageApplier { /* ... */ }
}
```

### Combat Interaction System

The combat interaction system provides a higher-level abstraction for handling complete combat interactions between entities:

```csharp
// Main system class for combat interactions
public static class CombatInteractionSystem
{
    // Events for monitoring combat interactions
    public static event Action<MonsterEntity, PlayerComponent, float> OnMonsterDamagePlayer;
    public static event Action<PlayerComponent, MonsterEntity, float> OnPlayerDamageMonster;
    public static event Action<MonsterEntity> OnMonsterDefeated;
    
    // Core method for handling combat interactions
    public static CombatResult HandleMonsterPlayerInteraction(MonsterEntity monsterEntity, PlayerComponent player);
}

// Data class for combat interaction results
public class CombatResult
{
    public int DamageToPlayer { get; set; }
    public int DamageToMonster { get; set; }
    public int ActualDamageToMonster { get; set; }
    public bool MonsterDefeated { get; set; }
    public bool EnrageStateChanged { get; set; }
    public bool IsEnraged { get; set; }
}
```

### Factories and Initializers

Factories simplify the creation of complex objects:

```csharp
// DamageInfo factory
public static class DamageInfoFactory
{
    public static DamageInfo CreateMonsterToPlayerDamage(/*...*/);
    public static DamageInfo CreatePlayerToMonsterDamage(/*...*/);
    public static DamageInfo CreateEntityToEntityDamage(/*...*/);
}

// Attribute initializer interface
public interface IAttributeInitializer
{
    void InitializeAttributes(Entity entity);
}

// Example initializers
public class SlimeAttributeInitializer : BaseMonsterAttributeInitializer { /* ... */ }
public class DragonAttributeInitializer : BaseMonsterAttributeInitializer { /* ... */ }

// Initializer factory
public static class AttributeInitializerFactory
{
    public enum MonsterType { Default, Slime, Spider, Dragon }
    public static IAttributeInitializer CreateMonsterInitializer(MonsterType type);
    public static MonsterEntity CreateMonsterEntity(MonsterType type, string name = null);
}
```

## Data Flow

The data flow in the Damage System follows a clear path from source to target:

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           DamageInfo Object                             │
├─────────────┬────────────┬───────────────┬───────────────┬──────────────┤
│ Source      │ Target     │ Base Values   │ Modifiers     │ Final Values │
│ - Entity    │ - Entity   │ - BaseDamage  │ - Multipliers │ - FinalDamage│
│ - Stats     │ - Stats    │ - DamageType  │ - Additions   │ - Status     │
└─────────────┴────────────┴───────────────┴───────────────┴──────────────┘
                     │                 │                    │
                    ▼                 ▼                   ▼
┌─────────────┐     ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ Base        │───▶│ Combat State│──▶│ Resistance  │──▶│ Final       │
│ Calculation │     │ Processing  │    │ Calculation │    │ Processing  │
└─────────────┘     └─────────────┘    └─────────────┘    └─────────────┘
                    │                                           │
                    ▼                                          ▼
┌─────────────────────────┐                ┌─────────────────────────┐
│ Metadata Updates        │                │ Target HP Reduction     │
└─────────────────────────┘                └─────────────────────────┘
```

### DamageInfo Lifecycle

1. **Creation**: A `DamageInfo` object is created with initial values
   - Source entity (attacker)
   - Target entity (defender)
   - Base damage amount
   - Damage type
   - Initial state flags (IsEnraged, IsPlayerDamage, etc.)

2. **Base Processing**: BaseDamageProcessor establishes initial damage value
   - Sets ModifiedDamage = BaseDamage + DamageAddition
   - Stores raw damage in Metadata for later stages

3. **State Processing**: Special state processors apply relevant modifiers
   - EnrageProcessor applies enrage multiplier if source is enraged
   - CriticalHitProcessor determines if hit is critical and applies multiplier

4. **Resistance Processing**: ResistanceProcessor applies damage reduction
   - Gets target's resistance for the specific damage type
   - Calculates resistance multiplier based on resistance value
   - Updates ModifiedDamage with resistance reduction

5. **Final Processing**: FinalDamageProcessor ensures valid final values
   - Ensures damage is never negative
   - Copies final value to FinalDamage property
   - Applies any final adjustments (rounding, etc.)

6. **Application**: DamageApplier applies the damage to target
   - Reduces target's CURRENT_HP attribute by FinalDamage
   - Sets TargetDefeated flag if HP reaches zero

7. **Effects Application**: StatusEffectApplier applies any additional effects
   - Processes each StatusEffectInfo in the DamageInfo
   - Applies effects based on their chance, duration, and strength

## Control Flow

The control flow of the Damage System can be understood as a series of stages:

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│              │     │              │     │              │     │              │
│  Initialize  │────▶│  Calculate   │────▶│    Apply     │────▶│   Update     │
│              │     │              │     │              │     │              │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
      │                     │                    │                    │
      ▼                     ▼                    ▼                    ▼
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Create       │     │ Process      │     │ Modify       │     │ Check for    │
│ Entities     │     │ Through      │     │ Target       │     │ Defeat       │
│ and Damage   │     │ Pipeline     │     │ Attributes   │     │ and Effects  │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
```

For more complex interactions, the CombatInteractionSystem simplifies this flow into a single operation:

```
┌──────────────┐     ┌──────────────────────────────┐     ┌──────────────┐
│              │     │                              │     │              │
│  Entities    │────▶│  CombatInteractionSystem     │────▶│  Result      │
│              │     │                              │     │  Processing  │
└──────────────┘     └──────────────────────────────┘     └──────────────┘
```

### Processing Steps

1. **Creation Phase**:
   - Create/obtain source and target entities
   - Create DamageInfo using appropriate factory method
   - Set initial values and flags

2. **Calculation Phase**:
   - Pass DamageInfo through DamageSystem.CalculateDamage()
   - Each processor updates the DamageInfo state
   - Final damage value is determined

3. **Application Phase**:
   - Pass calculated DamageInfo to DamageSystem.ApplyDamage()
   - Target's CURRENT_HP attribute is reduced
   - StatusEffects are applied
   - TargetDefeated flag is set if appropriate

4. **Post-Processing Phase**:
   - Check target's defeat status
   - Update target's visual representation
   - Trigger relevant events
   - Process any game state changes

## Basic Usage

### Creating and Initializing Entities

```csharp
// Create a player entity
var player = new PlayerEntity("Hero");

// Create a monster entity using the factory
var monster = AttributeInitializerFactory.CreateMonsterEntity(
    AttributeInitializerFactory.MonsterType.Dragon, 
    "Ancient Dragon"
);

// Or using the adapter for game-specific monster types
var gameMonster = MonsterTypeAdapter.CreateMonsterEntity(
    RPGMinesweeper.MonsterType.Dragon,
    "Fire Breather"
);
```

### Creating Damage Info

```csharp
// Method 1: Direct creation
var damageInfo = new DamageInfo
{
    Source = player,
    Target = monster,
    Type = DamageType.Physical,
    BaseDamage = 25.0f,
    IsCritical = true
};

// Method 2: Using factory (recommended)
var damageInfo = DamageInfoFactory.CreateEntityToEntityDamage(
    player,                 // Source entity
    monster,                // Target entity
    25.0f,                  // Base damage amount
    DamageType.Physical     // Damage type
);

// Method 3: Using specialized factory methods
var playerAttack = DamageInfoFactory.CreatePlayerToMonsterDamage(
    player, monster, 25.0f, DamageType.Physical
);

var monsterAttack = DamageInfoFactory.CreateMonsterToPlayerDamage(
    monster, player, DamageType.Fire
);
```

### Calculating and Applying Damage

```csharp
// Method 1: Separate calculation and application
var calculatedDamage = DamageSystem.CalculateDamage(damageInfo);
bool success = DamageSystem.ApplyDamage(calculatedDamage);

// Method 2: Combined calculation and application (recommended)
DamageSystem.CalculateAndApplyDamage(damageInfo);

// Method 3: Using DamageApplier directly
var calculator = new DamageCalculator();
var applier = new DamageApplier(calculator);
applier.ApplyDamage(damageInfo);
```

### Checking Results

```csharp
// Check final damage amount
float damageDealt = damageInfo.FinalDamage;
Console.WriteLine($"Dealt {damageDealt} damage of type {damageInfo.Type}");

// Check if target was defeated
if (damageInfo.TargetDefeated)
{
    Console.WriteLine($"{damageInfo.Target.Name} was defeated!");
}

// Check target's remaining HP
float remainingHp = monster.GetAttribute(AttributeTypes.CURRENT_HP).CurrentValue;
float maxHp = monster.GetAttribute(AttributeTypes.MAX_HP).CurrentValue;
Console.WriteLine($"Target HP: {remainingHp}/{maxHp} ({(remainingHp/maxHp)*100}%)");
```

### Monitoring Attribute Changes

```csharp
// Subscribe to attribute changes on an entity
monster.OnAttributeValueChanged += HandleAttributeChanged;

// Event handler
void HandleAttributeChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
{
    Console.WriteLine($"{entity.Name}'s {attribute.Type.Name} changed from {oldValue} to {newValue}");
    
    // Check if it was HP that changed
    if (attribute.Type == AttributeTypes.CURRENT_HP)
    {
        UpdateHealthBar(entity, newValue, entity.GetAttribute(AttributeTypes.MAX_HP).CurrentValue);
    }
}
```

### Using the Combat Interaction System

The CombatInteractionSystem provides a higher-level abstraction for handling complete combat interactions:

```csharp
// Get references to the entities involved
MonsterEntity monster = /* get monster entity */;
PlayerComponent player = /* get player component */;

// Handle the complete combat interaction
var result = CombatInteractionSystem.HandleMonsterPlayerInteraction(monster, player);

// Check results
if (result != null)
{
    Debug.Log($"Player took {result.DamageToPlayer} damage");
    Debug.Log($"Monster took {result.ActualDamageToMonster} damage");
    
    if (result.MonsterDefeated)
    {
        Debug.Log("Monster was defeated!");
        // Handle defeat (show rewards, etc.)
    }
    
    if (result.EnrageStateChanged && result.IsEnraged)
    {
        Debug.Log("Monster has become enraged!");
        // Update UI to show enraged state
    }
}

// Subscribe to combat events for global monitoring
CombatInteractionSystem.OnMonsterDefeated += (defeatedMonster) => {
    // Track statistics, update quest progress, etc.
    GameStats.MonsterDefeated(defeatedMonster);
};
```

## Advanced Usage

### Custom Damage Types

```csharp
// Add new damage type to the enum
public enum DamageType
{
    // Existing types...
    Radiant,    // New type
    Necrotic    // New type
}

// Update AttributeTypes class
public static class AttributeTypes
{
    // Existing types...
    public static readonly AttributeType RADIANT_RESISTANCE = new AttributeType("RadiantResistance");
    public static readonly AttributeType NECROTIC_RESISTANCE = new AttributeType("NecroticResistance");
    
    // Update the resistances dictionary
    static AttributeTypes()
    {
        ResistanceTypes = new Dictionary<DamageType, AttributeType>
        {
            // Existing mappings...
            { DamageType.Radiant, RADIANT_RESISTANCE },
            { DamageType.Necrotic, NECROTIC_RESISTANCE }
        };
    }
}
```

### Custom Damage Processor

```csharp
/// <summary>
/// Processor that handles element-specific damage bonuses
/// </summary>
public class ElementalBonusProcessor : IDamageProcessor
{
    private readonly Dictionary<DamageType, float> _bonusMultipliers = new Dictionary<DamageType, float>
    {
        { DamageType.Fire, 1.2f },
        { DamageType.Ice, 1.1f },
        { DamageType.Lightning, 1.3f }
    };
    
    public DamageInfo Process(DamageInfo damageInfo)
    {
        // Check if this damage type has a bonus
        if (_bonusMultipliers.TryGetValue(damageInfo.Type, out float bonus))
        {
            // Apply the bonus multiplier
            damageInfo.DamageMultiplier *= bonus;
            
            // Add to metadata for debugging
            damageInfo.Metadata["ElementalBonus"] = bonus;
        }
        
        return damageInfo;
    }
}

// Add the processor to the pipeline
DamageSystem.DamageProcessors.Add(new ElementalBonusProcessor());
```

### Custom Attribute Initialization

```csharp
/// <summary>
/// Custom initializer for undead monsters
/// </summary>
public class UndeadMonsterInitializer : BaseMonsterAttributeInitializer
{
    public override void InitializeAttributes(Entity entity)
    {
        base.InitializeAttributes(entity);
        
        // Override default values
        entity.SetAttribute(AttributeTypes.MAX_HP, 15);
        entity.SetAttribute(AttributeTypes.CURRENT_HP, 15);
        entity.SetAttribute(AttributeTypes.BASE_DAMAGE, 3);
        
        // Undead are resistant to physical and necrotic damage
        entity.SetAttribute(AttributeTypes.PHYSICAL_RESISTANCE, 30);
        entity.SetAttribute(AttributeTypes.NECROTIC_RESISTANCE, 90);
        
        // But weak against radiant damage
        entity.SetAttribute(AttributeTypes.RADIANT_RESISTANCE, -50);
        
        // Custom attribute for undead
        entity.AddAttribute(new AttributeType("UndeadRevivalChance"), 25); // 25% chance to revive
    }
}

// Register with factory
public static IAttributeInitializer CreateMonsterInitializer(MonsterType type)
{
    switch (type)
    {
        // Existing cases...
        case MonsterType.Undead:
            return new UndeadMonsterInitializer();
        default:
            return new DefaultMonsterAttributeInitializer();
    }
}
```

### Working with Status Effects

```csharp
// Create damage info
var damageInfo = DamageInfoFactory.CreateEntityToEntityDamage(
    wizard, 
    monster, 
    15.0f, 
    DamageType.Fire
);

// Add a burn status effect
damageInfo.StatusEffects.Add(new StatusEffectInfo
{
    EffectType = "Burn",
    Duration = 3.0f,     // 3 seconds
    Strength = 5.0f,     // 5 damage per tick
    ApplyChance = 0.75f  // 75% chance to apply
});

// Calculate and apply, including status effects
DamageSystem.CalculateAndApplyDamage(damageInfo);

// Check if any status effects were applied
bool effectApplied = damageInfo.StatusEffects.Any(e => e.WasApplied);
```

### Attribute Modifiers

```csharp
// Get an entity's attribute
var attackAttribute = player.GetAttribute(AttributeTypes.ATTACK_POWER);

// Create a temporary buff (flat addition)
var strengthPotion = new AttributeModifier(
    "Strength Potion", 
    10.0f,               // +10 to attack
    AttributeModifierType.Flat
);

// Apply the modifier
attackAttribute.AddModifier(strengthPotion);
Console.WriteLine($"Attack increased to {attackAttribute.CurrentValue}");

// Create a percentage-based modifier
var berserkBuff = new AttributeModifier(
    "Berserk Buff",
    0.25f,               // +25% to attack
    AttributeModifierType.Percentage
);

// Apply the percentage modifier
attackAttribute.AddModifier(berserkBuff);
Console.WriteLine($"Attack further increased to {attackAttribute.CurrentValue}");

// Remove a modifier
attackAttribute.RemoveModifier(strengthPotion);
Console.WriteLine($"Attack decreased to {attackAttribute.CurrentValue}");

// Clear all modifiers
attackAttribute.ClearModifiers();
Console.WriteLine($"Attack reset to {attackAttribute.CurrentValue}");
```

## Integration Examples

### Integration with MonsterMine

```csharp
public class MonsterMine : IDamagingMine
{
    private MonsterEntity m_Entity;
    
    // Constructor
    public MonsterMine(MonsterMineData _data, Vector2Int _position)
    {
        // Initialize basic fields...
        
        // Create the monster entity
        InitializeMonsterEntity();
        
        // Subscribe to attribute changes
        m_Entity.OnAttributeValueChanged += HandleAttributeValueChanged;
    }
    
    // Initialize the monster entity
    private void InitializeMonsterEntity()
    {
        // Create entity based on monster type
        m_Entity = MonsterTypeAdapter.CreateMonsterEntity(
            m_Data.MonsterType,
            $"Monster_{m_Position.x}_{m_Position.y}"
        );
    }
    
    // CalculateDamage method - implements IDamagingMine
    public int CalculateDamage()
    {
        // Check if we should enter enrage state
        if (m_Entity is MonsterEntity monsterEntity)
        {
            monsterEntity.UpdateEnrageState();
            
            // Create damage info using the factory
            var damageInfo = DamageInfoFactory.CreateMonsterToPlayerDamage(
                monsterEntity,
                null, // No specific player target yet
                DamageType.Physical
            );
            
            // Calculate damage through the damage system
            damageInfo = DamageSystem.CalculateDamage(damageInfo);
            
            // Return calculated damage as an integer
            return Mathf.RoundToInt(damageInfo.FinalDamage);
        }
        
        // Fallback to base damage if entity isn't a MonsterEntity
        return Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0);
    }
    
    // Handle player triggering the mine - implements IMine
    public void OnTrigger(PlayerComponent _player)
    {
        // Handle state and collection checks
        if (m_IsDefeated && !m_IsCollectable)
        {
            m_IsCollectable = true;
            OnHpChanged?.Invoke(m_Position, 0);
            return;
        }
        
        if (m_IsDefeated && m_IsCollectable)
        {
            return;
        }
        
        // Record previous HP
        int previousHp = CurrentHp;
        
        // Handle combat interaction between monster and player
        HandleCombatInteraction(_player);
        
        // Handle effects based on HP changes
        if (previousHp != CurrentHp)
        {
            // Notify split effects of HP change
            foreach (var effect in m_ActivePersistentEffects)
            {
                if (effect is SplitEffect splitEffect)
                {
                    splitEffect.OnMonsterDamaged(m_Position, HpPercentage);
                }
            }
        }
        
        // Apply new effects if monster is still alive
        if (CurrentHp > 0)
        {
            foreach (var effect in m_Data.CreatePersistentEffects())
            {
                if (effect is IPersistentEffect persistentEffect)
                {
                    persistentEffect.Apply(_player.gameObject, m_Position);
                    m_ActivePersistentEffects.Add(persistentEffect);
                }
                else
                {
                    effect.Apply(_player.gameObject, m_Position);
                }
            }
        }
    }
    
    // Handle combat interaction using the CombatInteractionSystem
    private void HandleCombatInteraction(PlayerComponent _player)
    {
        if (!(m_Entity is MonsterEntity monsterEntity))
        {
            Debug.LogWarning($"Entity for monster at {m_Position} is not a MonsterEntity");
            return;
        }
        
        // Use the combat interaction system to handle all damage calculations and application
        var combatResult = CombatInteractionSystem.HandleMonsterPlayerInteraction(monsterEntity, _player);
        
        // Handle interaction results
        if (combatResult == null)
        {
            Debug.LogWarning($"Failed to handle combat interaction for monster at {m_Position}");
            return;
        }
        
        // Update defeated state based on result
        if (combatResult.MonsterDefeated && !m_IsDefeated)
        {
            m_IsDefeated = true;
            OnDefeated?.Invoke(m_Position);
        }
        
        // Trigger enraged event if state changed to enraged
        if (combatResult.EnrageStateChanged && combatResult.IsEnraged)
        {
            OnEnraged?.Invoke(m_Position);
        }
    }
    
    // Handle attribute value changes
    private void HandleAttributeValueChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
    {
        // Handle HP changes
        if (attribute.Type == AttributeTypes.CURRENT_HP)
        {
            // Notify listeners of HP change
            OnHpChanged?.Invoke(m_Position, m_Entity.GetHpPercentage());
        }
    }
}
```

### Integration with UI

```csharp
public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Text healthText;
    
    private Entity _trackedEntity;
    
    public void Initialize(Entity entity)
    {
        // Unsubscribe from previous entity if any
        if (_trackedEntity != null)
        {
            _trackedEntity.OnAttributeValueChanged -= HandleAttributeChanged;
        }
        
        // Store reference to new entity
        _trackedEntity = entity;
        
        // Subscribe to attribute changes
        _trackedEntity.OnAttributeValueChanged += HandleAttributeChanged;
        
        // Initial update
        UpdateHealthDisplay();
    }
    
    private void HandleAttributeChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
    {
        // Only care about HP changes
        if (attribute.Type == AttributeTypes.CURRENT_HP || attribute.Type == AttributeTypes.MAX_HP)
        {
            UpdateHealthDisplay();
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (_trackedEntity == null) return;
        
        // Get current and max HP
        float currentHP = _trackedEntity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue ?? 0;
        float maxHP = _trackedEntity.GetAttribute(AttributeTypes.MAX_HP)?.CurrentValue ?? 1;
        
        // Calculate fill percentage
        float fillAmount = Mathf.Clamp01(currentHP / maxHP);
        
        // Update UI elements
        healthFill.fillAmount = fillAmount;
        healthText.text = $"{Mathf.CeilToInt(currentHP)}/{Mathf.CeilToInt(maxHP)}";
        
        // Change color based on health percentage
        if (fillAmount < 0.3f)
            healthFill.color = Color.red;
        else if (fillAmount < 0.6f)
            healthFill.color = Color.yellow;
        else
            healthFill.color = Color.green;
    }
    
    private void OnDestroy()
    {
        // Clean up event subscription
        if (_trackedEntity != null)
        {
            _trackedEntity.OnAttributeValueChanged -= HandleAttributeChanged;
        }
    }
}
```

## Extending the System

The Damage System is designed to be extended in multiple ways:

### Adding New Damage Types

1. Add new value to the `DamageType` enum
2. Add corresponding resistance attribute type in `AttributeTypes`
3. Update the `ResistanceTypes` dictionary to map between them

### Creating Custom Processors

1. Implement the `IDamageProcessor` interface
2. Add your processor to the pipeline at runtime
3. Consider processor order - early processors can affect later ones

### Adding New Entity Types

1. Create a new class extending `Entity`
2. Implement specialized behavior and attributes
3. Create a corresponding initializer implementing `IAttributeInitializer`

### Extending Status Effects

1. Enhance the `StatusEffectInfo` class with new properties
2. Create a custom status effect applier
3. Implement the application logic for your effect

## Debugging and Troubleshooting

### Logging Damage Calculations

Use the `Metadata` dictionary to track processing steps:

```csharp
public class DebuggingProcessor : IDamageProcessor
{
    public DamageInfo Process(DamageInfo damageInfo)
    {
        // Store current state for debugging
        string processorName = GetType().Name;
        damageInfo.Metadata[$"{processorName}_Input"] = damageInfo.ModifiedDamage;
        
        // Your processing logic here
        
        // Store output state
        damageInfo.Metadata[$"{processorName}_Output"] = damageInfo.ModifiedDamage;
        
        return damageInfo;
    }
}
```

Then examine the metadata after processing:

```csharp
var result = DamageSystem.CalculateDamage(damageInfo);

// Print debug information
foreach (var item in result.Metadata)
{
    Debug.Log($"{item.Key}: {item.Value}");
}
```

### Common Issues and Solutions

#### Issue: Damage Not Being Applied

- Check that both source and target entities are properly initialized
- Verify that the target has a CURRENT_HP attribute
- Ensure damage calculation is not resulting in zero damage
- Check resistance values - high resistance can reduce damage to zero

#### Issue: Unexpected Damage Values

- Review the order of processors in the pipeline
- Check for unintended modifiers on source or target attributes
- Verify that damage type and resistance mappings are correct
- Add debugging processors to log intermediate values

## Best Practices

1. **Use Factory Methods**: Always use factory methods to create complex objects like `DamageInfo` or entities
2. **Follow Interface Contracts**: Implement interfaces completely and correctly
3. **Clean Up Event Subscriptions**: Always unsubscribe from events when no longer needed
4. **Processor Order Matters**: Be mindful of the order in processors pipeline
5. **Attribute Naming**: Use consistent naming for attributes across the game
6. **Error Handling**: Include null checks and validation in custom processors
7. **Avoid Magic Numbers**: Define constants for common values instead of using hardcoded numbers
8. **Testing**: Test processors individually before adding them to the pipeline

## API Reference

### Key Classes and Methods

#### DamageSystem

- `CalculateDamage(DamageInfo)`: Process damage through calculation pipeline
- `ApplyDamage(DamageInfo)`: Apply calculated damage to target
- `CalculateAndApplyDamage(DamageInfo)`: Combined calculation and application
- `CreateDamageInfo(Entity, Entity, DamageType, float)`: Create damage info between entities
- `DealMonsterDamageToPlayer(MonsterEntity, PlayerComponent, float)`: Deal damage to player component

#### CombatInteractionSystem

- `HandleMonsterPlayerInteraction(MonsterEntity, PlayerComponent)`: Handle complete combat interaction between monster and player
- `OnMonsterDamagePlayer`: Event triggered when monster damages player
- `OnPlayerDamageMonster`: Event triggered when player damages monster 
- `OnMonsterDefeated`: Event triggered when monster is defeated

#### DamageInfoFactory

- `CreateMonsterToPlayerDamage(MonsterEntity, PlayerComponent, DamageType)`: Create monster-to-player damage
- `CreatePlayerToMonsterDamage(PlayerComponent, MonsterEntity, float, DamageType)`: Create player-to-monster damage
- `CreateEntityToEntityDamage(Entity, Entity, float, DamageType)`: Create generic entity-to-entity damage

#### Entity

- `GetAttribute(AttributeType)`: Get an attribute by type
- `HasAttribute(AttributeType)`: Check if entity has an attribute
- `SetAttribute(AttributeType, float)`: Set attribute value
- `AddAttribute(AttributeType, float)`: Add a new attribute
- `GetAllAttributes()`: Get all attributes on entity

#### MonsterEntity

- `GetHpPercentage()`: Get current HP as percentage
- `IsEnraged()`: Check if monster is enraged
- `UpdateEnrageState()`: Update and return enrage state
- `ApplyDamage(float)`: Apply damage directly to monster
- `Heal(float)`: Heal monster by amount

#### Attribute

- `SetBaseValue(float)`: Set the base value of the attribute
- `AddModifier(AttributeModifier)`: Add a modifier to the attribute
- `RemoveModifier(AttributeModifier)`: Remove a modifier
- `ClearModifiers()`: Remove all modifiers

## FAQ

### General Questions

**Q: How do I create a new damage type?**
A: Add the type to the `DamageType` enum, create a corresponding resistance `AttributeType`, and update the `ResistanceTypes` dictionary in `AttributeTypes`.

**Q: When should I use the CombatInteractionSystem vs. direct DamageSystem calls?**
A: Use CombatInteractionSystem for complete interactions between entities that involve bidirectional damage (like monster-player combat encounters). Use direct DamageSystem calls for simpler one-directional damage calculations or when you need more fine-grained control over the damage process.

**Q: How do I extend the CombatInteractionSystem for different types of entities?**
A: Create new methods in CombatInteractionSystem that handle different entity type combinations, following the pattern established by HandleMonsterPlayerInteraction but adapting the logic for your specific entity types.

**Q: How do I make an entity immune to a damage type?**
A: Set their resistance attribute for that damage type to a very high value (e.g., 200), or use the `DamageType.True` type which ignores resistances.

**Q: How can I implement damage over time effects?**
A: Create a status effect that applies damage periodically. Implement a component that tracks active effects and applies damage each tick.

**Q: Can attributes have negative values?**
A: Yes, by default attributes can have negative values unless you explicitly set `MinValue` for the attribute.

### Performance Questions

**Q: Is this system suitable for large numbers of entities?**
A: The system is designed to be efficient for normal gameplay scenarios. For large-scale simulations, consider implementing a simplified version.

**Q: How can I optimize the damage calculation?**
A: Minimize the number of processors in the pipeline for common damage types, and consider caching damage calculations for identical scenarios.

### Integration Questions

**Q: How do I integrate this with Unity's Health system?**
A: Create a bridge component that synchronizes the CURRENT_HP attribute with Unity's health component.

**Q: Can I use this system with multiplayer games?**
A: Yes, but you'll need to ensure that damage calculations are performed on the server and results are synchronized to clients.

**Q: How do I extend this system for RPG-style abilities and skills?**
A: Create a skill system that uses the DamageSystem as its foundation. Skills can create and configure DamageInfo objects and process them through the system.

### Technical Questions

**Q: Why use an attribute-based system instead of direct properties?**
A: Attributes provide flexibility, modifiability, and event handling that direct properties lack. They support the modifier pattern for temporary buffs and debuffs.

**Q: How do I add a new processor to the pipeline?**
A: Implement the `IDamageProcessor` interface and add your processor to the `DamageSystem.DamageProcessors` list.

**Q: What's the difference between the main IDamageApplier and Processors.IDamageApplier?**
A: The main interface is for high-level damage application, while the Processors namespace interface is for specialized appliers in the pipeline.

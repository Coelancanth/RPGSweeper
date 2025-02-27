# Class System Design

## Overview

This document outlines the design for a flexible class system for the RPG Minesweeper game. The system allows players to equip multiple classes simultaneously, each providing unique abilities, passive skills, and attribute modifiers.

## Requirements

1. **Class Components**:
   - **Abilities**: Active skills that players can use
   - **Passives**: Automatic effects that are always active
   - **Stats**: Attribute modifiers that enhance character stats

2. **Hybrid Mechanics**:
   - Players can equip up to 2 classes simultaneously
   - Players can change classes at designated times
   - Main class provides all benefits (abilities, passives, and stats)
   - Sub-class provides only abilities and passives (no stat modifiers)

3. **Design Principles**:
   - Follow SOLID principles
   - Integrate with existing systems (Buff System, Effect System)
   - Provide clear interfaces for future extensions

## Core Concepts and Definitions

- **Class**: A collection of abilities, passives, and stat modifiers that define a playstyle
- **Ability**: An active skill that can be triggered by the player
- **Passive**: An automatic effect that's always active while the class is equipped
- **Main Class**: The primary class that provides all benefits (abilities, passives, stats)
- **Sub-Class**: A secondary class that provides only abilities and passives (no stats)

## System Architecture

### 1. Class Definition System

The class definition system defines the blueprint for all classes in the game.

```csharp
// Represent a game class (like Warrior, Mage, etc.)
public class ClassDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    
    private readonly List<AbilityDefinition> _abilities = new();
    private readonly List<PassiveDefinition> _passives = new();
    private readonly List<AttributeModifier> _statModifiers = new();
    
    public IReadOnlyList<AbilityDefinition> Abilities => _abilities.AsReadOnly();
    public IReadOnlyList<PassiveDefinition> Passives => _passives.AsReadOnly();
    public IReadOnlyList<AttributeModifier> StatModifiers => _statModifiers.AsReadOnly();
    
    public ClassDefinition(string id, string name, string description, Sprite icon)
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
    }
    
    public void AddAbility(AbilityDefinition ability)
    {
        _abilities.Add(ability);
    }
    
    public void AddPassive(PassiveDefinition passive)
    {
        _passives.Add(passive);
    }
    
    public void AddStatModifier(AttributeModifier modifier)
    {
        _statModifiers.Add(modifier);
    }
}
```

### 2. Ability System

The ability system defines active skills that players can use.

```csharp
// Base class for all abilities
public abstract class AbilityDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    public float Cooldown { get; }
    
    protected AbilityDefinition(string id, string name, string description, Sprite icon, float cooldown)
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
        Cooldown = cooldown;
    }
    
    // Factory method to create an ability instance for a specific entity
    public abstract Ability CreateInstance(Entity owner);
}

// Runtime instance of an ability attached to an entity
public abstract class Ability
{
    public AbilityDefinition Definition { get; }
    public Entity Owner { get; }
    public float CurrentCooldown { get; protected set; }
    public bool IsOnCooldown => CurrentCooldown > 0;
    
    protected Ability(AbilityDefinition definition, Entity owner)
    {
        Definition = definition;
        Owner = owner;
        CurrentCooldown = 0;
    }
    
    public virtual bool CanUse()
    {
        return !IsOnCooldown;
    }
    
    public virtual void Use(params object[] args)
    {
        if (!CanUse())
            return;
            
        ExecuteAbility(args);
        CurrentCooldown = Definition.Cooldown;
    }
    
    public virtual void UpdateCooldown(float deltaTime)
    {
        if (CurrentCooldown > 0)
            CurrentCooldown = Mathf.Max(0, CurrentCooldown - deltaTime);
    }
    
    protected abstract void ExecuteAbility(params object[] args);
}
```

### 3. Passive System

The passive system defines automatic effects that are always active while the class is equipped.

```csharp
// Base class for all passives
public abstract class PassiveDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    
    protected PassiveDefinition(string id, string name, string description, Sprite icon)
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
    }
    
    // Factory method to create a passive instance for a specific entity
    public abstract Passive CreateInstance(Entity owner);
}

// Runtime instance of a passive attached to an entity
public abstract class Passive
{
    public PassiveDefinition Definition { get; }
    public Entity Owner { get; }
    public bool IsActive { get; private set; }
    
    protected Passive(PassiveDefinition definition, Entity owner)
    {
        Definition = definition;
        Owner = owner;
        IsActive = false;
    }
    
    public virtual void Activate()
    {
        if (IsActive)
            return;
            
        IsActive = true;
        OnActivate();
    }
    
    public virtual void Deactivate()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        OnDeactivate();
    }
    
    // Called when the passive is activated
    protected abstract void OnActivate();
    
    // Called when the passive is deactivated
    protected abstract void OnDeactivate();
    
    // Called each frame/update while the passive is active
    public abstract void Update(float deltaTime);
}
```

### 4. Class Manager System

The class manager system manages the classes equipped by an entity.

```csharp
// Enum for class slot type
public enum ClassSlotType
{
    Main,
    Sub
}

// Manages the classes equipped by an entity
public class ClassManager
{
    private readonly Entity _owner;
    private ClassInstance _mainClass;
    private ClassInstance _subClass;
    
    private readonly List<Ability> _equippedAbilities = new();
    private readonly List<Passive> _equippedPassives = new();
    
    public ClassInstance MainClass => _mainClass;
    public ClassInstance SubClass => _subClass;
    
    public IReadOnlyList<Ability> EquippedAbilities => _equippedAbilities.AsReadOnly();
    public IReadOnlyList<Passive> EquippedPassives => _equippedPassives.AsReadOnly();
    
    // Event triggered when classes change
    public event Action<ClassSlotType, ClassInstance, ClassInstance> OnClassChanged;
    
    public ClassManager(Entity owner)
    {
        _owner = owner;
    }
    
    public void EquipClass(ClassDefinition classDefinition, ClassSlotType slotType)
    {
        // Create an instance of the class for this entity
        var classInstance = new ClassInstance(classDefinition, _owner);
        
        // Remove the previous class in this slot
        if (slotType == ClassSlotType.Main && _mainClass != null)
        {
            var oldClass = _mainClass;
            RemoveClass(ClassSlotType.Main);
            OnClassChanged?.Invoke(ClassSlotType.Main, oldClass, classInstance);
        }
        else if (slotType == ClassSlotType.Sub && _subClass != null)
        {
            var oldClass = _subClass;
            RemoveClass(ClassSlotType.Sub);
            OnClassChanged?.Invoke(ClassSlotType.Sub, oldClass, classInstance);
        }
        
        // Assign the new class to the appropriate slot
        if (slotType == ClassSlotType.Main)
        {
            _mainClass = classInstance;
            // Apply all benefits (abilities, passives, stats) for main class
            ApplyClassBenefits(classInstance, applyStats: true);
        }
        else // Sub class
        {
            _subClass = classInstance;
            // Apply only abilities and passives for sub class (no stats)
            ApplyClassBenefits(classInstance, applyStats: false);
        }
    }
    
    public void RemoveClass(ClassSlotType slotType)
    {
        ClassInstance classToRemove = null;
        
        if (slotType == ClassSlotType.Main && _mainClass != null)
        {
            classToRemove = _mainClass;
            _mainClass = null;
        }
        else if (slotType == ClassSlotType.Sub && _subClass != null)
        {
            classToRemove = _subClass;
            _subClass = null;
        }
        
        if (classToRemove != null)
        {
            RemoveClassBenefits(classToRemove);
            OnClassChanged?.Invoke(slotType, classToRemove, null);
        }
    }
    
    private void ApplyClassBenefits(ClassInstance classInstance, bool applyStats)
    {
        // Add abilities
        foreach (var ability in classInstance.Abilities)
        {
            _equippedAbilities.Add(ability);
        }
        
        // Activate passives
        foreach (var passive in classInstance.Passives)
        {
            passive.Activate();
            _equippedPassives.Add(passive);
        }
        
        // Apply stat modifiers (only for main class)
        if (applyStats)
        {
            foreach (var modifier in classInstance.ClassDefinition.StatModifiers)
            {
                _owner.AddModifier(modifier);
            }
        }
    }
    
    private void RemoveClassBenefits(ClassInstance classInstance)
    {
        // Remove abilities
        _equippedAbilities.RemoveAll(a => classInstance.Abilities.Contains(a));
        
        // Deactivate and remove passives
        foreach (var passive in classInstance.Passives)
        {
            passive.Deactivate();
            _equippedPassives.Remove(passive);
        }
        
        // Remove stat modifiers (regardless of class type, it's safe to attempt removing)
        foreach (var modifier in classInstance.ClassDefinition.StatModifiers)
        {
            _owner.RemoveModifier(modifier);
        }
    }
    
    public void Update(float deltaTime)
    {
        // Update cooldowns for all abilities
        foreach (var ability in _equippedAbilities)
        {
            ability.UpdateCooldown(deltaTime);
        }
        
        // Update all active passives
        foreach (var passive in _equippedPassives)
        {
            passive.Update(deltaTime);
        }
    }
    
    // Find an ability by ID across all equipped classes
    public Ability GetAbility(string abilityId)
    {
        return _equippedAbilities.FirstOrDefault(a => a.Definition.Id == abilityId);
    }
}

// Represents an instance of a class equipped by an entity
public class ClassInstance
{
    public ClassDefinition ClassDefinition { get; }
    public Entity Owner { get; }
    
    private readonly List<Ability> _abilities = new();
    private readonly List<Passive> _passives = new();
    
    public IReadOnlyList<Ability> Abilities => _abilities.AsReadOnly();
    public IReadOnlyList<Passive> Passives => _passives.AsReadOnly();
    
    public ClassInstance(ClassDefinition classDefinition, Entity owner)
    {
        ClassDefinition = classDefinition;
        Owner = owner;
        
        // Create ability instances
        foreach (var abilityDef in classDefinition.Abilities)
        {
            _abilities.Add(abilityDef.CreateInstance(owner));
        }
        
        // Create passive instances
        foreach (var passiveDef in classDefinition.Passives)
        {
            _passives.Add(passiveDef.CreateInstance(owner));
        }
    }
}
```

### 5. Integration with Entity System

The Entity class needs to be extended to support the class system.

```csharp
// Add to the Entity class from the Buff System
public class Entity
{
    // Existing code from the Buff System...
    
    private ClassManager _classManager;
    
    public ClassManager ClassManager => _classManager ??= new ClassManager(this);
    
    // Add to the OnTurnEnd method
    public virtual void OnTurnEnd()
    {
        // Existing buff code...
        
        // Check class abilities for turn-based effects if needed
        // This is optional and depends on your gameplay requirements
    }
    
    // Add a new Update method
    public virtual void Update(float deltaTime)
    {
        // Update class abilities and passives
        _classManager?.Update(deltaTime);
    }
}
```

## Example Implementation

### Example Class Definitions

```csharp
// Create a Warrior class
public static ClassDefinition CreateWarriorClass()
{
    var warrior = new ClassDefinition(
        "warrior",
        "Warrior",
        "A melee fighter skilled in combat and defense.",
        warriorIcon);
    
    // Add abilities
    warrior.AddAbility(new PowerStrikeAbility(
        "power_strike",
        "Power Strike",
        "A powerful melee attack that deals 50% extra damage.",
        powerStrikeIcon,
        3.0f)); // 3 second cooldown
    
    // Add passives
    warrior.AddPassive(new ToughnessPassive(
        "toughness",
        "Toughness",
        "Reduces incoming damage by 10%.",
        toughnessIcon));
    
    // Add stat modifiers
    warrior.AddStatModifier(new AttributeModifier(
        AttributeType.MaxHealth,
        20,
        ModifierType.Flat,
        warrior));
    warrior.AddStatModifier(new AttributeModifier(
        AttributeType.Attack,
        10,
        ModifierType.Flat,
        warrior));
    warrior.AddStatModifier(new AttributeModifier(
        AttributeType.Defense,
        15,
        ModifierType.Flat,
        warrior));
    
    return warrior;
}

// Create a Mage class
public static ClassDefinition CreateMageClass()
{
    var mage = new ClassDefinition(
        "mage",
        "Mage",
        "A spellcaster who controls the elements.",
        mageIcon);
    
    // Add abilities
    mage.AddAbility(new FireballAbility(
        "fireball",
        "Fireball",
        "Launch a ball of fire that damages enemies in an area.",
        fireballIcon,
        5.0f)); // 5 second cooldown
    
    // Add passives
    mage.AddPassive(new ManaRegenerationPassive(
        "mana_regen",
        "Mana Regeneration",
        "Regenerates mana over time.",
        manaRegenIcon));
    
    // Add stat modifiers
    mage.AddStatModifier(new AttributeModifier(
        AttributeType.MaxHealth,
        -10,
        ModifierType.Flat,
        mage));
    mage.AddStatModifier(new AttributeModifier(
        AttributeType.MagicPower,
        25,
        ModifierType.Flat,
        mage));
    
    return mage;
}
```

### Example Ability Implementation

```csharp
// Example of a concrete ability: Power Strike
public class PowerStrikeAbilityDefinition : AbilityDefinition
{
    public float DamageMultiplier { get; }
    
    public PowerStrikeAbilityDefinition(
        string id, string name, string description, Sprite icon, 
        float cooldown, float damageMultiplier = 1.5f)
        : base(id, name, description, icon, cooldown)
    {
        DamageMultiplier = damageMultiplier;
    }
    
    public override Ability CreateInstance(Entity owner)
    {
        return new PowerStrikeAbility(this, owner);
    }
}

public class PowerStrikeAbility : Ability
{
    private readonly PowerStrikeAbilityDefinition _definition;
    
    public PowerStrikeAbility(PowerStrikeAbilityDefinition definition, Entity owner)
        : base(definition, owner)
    {
        _definition = definition;
    }
    
    protected override void ExecuteAbility(params object[] args)
    {
        // Get target entity
        if (args.Length == 0 || !(args[0] is Entity target))
            return;
            
        // Create damage info with increased damage
        var damageInfo = new DamageInfo(
            Owner, 
            target, 
            0, // Base damage value will be set by calculator
            DamageType.Physical);
            
        // Add metadata to indicate this is a power strike
        damageInfo.Metadata["PowerStrike"] = true;
        damageInfo.Metadata["DamageMultiplier"] = _definition.DamageMultiplier;
        
        // Calculate and apply damage
        float damage = DamageCalculator.CalculateDamage(damageInfo);
        target.TakeDamage(damage);
        
        // Visual/sound effects would be triggered here
    }
}
```

### Example Passive Implementation

```csharp
// Example of a concrete passive: Toughness
public class ToughnessPassiveDefinition : PassiveDefinition
{
    public float DamageReductionPercent { get; }
    
    public ToughnessPassiveDefinition(
        string id, string name, string description, Sprite icon,
        float damageReductionPercent = 10f)
        : base(id, name, description, icon)
    {
        DamageReductionPercent = damageReductionPercent;
    }
    
    public override Passive CreateInstance(Entity owner)
    {
        return new ToughnessPassive(this, owner);
    }
}

public class ToughnessPassive : Passive, IDamageModifier
{
    private readonly ToughnessPassiveDefinition _definition;
    
    public int Priority => 100; // Higher priority to run after base calculations
    
    public ToughnessPassive(ToughnessPassiveDefinition definition, Entity owner)
        : base(definition, owner)
    {
        _definition = definition;
    }
    
    protected override void OnActivate()
    {
        // Register as a damage modifier
        DamageCalculator.RegisterModifier(this);
    }
    
    protected override void OnDeactivate()
    {
        // Unregister as a damage modifier
        DamageCalculator.UnregisterModifier(this);
    }
    
    public override void Update(float deltaTime)
    {
        // No per-frame updates needed for this passive
    }
    
    public void ModifyDamage(DamageInfo damageInfo)
    {
        // Only reduce damage when this entity is the target
        if (damageInfo.Target == Owner)
        {
            // Reduce incoming damage by the percentage
            damageInfo.Amount *= (1f - (_definition.DamageReductionPercent / 100f));
        }
    }
}
```

### Player Usage Example

```csharp
// Example of how a player would use the class system
public class Player : Entity
{
    public void EquipMainClass(ClassDefinition classDefinition)
    {
        ClassManager.EquipClass(classDefinition, ClassSlotType.Main);
    }
    
    public void EquipSubClass(ClassDefinition classDefinition)
    {
        ClassManager.EquipClass(classDefinition, ClassSlotType.Sub);
    }
    
    public void UseAbility(string abilityId, Entity target)
    {
        var ability = ClassManager.GetAbility(abilityId);
        if (ability != null && ability.CanUse())
        {
            ability.Use(target);
        }
    }
}
```

## UI Integration

A key part of the class system is the UI to display and interact with classes, abilities, and passives.

```csharp
// This would be implemented in your UI layer
public class ClassUI : MonoBehaviour
{
    [SerializeField] private GameObject _mainClassPanel;
    [SerializeField] private GameObject _subClassPanel;
    [SerializeField] private GameObject _abilitiesPanel;
    
    private Player _player;
    
    private void Start()
    {
        _player = FindObjectOfType<Player>();
        
        // Subscribe to class change events
        _player.ClassManager.OnClassChanged += OnPlayerClassChanged;
        
        // Initial update of UI
        UpdateUI();
    }
    
    private void OnPlayerClassChanged(ClassSlotType slotType, ClassInstance oldClass, ClassInstance newClass)
    {
        // Update UI when classes change
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Update main class display
        if (_player.ClassManager.MainClass != null)
        {
            // Display main class info
        }
        
        // Update sub class display
        if (_player.ClassManager.SubClass != null)
        {
            // Display sub class info
        }
        
        // Update abilities display
        // Clear previous abilities
        // For each ability in _player.ClassManager.EquippedAbilities, create UI element
    }
    
    // Handler for when player wants to change class
    public void OpenClassSelectionUI()
    {
        // Open UI for class selection
    }
    
    // Handler for when player selects a class in the UI
    public void OnClassSelected(ClassDefinition selectedClass, ClassSlotType slot)
    {
        if (slot == ClassSlotType.Main)
        {
            _player.EquipMainClass(selectedClass);
        }
        else
        {
            _player.EquipSubClass(selectedClass);
        }
    }
    
    // Handler for when player clicks an ability button
    public void OnAbilityButtonPressed(string abilityId)
    {
        // Enter ability targeting mode or use immediately if no target needed
    }
}
```

## SOLID Principles Application

### Single Responsibility Principle
- Each class has a clear, singular purpose:
  - `ClassDefinition` defines a class's characteristics
  - `AbilityDefinition` and `Ability` handle active skill functionality
  - `PassiveDefinition` and `Passive` handle passive skill functionality
  - `ClassManager` manages equipped classes and their benefits

### Open/Closed Principle
- The system is designed for extension without modification:
  - New classes can be added without changing existing code
  - New abilities can be created by extending `AbilityDefinition`
  - New passives can be created by extending `PassiveDefinition`

### Liskov Substitution Principle
- Derived classes can replace base classes without affecting functionality:
  - All ability implementations derive from `Ability` base class
  - All passive implementations derive from `Passive` base class
  - Interface implementations like `IDamageModifier` ensure consistent behavior

### Interface Segregation Principle
- Interfaces are specific rather than general:
  - `IDamageModifier` only requires methods relevant to damage modification
  - Ability and passive interfaces are separate and focused on their specific needs

### Dependency Inversion Principle
- High-level modules don't depend on low-level modules:
  - The class system uses interfaces and abstractions for communication
  - `ClassManager` operates on abstractions like `Ability` and `Passive`
  - Systems interact through well-defined interfaces

## Implementation Plan

1. **Phase 1: Core Definition Systems**
   - Implement `ClassDefinition` class
   - Implement `AbilityDefinition` and `PassiveDefinition` base classes
   - Create class registry system for easy access to class definitions

2. **Phase 2: Runtime Instance Classes**
   - Implement `ClassInstance` class
   - Implement `Ability` and `Passive` base classes
   - Create example implementations of concrete abilities and passives

3. **Phase 3: Class Management System**
   - Implement `ClassManager` for handling equipped classes
   - Integrate with existing `Entity` system
   - Handle the main/sub class distinction and differential benefits

4. **Phase 4: Integration with Existing Systems**
   - Connect with `DamageCalculator` for ability damage calculation
   - Implement attribute modifier support for class stat bonuses
   - Handle persistence and serialization of equipped classes

5. **Phase 5: UI and Player Experience**
   - Create UI for displaying classes, abilities, and passives
   - Implement class selection/switching interface
   - Add visual feedback for ability usage and cooldowns

## Conclusion

This class system design provides a flexible and extensible framework for implementing classes in the RPG Minesweeper game. By following SOLID principles, the system can easily accommodate new classes, abilities, and passives as the game evolves.

The hybrid class approach allows players to customize their playstyle by combining a main class (providing abilities, passives, and stats) with a sub-class (providing additional abilities and passives). This creates depth in gameplay and encourages experimentation with different class combinations.

Integration with the existing buff and damage calculation systems ensures that class abilities and passives work seamlessly with other game mechanics, providing a cohesive player experience.

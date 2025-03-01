# Monster System Refactoring Plan (SOLID Version)

## Evaluation of Approaches

### Approach 1: Entity-Attribute System Integration
Using the Entity system as a component manager for attributes provides a strong foundation with built-in event handling, attribute management, and modifier support. This offers good separation between data (MonsterMineData), state/behavior (MonsterMine), and attribute management (Entity).

### Approach 2: DamageInfo with Centralized Calculation
Creating a `DamageInfo` class that acts as a data transfer object between entities and a central calculation service.

#### Pros and Cons
Pros and cons remain as previously stated. The main benefit is the complete separation of damage calculation from entity state.

## SOLID Principles Implementation

The revised implementation plan will focus on adhering to SOLID principles:

1. **Single Responsibility Principle (SRP)**: Each class should have one reason to change
2. **Open/Closed Principle (OCP)**: Software entities should be open for extension but closed for modification
3. **Liskov Substitution Principle (LSP)**: Subtypes must be substitutable for their base types
4. **Interface Segregation Principle (ISP)**: Clients shouldn't depend on interfaces they don't use
5. **Dependency Inversion Principle (DIP)**: Depend on abstractions, not concretions

## Implementation Plan

### 1. Core Interfaces

```csharp
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
```

### 2. Attribute Type Registry

```csharp
/// <summary>
/// Central registry for all attribute types
/// </summary>
public static class AttributeTypes
{
    // Core attributes
    public static readonly AttributeType MAX_HP = new AttributeType("MaxHP");
    public static readonly AttributeType CURRENT_HP = new AttributeType("CurrentHP");
    
    // Damage attributes
    public static readonly AttributeType BASE_DAMAGE = new AttributeType("BaseDamage");
    public static readonly AttributeType DAMAGE_PER_HIT = new AttributeType("DamagePerHit");
    
    // Special attributes
    public static readonly AttributeType ENRAGE_MULTIPLIER = new AttributeType("EnrageMultiplier");
    
    // Resistance attributes
    public static readonly AttributeType PHYSICAL_RESISTANCE = new AttributeType("PhysicalResistance");
    public static readonly AttributeType MAGICAL_RESISTANCE = new AttributeType("MagicalResistance");
    public static readonly AttributeType FIRE_RESISTANCE = new AttributeType("FireResistance");
    public static readonly AttributeType ICE_RESISTANCE = new AttributeType("IceResistance");
    
    // Collection of all resistance types
    public static readonly Dictionary<DamageType, AttributeType> ResistanceTypes = 
        new Dictionary<DamageType, AttributeType>
        {
            { DamageType.Physical, PHYSICAL_RESISTANCE },
            { DamageType.Magical, MAGICAL_RESISTANCE },
            { DamageType.Fire, FIRE_RESISTANCE },
            { DamageType.Ice, ICE_RESISTANCE }
        };
}
```

### 3. DamageInfo Class

```csharp
/// <summary>
/// Contains all information needed for damage calculation
/// </summary>
public class DamageInfo
{
    // Core properties
    public IEntity Source { get; set; }          // Who's dealing the damage
    public IEntity Target { get; set; }          // Who's receiving the damage
    public DamageType Type { get; set; }        // Physical, magical, etc.
    public float BaseDamage { get; set; }       // Starting damage value
    
    // Calculation modifiers
    public float DamageMultiplier { get; set; } = 1.0f;
    public float DamageAddition { get; set; } = 0.0f;
    public float ResistanceMultiplier { get; set; } = 1.0f;
    
    // Special state flags
    public bool IsCritical { get; set; } = false;
    public bool IsEnraged { get; set; } = false;
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    
    // Results (filled after calculation)
    public float FinalDamage { get; set; }
    public bool WasEvaded { get; set; } = false;
    public bool TargetDefeated { get; set; } = false;
    
    // Factory method pattern for creating specific damage info types
    public static class Factory
    {
        public static DamageInfo CreateMonsterDamage(MonsterEntity monster, IEntity player)
        {
            var info = new DamageInfo
            {
                Source = monster,
                Target = player,
                Type = DamageType.Physical,
                BaseDamage = monster.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0,
                IsEnraged = monster.IsEnraged()
            };
            
            return info;
        }
        
        public static DamageInfo CreatePlayerDamage(IEntity player, MonsterEntity monster, float damageAmount)
        {
            var info = new DamageInfo
            {
                Source = player,
                Target = monster,
                Type = DamageType.Physical,
                BaseDamage = damageAmount
            };
            
            return info;
        }
    }
}

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

### 4. Damage Calculation Pipeline

```csharp
/// <summary>
/// Pipeline step for damage calculation
/// </summary>
public interface IDamageCalculationStep
{
    DamageInfo Process(DamageInfo damageInfo);
}

/// <summary>
/// Special state handler for enrage, critical hits, etc.
/// </summary>
public class SpecialStateProcessor : IDamageCalculationStep
{
    public DamageInfo Process(DamageInfo info)
    {
        // Handle enrage state for monsters
        if (info.Source is MonsterEntity monster && info.IsEnraged)
        {
            float enrageMultiplier = monster.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.CurrentValue ?? 1.0f;
            info.DamageMultiplier *= enrageMultiplier;
        }
        
        // Handle critical hits
        if (info.IsCritical)
        {
            info.DamageMultiplier *= 1.5f; // Default critical multiplier
        }
        
        return info;
    }
}

/// <summary>
/// Base damage processor - calculates raw damage
/// </summary>
public class BaseDamageProcessor : IDamageCalculationStep
{
    public DamageInfo Process(DamageInfo info)
    {
        // Calculate base damage with multipliers
        float rawDamage = (info.BaseDamage + info.DamageAddition) * info.DamageMultiplier;
        
        // Store intermediate result in metadata for debugging
        info.Metadata["RawDamage"] = rawDamage;
        
        return info;
    }
}

/// <summary>
/// Resistance processor - applies damage reduction from resistances
/// </summary>
public class ResistanceProcessor : IDamageCalculationStep
{
    public DamageInfo Process(DamageInfo info)
    {
        if (info.Type == DamageType.True)
        {
            // True damage ignores resistances
            info.FinalDamage = (float)info.Metadata["RawDamage"];
            return info;
        }
            
        float resistanceValue = 0;
        float rawDamage = (float)info.Metadata["RawDamage"];
        
        // Get the appropriate resistance attribute based on damage type
        if (info.Target != null && AttributeTypes.ResistanceTypes.TryGetValue(info.Type, out var resistanceType))
        {
            resistanceValue = info.Target.GetAttribute(resistanceType)?.CurrentValue ?? 0;
        }
        
        // Apply resistance formula: 
        // Each point of resistance reduces damage by 0.5% (can be adjusted)
        float resistanceMultiplier = 1f - (resistanceValue * 0.005f * info.ResistanceMultiplier);
        resistanceMultiplier = Mathf.Clamp(resistanceMultiplier, 0.1f, 2.0f); // Resistance can reduce damage by at most 90%
        
        // Calculate final damage
        float finalDamage = rawDamage * resistanceMultiplier;
        
        // For monsters, we want integers only
        if (info.Target is MonsterEntity)
        {
            finalDamage = Mathf.RoundToInt(finalDamage);
        }
        
        info.FinalDamage = Mathf.Max(0, finalDamage);
        return info;
    }
}
```

### 5. Composite Damage Calculator

```csharp
/// <summary>
/// Main damage calculator that processes a damage info through a pipeline
/// </summary>
public class DamageCalculator : IDamageCalculator
{
    private readonly List<IDamageCalculationStep> _calculationSteps = new List<IDamageCalculationStep>();
    
    // Event for monitoring damage calculations
    public event Action<DamageInfo> OnDamageCalculated;
    
    public DamageCalculator()
    {
        // Add default calculation steps
        _calculationSteps.Add(new SpecialStateProcessor());
        _calculationSteps.Add(new BaseDamageProcessor());
        _calculationSteps.Add(new ResistanceProcessor());
    }
    
    /// <summary>
    /// Add a custom calculation step to the pipeline
    /// </summary>
    public void AddCalculationStep(IDamageCalculationStep step)
    {
        _calculationSteps.Add(step);
    }
    
    /// <summary>
    /// Calculate the final damage for a given DamageInfo
    /// </summary>
    public DamageInfo Calculate(DamageInfo info)
    {
        // Process through all calculation steps
        foreach (var step in _calculationSteps)
        {
            info = step.Process(info);
        }
        
        // Notify listeners
        OnDamageCalculated?.Invoke(info);
        
        return info;
    }
}
```

### 6. Damage Applier

```csharp
/// <summary>
/// Applies damage to entities
/// </summary>
public class DamageApplier : IDamageApplier
{
    private readonly IDamageCalculator _calculator;
    
    public DamageApplier(IDamageCalculator calculator)
    {
        _calculator = calculator;
    }
    
    /// <summary>
    /// Apply damage to the target entity
    /// </summary>
    public void ApplyDamage(DamageInfo info)
    {
        if (info.Target == null) return;
        
        // Calculate final damage if not already calculated
        if (info.FinalDamage <= 0)
        {
            info = _calculator.Calculate(info);
        }
        
        // Apply damage to HP
        float currentHp = 0;
        Attribute hpAttribute = null;
        
        // Different handling for different entity types
        if (info.Target is MonsterEntity)
        {
            hpAttribute = info.Target.GetAttribute(AttributeTypes.CURRENT_HP);
        }
        else
        {
            // Assuming player uses standard AttributeType.CurrentHealth
            hpAttribute = info.Target.GetAttribute(AttributeType.CurrentHealth);
        }
        
        if (hpAttribute != null)
        {
            currentHp = hpAttribute.CurrentValue;
            float newHp = currentHp - info.FinalDamage;
            hpAttribute.SetBaseValue(newHp);
            
            // Check if target was defeated
            info.TargetDefeated = newHp <= 0;
        }
    }
}
```

### 7. Attribute Initializer Factory

```csharp
/// <summary>
/// Factory for creating attribute initializers
/// </summary>
public static class AttributeInitializerFactory
{
    /// <summary>
    /// Create an initializer for a specific monster type
    /// </summary>
    public static IAttributeInitializer CreateMonsterInitializer(MonsterType monsterType, MonsterMineData data)
    {
        switch (monsterType)
        {
            case MonsterType.Slime:
                return new SlimeAttributeInitializer(data);
            case MonsterType.Spider:
                return new SpiderAttributeInitializer(data);
            case MonsterType.Dragon:
                return new DragonAttributeInitializer(data);
            default:
                return new DefaultMonsterAttributeInitializer(data);
        }
    }
    
    /// <summary>
    /// Create an initializer for player attributes
    /// </summary>
    public static IAttributeInitializer CreatePlayerInitializer()
    {
        return new PlayerAttributeInitializer();
    }
}

/// <summary>
/// Base class for monster attribute initializers
/// </summary>
public abstract class BaseMonsterAttributeInitializer : IAttributeInitializer
{
    protected readonly MonsterMineData _data;
    
    protected BaseMonsterAttributeInitializer(MonsterMineData data)
    {
        _data = data;
    }
    
    public virtual void InitializeAttributes(Entity entity)
    {
        // Set up core attributes
        entity.AddAttribute(AttributeTypes.MAX_HP, _data.MaxHp);
        entity.AddAttribute(AttributeTypes.CURRENT_HP, _data.MaxHp);
        entity.AddAttribute(AttributeTypes.BASE_DAMAGE, _data.BaseDamage);
        entity.AddAttribute(AttributeTypes.DAMAGE_PER_HIT, _data.DamagePerHit);
        entity.AddAttribute(AttributeTypes.ENRAGE_MULTIPLIER, _data.EnrageDamageMultiplier);
        
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
    public DefaultMonsterAttributeInitializer(MonsterMineData data) : base(data) {}
}

/// <summary>
/// Slime-specific attribute initializer
/// </summary>
public class SlimeAttributeInitializer : BaseMonsterAttributeInitializer
{
    public SlimeAttributeInitializer(MonsterMineData data) : base(data) {}
    
    public override void InitializeAttributes(Entity entity)
    {
        base.InitializeAttributes(entity);
        
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
    public SpiderAttributeInitializer(MonsterMineData data) : base(data) {}
    
    public override void InitializeAttributes(Entity entity)
    {
        base.InitializeAttributes(entity);
        
        // Spiders have higher evasion
        entity.AddAttribute(new AttributeType("Evasion"), 15);
    }
}

/// <summary>
/// Dragon-specific attribute initializer
/// </summary>
public class DragonAttributeInitializer : BaseMonsterAttributeInitializer
{
    public DragonAttributeInitializer(MonsterMineData data) : base(data) {}
    
    public override void InitializeAttributes(Entity entity)
    {
        base.InitializeAttributes(entity);
        
        // Dragons are resistant to all elemental damage
        entity.SetAttribute(AttributeTypes.FIRE_RESISTANCE, 50);
        entity.SetAttribute(AttributeTypes.ICE_RESISTANCE, 25);
        
        // Add unique attribute for dragons
        entity.AddAttribute(new AttributeType("BreathDamage"), 100);
    }
}
```

### 8. Service Locator for Damage System

```csharp
/// <summary>
/// Service locator for damage related systems
/// </summary>
public static class DamageSystem
{
    private static IDamageCalculator _calculator;
    private static IDamageApplier _applier;
    
    static DamageSystem()
    {
        // Default initialization
        _calculator = new DamageCalculator();
        _applier = new DamageApplier(_calculator);
    }
    
    /// <summary>
    /// Set a custom damage calculator
    /// </summary>
    public static void SetCalculator(IDamageCalculator calculator)
    {
        _calculator = calculator;
    }
    
    /// <summary>
    /// Set a custom damage applier
    /// </summary>
    public static void SetApplier(IDamageApplier applier)
    {
        _applier = applier;
    }
    
    /// <summary>
    /// Calculate damage without applying it
    /// </summary>
    public static DamageInfo CalculateDamage(DamageInfo info)
    {
        return _calculator.Calculate(info);
    }
    
    /// <summary>
    /// Apply pre-calculated damage to a target
    /// </summary>
    public static void ApplyDamage(DamageInfo info)
    {
        _applier.ApplyDamage(info);
    }
    
    /// <summary>
    /// Calculate and apply damage in one step
    /// </summary>
    public static void CalculateAndApplyDamage(DamageInfo info)
    {
        info = _calculator.Calculate(info);
        _applier.ApplyDamage(info);
    }
}
```

### 9. Revised MonsterEntity

```csharp
/// <summary>
/// Entity representing a monster with attributes
/// </summary>
public class MonsterEntity : Entity, IDamageSource, IDamageTarget
{
    private MonsterMineData _mineData;
    private bool _isEnraged = false;
    
    /// <summary>
    /// Creates a new monster entity from mine data
    /// </summary>
    public MonsterEntity(MonsterMineData data, string name = null) 
        : base(name ?? $"Monster_{data.MonsterType}")
    {
        _mineData = data;
        
        // Use factory to create appropriate initializer
        var initializer = AttributeInitializerFactory.CreateMonsterInitializer(data.MonsterType, data);
        initializer.InitializeAttributes(this);
    }
    
    /// <summary>
    /// Get the HP percentage of this monster
    /// </summary>
    public float GetHpPercentage()
    {
        var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
        var maxHp = GetAttribute(AttributeTypes.MAX_HP);
        
        if (currentHp == null || maxHp == null || maxHp.CurrentValue == 0)
            return 0;
            
        return currentHp.CurrentValue / maxHp.CurrentValue;
    }
    
    /// <summary>
    /// Check if the monster should be enraged and update state
    /// </summary>
    public bool UpdateEnrageState()
    {
        if (!_mineData.HasEnrageState)
            return false;
            
        bool shouldBeEnraged = GetHpPercentage() <= 0.3f;
        
        if (shouldBeEnraged && !_isEnraged)
        {
            _isEnraged = true;
            return true; // Indicates state changed
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if monster is currently enraged
    /// </summary>
    public bool IsEnraged()
    {
        return _isEnraged;
    }
    
    /// <summary>
    /// Check if the monster is defeated
    /// </summary>
    public bool IsDefeated()
    {
        var currentHp = GetAttribute(AttributeTypes.CURRENT_HP);
        return currentHp == null || currentHp.CurrentValue <= 0;
    }
    
    /// <summary>
    /// Create damage info targeting an entity
    /// </summary>
    public DamageInfo CreateDamageInfo(IEntity target)
    {
        return DamageInfo.Factory.CreateMonsterDamage(this, target);
    }
    
    /// <summary>
    /// Deal damage to a target using the damage system
    /// </summary>
    public void DealDamageTo(IEntity target)
    {
        var damageInfo = CreateDamageInfo(target);
        DamageSystem.CalculateAndApplyDamage(damageInfo);
    }
    
    /// <summary>
    /// Receive damage from a damage info
    /// </summary>
    public void ReceiveDamage(DamageInfo damageInfo)
    {
        // Damage info is already targeted at this entity
        DamageSystem.ApplyDamage(damageInfo);
        
        // Update enrage state
        UpdateEnrageState();
    }
}
```

### 10. Revised MonsterMine

```csharp
/// <summary>
/// Mine implementation for monsters
/// </summary>
public class MonsterMine : IDamagingMine
{
    #region Private Fields
    private readonly MonsterMineData m_Data;
    private readonly Vector2Int m_Position;
    private readonly List<Vector2Int> m_AffectedPositions;
    private readonly List<IPersistentEffect> m_ActivePersistentEffects = new();
    private float m_ElapsedTime;
    private bool m_IsEnraged;
    private bool m_IsDefeated;
    private bool m_IsCollectable = false;
    private GameObject m_GameObject;
    
    // Entity for attribute and damage management
    private MonsterEntity m_Entity;
    #endregion

    #region Public Properties
    public MineType Type => m_Data.Type;
    public bool CanDisguise => true;
    
    // HP management through entity
    public int CurrentHp 
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue ?? 0);
        set 
        {
            float oldHp = m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.CurrentValue ?? 0;
            m_Entity.GetAttribute(AttributeTypes.CURRENT_HP)?.SetBaseValue(value);
            
            OnHpChanged?.Invoke(m_Position, HpPercentage);
            
            // Check if monster was just defeated
            if (CurrentHp <= 0 && !m_IsDefeated)
            {
                m_IsDefeated = true;
                OnDefeated?.Invoke(m_Position);
            }
        }
    }
    
    public float HpPercentage => m_Entity.GetHpPercentage();
    public bool IsEnraged => m_IsEnraged;
    public bool IsDefeated => m_IsDefeated;
    public bool IsCollectable => m_IsCollectable;
    public MonsterType MonsterType => m_Data.MonsterType;
    
    // Properties still exposed for compatibility
    public int MaxHp
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.MAX_HP)?.CurrentValue ?? 0);
        set
        {
            m_Entity.GetAttribute(AttributeTypes.MAX_HP)?.SetBaseValue(value);
            OnHpChanged?.Invoke(m_Position, HpPercentage);
        }
    }
    
    public int BaseDamage
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.CurrentValue ?? 0);
        set => m_Entity.GetAttribute(AttributeTypes.BASE_DAMAGE)?.SetBaseValue(value);
    }
    
    public int DamagePerHit
    {
        get => Mathf.RoundToInt(m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.CurrentValue ?? 0);
        set => m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.SetBaseValue(value);
    }
    
    public float EnrageDamageMultiplier
    {
        get => m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.CurrentValue ?? 1f;
        set => m_Entity.GetAttribute(AttributeTypes.ENRAGE_MULTIPLIER)?.SetBaseValue(Mathf.Clamp(value, 1f, 3f));
    }
    
    public bool HasEnrageState => m_Data.HasEnrageState;

    // Accessor methods for external systems
    public MineData GetMineData() => m_Data;
    public MonsterEntity GetEntity() => m_Entity;
    #endregion

    #region Events
    public System.Action<Vector2Int> OnEnraged;
    public System.Action<Vector2Int, float> OnHpChanged;
    public System.Action<Vector2Int> OnDefeated;
    #endregion

    #region Constructor
    public MonsterMine(MonsterMineData _data, Vector2Int _position)
    {
        m_Data = _data;
        m_Position = _position;
        
        // Create the entity
        m_Entity = new MonsterEntity(_data);
        
        // Subscribe to entity events
        m_Entity.OnAttributeValueChanged += HandleAttributeValueChanged;
        
        // Initialize other fields
        m_IsDefeated = false;
        m_AffectedPositions = GridShapeHelper.GetAffectedPositions(m_Position, m_Data.Shape, m_Data.Radius);
        InitializePersistentEffects();
    }
    
    private void HandleAttributeValueChanged(Entity entity, Attribute attribute, float oldValue, float newValue)
    {
        // Handle HP changes
        if (attribute.Type == AttributeTypes.CURRENT_HP)
        {
            OnHpChanged?.Invoke(m_Position, HpPercentage);
            
            // Check enrage state
            if (m_Data.HasEnrageState && m_Entity.UpdateEnrageState() && !m_IsEnraged)
            {
                m_IsEnraged = true;
                OnEnraged?.Invoke(m_Position);
            }
            
            // Check defeated state
            if (newValue <= 0 && !m_IsDefeated)
            {
                m_IsDefeated = true;
                OnDefeated?.Invoke(m_Position);
            }
        }
    }
    #endregion

    #region IDamagingMine Implementation
    public int CalculateDamage()
    {
        // Get player entity
        var playerEntity = GameObject.FindFirstObjectByType<PlayerComponent>()?.GetEntity();
        if (playerEntity == null) return 0;
        
        // Create damage info
        var damageInfo = m_Entity.CreateDamageInfo(playerEntity);
        
        // Calculate damage through the damage system
        damageInfo = DamageSystem.CalculateDamage(damageInfo);
        
        // Return calculated damage
        return Mathf.RoundToInt(damageInfo.FinalDamage);
    }
    #endregion

    #region IMine Implementation
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
        
        // Get player entity
        var playerEntity = _player.GetEntity();
        if (playerEntity == null) return;
        
        // Deal damage to player
        m_Entity.DealDamageTo(playerEntity);
        
        // Create damage info for player -> monster
        var damageInfo = DamageInfo.Factory.CreatePlayerDamage(
            playerEntity, 
            m_Entity, 
            m_Entity.GetAttribute(AttributeTypes.DAMAGE_PER_HIT)?.CurrentValue ?? 0
        );
        
        // Apply damage to monster
        m_Entity.ReceiveDamage(damageInfo);
        
        // Apply effects if monster is still alive
        if (!m_IsDefeated)
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
    
    // Other IMine methods remain largely unchanged
    #endregion
}
```

## SOLID Principles Applied

### 1. Single Responsibility Principle (SRP)
- **DamageCalculator**: Only handles damage calculation
- **DamageApplier**: Only handles applying damage
- **AttributeInitializer**: Only handles attribute initialization
- **Calculation Steps**: Each step handles one aspect of damage calculation

### 2. Open/Closed Principle (OCP)
- **AttributeInitializerFactory**: Allows adding new monster types without modifying existing code
- **Damage Pipeline**: New calculation steps can be added without changing the calculator
- **DamageSystem**: Services can be replaced without changing client code

### 3. Liskov Substitution Principle (LSP)
- **IEntity**: All entity implementations are substitutable
- **IDamageCalculationStep**: All steps correctly implement the processing contract

### 4. Interface Segregation Principle (ISP)
- **IDamageSource/IDamageTarget**: Separate interfaces for damage dealing vs receiving
- **IAttributeInitializer**: Focused on just attribute initialization
- **IDamageCalculator/IDamageApplier**: Separate interfaces for calculation vs application

### 5. Dependency Inversion Principle (DIP)
- All components depend on abstractions (interfaces) rather than concrete implementations
- Factories create concrete implementations
- Service locator pattern allows swapping services

## Benefits of This Approach

1. **Extensibility**
   - Easy to add new monster types with specialized attributes
   - Damage calculation pipeline can be extended with new steps
   - Additional damage types can be added without modifying existing code

2. **Testability**
   - Each component can be tested in isolation
   - Mock implementations can be created for interfaces
   - Calculation pipeline can be tested step by step

3. **Maintainability**
   - Clear separation of concerns
   - Well-defined interfaces
   - Isolated changes don't ripple through the codebase

4. **Flexibility**
   - Services can be replaced at runtime
   - New monster behaviors can be added by creating new initializers
   - Damage calculation can be customized for different game modes

## Conclusion

This SOLID-compliant implementation provides a robust foundation for your combat system that is highly extensible and maintainable. The use of the factory pattern for attribute initialization is particularly valuable for adding new monster types without modifying existing code.

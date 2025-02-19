# Effect Implementation Process in RPG Minesweeper

This document outlines the step-by-step process of implementing a new effect in the RPG Minesweeper system, using the `FreezeEffect` as a reference implementation.

## 1. Core Components Overview

The effect system consists of several key components that work together:

- `BaseEffect`: Abstract base class providing core effect functionality
- `EffectData`: ScriptableObject-based configuration
- `State`: State management for effect behaviors
- Integration with the grid and mine systems
- This game is a *turn-based* game!!!
- Should implement both `IPersistentEffect` and `ITriggerableEffect` interfaces
- For `Triggerable` effect, it's now understood as the effect that triggered when a cell is removed
- For `Persistent` effect, it's now understood as the effect that is applied every turn.
- The concrete effect will be auto assigned in the inspector, so there is no need to use `m_CurrentMode` to switch between `IPersistentEffect` and `ITriggerableEffect`

## 2. Implementation Steps

### 2.1. Create the Effect Class

1. Create a new class inheriting from `BaseEffect`:
```csharp
public class NewEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
{
    // Implement required interfaces based on effect behavior
}
```

2. Implement core properties:
```csharp
public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
public bool IsActive => m_IsActive;
public string Name => "NewEffectName";
```

3. Define effect parameters:
```csharp
private readonly int m_Duration;
private readonly int m_Radius;
private readonly GridShape m_Shape;
private bool m_IsActive;
```

### 2.2. Create the Effect Data Class

1. Create a ScriptableObject for effect configuration:
```csharp
[CreateAssetMenu(fileName = "NewEffectData", menuName = "RPGMinesweeper/Effects/NewEffect")]
public class NewEffectData : EffectData
{
    public override IEffect CreateEffect()
    {
        var effect = new NewEffect(Duration, Radius, Shape);
        return effect;
    }
}
```

### 2.3. Implement State Management (if needed)

1. Create a state class inheriting from appropriate base state:
```csharp
public class NewState : BaseTurnState
{
    public NewState(int turns, int radius, Vector2Int sourcePosition, GridShape shape) 
        : base("StateName", turns, StateTarget.Cell, sourcePosition)
    {
        // Initialize state parameters
    }
}
```

2. Implement state lifecycle methods:
```csharp
public override void Enter(GameObject target)
public override void Exit(GameObject target)
public override void OnTurnEnd()
```

### 2.4. Effect Implementation Details

1. Implement effect application methods:
```csharp
protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
{
    m_IsActive = true;
    EnsureStateManager();
    ApplyEffectState(target, sourcePosition);
}

protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
{
    // Similar to persistent but with triggerable logic
}
```

2. Implement state management:
```csharp
private void ApplyEffectState(GameObject target, Vector2Int sourcePosition)
{
    var state = new NewState(m_Duration, m_Radius, sourcePosition, m_Shape);
    m_StateManager.AddState(state);
}
```

### 2.5. Integration with Grid System

1. Register affected cells:
```csharp
protected void RegisterEffectInArea(Vector2Int sourcePosition, float radius, GridShape shape)
{
    // Use GridShapeHelper to get affected positions
    // Register effect with MineValueModifier
}
```

2. Handle cleanup:
```csharp
protected void UnregisterEffectFromArea()
{
    // Unregister effect from affected cells
    // Clear affected cells collection
}
```

## 3. System Integration Points

### 3.1. Effect Registration

- Effects are typically created through their `EffectData` ScriptableObjects
- Registration happens through the state system or direct application

### 3.2. State Management

- States are managed by the `StateManager`
- States can affect:
  - Cell appearance/behavior
  - Mine behavior
  - Grid calculations

### 3.3. Grid Integration

- Effects can modify:
  - Cell values
  - Cell appearance
  - Mine behavior
  - Grid calculations

## 4. Example: FreezeEffect Implementation

The FreezeEffect serves as a reference implementation:

1. Effect behavior:
   - Freezes cells in an area
   - Supports both persistent and triggerable modes
   - Uses turn-based duration

2. Key components:
   - `FreezeEffect.cs`: Main effect implementation
   - `FreezeEffectData.cs`: ScriptableObject configuration
   - `FrozenState.cs`: State management

3. Integration points:
   - Registers with `StateManager`
   - Modifies cell behavior through `CellView`
   - Affects grid calculations through state system

## 5. Best Practices

1. Effect Implementation:
   - Keep effects modular and focused
   - Implement appropriate interfaces
   - Use state system for complex behaviors

2. State Management:
   - Clear Enter/Exit behavior
   - Proper cleanup in Exit
   - Handle turn-based updates

3. Grid Integration:
   - Register/unregister effects properly
   - Update cell values when needed
   - Handle visual updates

4. Error Handling:
   - Validate parameters
   - Handle missing dependencies
   - Provide debug logging

## 6. Testing Considerations

1. Test effect creation and configuration
2. Verify state management
3. Check grid integration
4. Validate cleanup and resource management
5. Test interaction with other effects

## 7. Common Pitfalls

1. Forgetting to unregister effects
2. Not handling state cleanup
3. Missing dependency validation
4. Improper turn management
5. Incomplete interface implementation

This implementation process provides a structured approach to adding new effects to the RPG Minesweeper system while maintaining consistency and reliability.

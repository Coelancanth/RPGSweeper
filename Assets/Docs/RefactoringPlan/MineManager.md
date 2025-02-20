# MineManager Refactoring Plan - Updated

## Current State Analysis

### Already Implemented Components
1. `IMineFactory` and `MineFactory` - Factory pattern for mine creation
2. `IMineSpawner` and `MineSpawner` - Spawning strategy implementation
3. `IMineSpawnStrategy` and concrete strategies - Strategy pattern for spawn positions
4. `IMineCopyStrategy` and concrete strategies - Strategy pattern for mine copying
5. Basic interfaces like `IMine`, `IDamagingMine`

### Current Issues
1. `MineManager` still handles too many responsibilities
2. Visual updates are mixed with business logic
3. Event handling is tightly coupled
4. Configuration management needs improvement

## Revised Refactoring Plan

### 1. Extract Visual Management
```csharp
public interface IMineVisualManager
{
    void UpdateCellView(Vector2Int position, MineData mineData, IMine mine);
    void ShowMineSprite(Vector2Int position, Sprite sprite, IMine mine, MineData mineData);
    void HandleMineRemoval(Vector2Int position);
}
```

### 2. Extract Event Management
```csharp
public interface IMineEventHandler
{
    void SubscribeToEvents();
    void UnsubscribeFromEvents();
    void HandleCellRevealed(Vector2Int position);
    void HandleMineRemoval(Vector2Int position);
    void HandleMineAdd(Vector2Int position, MineType type, MonsterType? monsterType);
}
```

### 3. Improve Configuration Management
```csharp
public class MineConfiguration
{
    public IReadOnlyList<MineTypeSpawnData> SpawnData { get; }
    public IReadOnlyDictionary<MineType, MineData> MineDataMap { get; }
}

public interface IMineConfigurationProvider
{
    MineConfiguration GetConfiguration();
    void ValidateConfiguration(int gridWidth, int gridHeight);
}
```

### 4. Implementation Steps

1. **Phase 1: Visual Management**
   - Create `MineVisualManager` class
   - Move all UI-related code from `MineManager`
   - Update `CellView` interactions

2. **Phase 2: Event Management**
   - Create `MineEventHandler` class
   - Move event subscription and handling logic
   - Implement event coordination

3. **Phase 3: Configuration Management**
   - Create `MineConfigurationProvider`
   - Move configuration validation logic
   - Implement configuration caching

4. **Phase 4: MineManager Cleanup**
   - Remove duplicated code
   - Implement proper dependency injection
   - Update Unity inspector fields
   - Add XML documentation

### 5. Testing Strategy

1. **Unit Tests**
   - Visual manager tests
   - Event handler tests
   - Configuration validation tests

2. **Integration Tests**
   - Event flow tests
   - Visual update tests
   - Full game loop tests

### 6. Migration Steps

1. Create new components alongside existing code
2. Move functionality one piece at a time:
   - First: Visual management
   - Second: Event handling
   - Third: Configuration
3. Update existing references
4. Remove old implementations
5. Add tests for new components

### 7. Code Organization

```
Assets/
  Scripts/
    Core/
      Mines/
        Interfaces/
          IMineVisualManager.cs
          IMineEventHandler.cs
          IMineConfigurationProvider.cs
        Implementation/
          MineVisualManager.cs
          MineEventHandler.cs
          MineConfigurationProvider.cs
    Managers/
      MineManager.cs (simplified version)
```

### Expected Benefits

1. **Better Separation of Concerns**
   - Visual logic isolated
   - Event handling decoupled
   - Configuration management centralized

2. **Improved Testability**
   - Each component testable in isolation
   - Mocking interfaces possible
   - Better test coverage

3. **Enhanced Maintainability**
   - Clear component boundaries
   - Reduced class sizes
   - Better documentation

4. **Easier Extensions**
   - New mine types
   - New visual effects
   - New event types

## Next Steps

1. Create `IMineVisualManager` and implementation
2. Move visual logic from `MineManager`
3. Create `IMineEventHandler` and implementation
4. Update `MineManager` to use new components

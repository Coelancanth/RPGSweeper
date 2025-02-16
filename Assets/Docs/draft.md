# Teleport/Swap Effect System Design

## Overview
The teleport/swap effect system will allow mines to move around the grid, either randomly or in specific patterns. This adds a dynamic element to the game where mines can change positions based on triggers or turn events.

## Core Components

### 1. TeleportEffectData
```csharp
public enum TeleportType
{
    Random,           // Move to any valid position
    DirectionalShift, // Move in a specific direction
    RowEnd,          // Move to end of current row
    ColumnEnd,       // Move to end of current column
    SwapNearest,     // Swap with nearest mine
    SwapRandom       // Swap with random mine
}

[CreateAssetMenu(fileName = "TeleportEffectData", menuName = "RPGMinesweeper/Effects/TeleportEffectData")]
public class TeleportEffectData : EffectData
{
    [SerializeField] private TeleportType m_TeleportType;
    [SerializeField] private Vector2Int m_Direction; // For DirectionalShift
    [SerializeField] private bool m_TriggerOnTurnEnd = false;
    [SerializeField] private float m_TeleportChance = 1f; // Probability of teleport occurring
    
    public TeleportType TeleportType => m_TeleportType;
    public Vector2Int Direction => m_Direction;
    public bool TriggerOnTurnEnd => m_TriggerOnTurnEnd;
    public float TeleportChance => m_TeleportChance;
}
```

### 2. TeleportEffect
```csharp
public class TeleportEffect : IEffect
{
    private readonly TeleportEffectData m_Data;
    private readonly MineManager m_MineManager;
    private readonly GridManager m_GridManager;
    
    public void Apply(GameObject target, Vector2Int position)
    {
        if (Random.value > m_Data.TeleportChance) return;
        
        Vector2Int newPosition = CalculateTargetPosition(position);
        if (newPosition != position)
        {
            PerformTeleport(position, newPosition);
        }
    }
    
    private Vector2Int CalculateTargetPosition(Vector2Int currentPos)
    {
        switch (m_Data.TeleportType)
        {
            case TeleportType.Random:
                return GetRandomValidPosition();
            case TeleportType.DirectionalShift:
                return GetDirectionalPosition(currentPos);
            case TeleportType.RowEnd:
                return GetRowEndPosition(currentPos);
            case TeleportType.ColumnEnd:
                return GetColumnEndPosition(currentPos);
            case TeleportType.SwapNearest:
                return FindNearestMine(currentPos);
            case TeleportType.SwapRandom:
                return GetRandomMinePosition();
            default:
                return currentPos;
        }
    }
}
```

## Implementation Plan

### Phase 1: Core Teleport System
1. Create `TeleportEffectData` and `TeleportEffect` classes
2. Implement basic position calculation methods
3. Add teleport effect to existing effect system
4. Test basic random teleportation

### Phase 2: Advanced Movement Patterns
1. Implement directional movement
2. Add row/column end targeting
3. Create swap functionality
4. Add turn-end trigger support

### Phase 3: Visual Feedback
1. Add animation system for teleporting mines
2. Implement visual indicators for teleport destinations
3. Create particle effects for teleportation

### Phase 4: Integration
1. Update `MineManager` to handle mine position updates
2. Modify `CellView` to support teleport animations
3. Add event system for teleport notifications
4. Implement proper value recalculation after teleports

## Technical Considerations

### 1. Position Validation
- Check for grid boundaries
- Handle occupied positions
- Maintain proper mine spacing
- Prevent infinite loops in position finding

### 2. State Management
- Update mine position dictionaries
- Maintain proper cell references
- Handle revealed state preservation
- Update adjacent cell values

### 3. Visual Updates
- Smooth transition animations
- Clear visual feedback
- Proper sprite/text updates
- Maintain visual consistency

### 4. Performance
- Efficient position calculation
- Optimize swap operations
- Minimize unnecessary updates
- Cache frequently used values

## Example Usage

```csharp
// Example teleport effect configuration
[CreateAssetMenu(fileName = "RandomTeleportMine", menuName = "RPGMinesweeper/Mines/RandomTeleportMine")]
public class TeleportMineData : MineData
{
    protected override void InitializeEffects()
    {
        var teleportEffect = CreateInstance<TeleportEffectData>();
        teleportEffect.Initialize(TeleportType.Random, 0.5f);
        
        m_ActiveEffects = new EffectData[] { teleportEffect };
    }
}
```

## Next Steps
1. Implement `TeleportEffectData` and `TeleportEffect` classes
2. Add position calculation methods
3. Create test mine prefab with teleport effect
4. Implement basic visual feedback
5. Test and refine the system

## Questions to Consider
1. Should teleported mines maintain their revealed state?
2. How should teleport effects interact with other effects?
3. Should there be a cooldown between teleports?
4. How to handle teleport chains/cascades?
5. Should certain mine types be immune to teleportation?

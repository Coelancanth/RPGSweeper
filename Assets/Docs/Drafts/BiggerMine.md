# Bigger Mine Implementation Analysis

## Current Implementation Overview
The current system uses a 1x1 grid-based mine placement system with several key components:
- `MineSpawner`: Handles mine placement using various strategies
- `SpawnContext`: Manages grid state and available positions
- `MineData`: Stores mine-specific data
- Various spawn strategies (Random, Edge, Corner, etc.)

## Design Considerations for Bigger Mines

### 1. Shape Definition
- Introduce a new `MineShape` class/struct to define mine shapes
- Shapes could be defined as:
  ```csharp
  public struct MineShape {
      public Vector2Int[] Positions; // Relative to mine center
      public Vector2Int Center;      // Reference point
  }
  ```

### 2. Required Changes

#### Core Components
1. **MineData Modifications**
   - Add shape information
   - Update sprite handling for larger shapes
   - Consider rotation support

2. **SpawnContext Updates**
   - Modify `IsPositionAvailable` to check all cells in shape
   - Update position validation logic

3. **MineSpawner Changes**
   - Enhance placement logic to consider full shape
   - Add shape validation during placement

4. **Visual System**
   - Update cell visualization for multi-cell mines
   - Handle revealing multiple cells
   - Consider special effects for larger mines

#### New Features
1. **Shape Registry**
   ```csharp
   public class MineShapeRegistry {
       Dictionary<string, MineShape> Shapes;
       // Methods for registering and retrieving shapes
   }
   ```

2. **Shape Validation System**
   - Ensure shapes fit within grid
   - Check for overlaps with other mines
   - Validate shape connectivity

## Implementation Steps

1. **Phase 1: Core Structure**
   - Create `MineShape` class
   - Implement shape registry
   - Update `MineData` to support shapes

2. **Phase 2: Spawn System Updates**
   - Modify `SpawnContext` for shape awareness
   - Update spawn strategies to handle shapes
   - Implement shape validation

3. **Phase 3: Visual Updates**
   - Create new sprites for bigger mines
   - Update `MineVisualManager`
   - Implement multi-cell reveal effects

4. **Phase 4: Gameplay Integration**
   - Update scoring system for bigger mines
   - Adjust difficulty balancing
   - Add shape-specific effects

## Technical Considerations

### Performance
- Cache shape validations
- Optimize collision checks
- Consider chunking for large grids

### Memory
- Pool shape objects
- Optimize shape storage
- Use flyweight pattern for shared shape data

### Extensibility
- Design for future shape types
- Allow for dynamic shape creation
- Support for shape rotation/transformation

## Example Shapes
```
2x2 Mine:     3x3 Mine:     L-Shape Mine:
[X][X]        [X][X][X]     [X][ ]
[X][X]        [X][X][X]     [X][ ]
              [X][X][X]     [X][X]
```

## Impact on Existing Systems

1. **Grid Management**
   - Update neighbor calculations
   - Modify cell state tracking
   - Enhance path finding

2. **Game Logic**
   - Adjust win condition checking
   - Update mine counting
   - Modify save/load system

3. **UI/UX**
   - Update cell highlighting
   - Enhance mine markers
   - Add shape indicators

## Next Steps
1. Create prototype implementation
2. Test with basic shapes
3. Gather feedback on gameplay impact
4. Iterate on visual design
5. Implement full shape library

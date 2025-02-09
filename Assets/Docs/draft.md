# Mine Number Indication Feature Design

## Overview
Each cell should display a number indicating:
1. If the cell has no mine: Sum of values from adjacent mines that affect this cell
2. If the cell has a mine: Sum of values from mines that affect this cell (including itself)

## Case Analysis

### Example Setup
Consider a grid with two mines:
1. Mine A: Single shape, range 1, value 3
2. Mine B: Square shape, range 2, value 5

### Cell Value Calculation Cases

```
Grid Representation (B = Mine B center, A = Mine A)
For cells with mines:
. . 5 5 5 .
. 5 5 5 5 .
5 5 8 5 5 .  // 8 = 3 (from A) + 5 (from B)
. 5 5 5 5 .
. . 5 5 5 .

For cells without mines (showing adjacent values):
3 3 5 5 5 3
3 5 5 5 5 3
5 5 8 5 5 3  // Empty cell next to both A(3) and B(5) shows 8
3 5 5 5 5 3
3 3 5 5 5 3
```

#### Case 1: Cell with Mine
- When a cell contains a mine:
  - Single shape mine (A): Cell shows value 3
  - Square shape mine (B): Cell shows value 5
  - Multiple mines: Sum of all mine values affecting the cell

#### Case 2: Empty Cell Adjacent to Mines
- When a cell has no mine but is adjacent to mines:
  - Sum all mine values that affect this cell based on each mine's shape and range
  - Example: Empty cell adjacent to both mine A and B shows 8 (3 + 5)

#### Case 3: Edge Cases
1. Cell outside all mine ranges: Shows 0 or empty
2. Cell at shape edge: Must check exact shape pattern
3. Overlapping shapes: Sum all applicable values
4. Empty cell with no adjacent mines: Shows 0 or empty

## Implementation Strategy

### 1. Shape Pattern Validation
```csharp
public bool IsAffectedByMine(Vector2Int cellPos, Vector2Int minePos, MineData mineData)
{
    return mineData.IsPositionInShape(cellPos, minePos);
}
```

### 2. Value Calculation
```csharp
public int CalculateCellValue(Vector2Int cellPos)
{
    int sum = 0;
    
    // If cell has no mine, check adjacent cells for mines
    if (!HasMine(cellPos))
    {
        foreach (var mine in mines)
        {
            if (mine.IsPositionInShape(cellPos, mine.Position))
            {
                sum += mine.Value;
            }
        }
        return sum;
    }
    
    // If cell has mine, sum all mine values affecting this cell
    foreach (var mine in mines)
    {
        if (mine.IsPositionInShape(cellPos, mine.Position))
        {
            sum += mine.Value;
        }
    }
    return sum;
}
```

### 3. Visual Updates
- Add TextMeshPro component to CellView
- Show number only when cell is revealed
- Use color coding for different value ranges:
  - 0: Hidden or transparent
  - 1-3: Blue
  - 4-6: Green
  - 7-9: Orange
  - 10+: Red
- Different styles for mine cells vs adjacent cells

### 4. Performance Considerations
- Precalculate values during mine placement
- Cache results in grid data structure
- Update only affected cells when mines change
- Use spatial partitioning to optimize adjacent cell checks

## Required Changes

1. CellView.cs:
   - Add TextMeshPro component
   - Add methods for setting/updating number
   - Add color configuration for different ranges
   - Add distinction between mine and adjacent numbers

2. MineManager.cs:
   - Add method to calculate cell values
   - Track affected cells for each mine
   - Update cell values on mine changes
   - Add methods to check adjacent cells

3. Grid.cs:
   - Add value tracking per cell
   - Add methods for value queries
   - Add methods for adjacent cell queries

## Next Steps

1. Implement value calculation system
   - Add mine value calculation
   - Add adjacent cell calculation
2. Update visual representation
   - Add number display
   - Add color coding
3. Add caching mechanism
4. Add color coding configuration
5. Update existing mine placement logic
6. Add tests for edge cases
7. Optimize performance for large grids

## SpecStory

## Fixing Value Propagation Bug in Game Code (2/5/2025, 11:42:04 PM)

_**User**_

there are bugs, the 2 rightmost columns' update isn't right

---

_**Assistant**_

Looking at the image and code, I can see that the values in the rightmost columns aren't propagating correctly. The issue is likely in how we're handling the value propagation. Let me propose a fix:

```csharp:Assets/Scripts/Core/Mines/MineValuePropagator.cs
// ... existing code ...

    private static void PropagateValueFromMine(Vector2Int minePosition, MineData mineData, Dictionary<Vector2Int, int> cellValues, MineManager mineManager, GridManager gridManager)
    {
        var affectedPositions = MineShapeHelper.GetShapePositions(minePosition, mineData.Shape, mineData.Radius);
        
        // Add the mine's own position to affected positions if it's not already included
        if (!affectedPositions.Contains(minePosition))
        {
            affectedPositions.Add(minePosition);
        }

        foreach (var position in affectedPositions)
        {
            // Remove the condition that excludes mine positions
            if (gridManager.IsValidPosition(position))
            {
                if (!cellValues.ContainsKey(position))
                {
                    cellValues[position] = 0;
                }
                cellValues[position] += mineData.Value;
            }
        }
    }

// ... existing code ...
```

The main issue was in the `PropagateValueFromMine` method where we were excluding positions that had mines (`!mineManager.HasMineAt(position)`). This was preventing the values from being properly propagated in cells that contain mines. I've made two changes:

1. Removed the condition `!mineManager.HasMineAt(position)` so that mine positions can also accumulate values
2. Added the mine's own position to the affected positions if it's not already included

This should fix the value propagation in the rightmost columns where mines are present. The numbers should now correctly reflect the influence of all nearby mines, including when mines are adjacent to each other.

---



---


using UnityEngine;
using System.Collections.Generic;

public static class MineValuePropagator
{
    public static void PropagateValues(MineManager mineManager, GridManager gridManager)
    {
        if (mineManager == null)
        {
            Debug.LogError("MineValuePropagator: MineManager is null!");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError("MineValuePropagator: GridManager is null!");
            return;
        }

        var cellValues = new Dictionary<Vector2Int, int>();

        // First pass: Calculate values for all cells
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                var position = new Vector2Int(x, y);
                if (mineManager.HasMineAt(position))
                {
                    var mineData = mineManager.GetMineDataAt(position);
                    if (mineData != null)
                    {
                        PropagateValueFromMine(position, mineData, cellValues, mineManager, gridManager);
                    }
                }
            }
        }

        // Second pass: Apply values to cells
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                var position = new Vector2Int(x, y);
                var cellObject = gridManager.GetCellObject(position);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        // Only set a value if this cell is affected by at least one mine
                        int value = 0;
                        if (cellValues.TryGetValue(position, out int calculatedValue))
                        {
                            value = calculatedValue;
                        }
                        cellView.SetValue(value);
                    }
                }
            }
        }
    }

    private static void PropagateValueFromMine(Vector2Int minePosition, MineData mineData, Dictionary<Vector2Int, int> cellValues, MineManager mineManager, GridManager gridManager)
    {
        var affectedPositions = MineShapeHelper.GetShapePositions(minePosition, mineData.Shape, mineData.Radius);
        foreach (var position in affectedPositions)
        {
            if (gridManager.IsValidPosition(position) && !mineManager.HasMineAt(position))
            {
                if (!cellValues.ContainsKey(position))
                {
                    cellValues[position] = 0;
                }
                cellValues[position] += mineData.Value;
            }
        }
    }
} 
using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper;
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

        // First pass: Calculate base values for all cells
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                var position = new Vector2Int(x, y);
                cellValues[position] = 0;
            }
        }

        // Second pass: Calculate values for cells affected by mines
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

        // Third pass: Apply calculated values to cells, considering active effects
        foreach (var kvp in cellValues)
        {
            var cellObject = gridManager.GetCellObject(kvp.Key);
            if (cellObject != null)
            {
                var cellView = cellObject.GetComponent<CellView>();
                if (cellView != null)
                {
                    // Get effect modifications and color
                    var (modifiedValue, effectColor) = MineValueModifier.ModifyValueAndGetColor(kvp.Key, kvp.Value);
                    cellView.SetValue(modifiedValue, effectColor);
                }
            }
        }
    }

    private static void PropagateValueFromMine(Vector2Int minePosition, MineData mineData, Dictionary<Vector2Int, int> cellValues, MineManager mineManager, GridManager gridManager)
    {
        var affectedPositions = GridShapeHelper.GetAffectedPositions(minePosition, mineData.Shape, mineData.Radius);
        foreach (var position in affectedPositions)
        {
            if (gridManager.IsValidPosition(position) && !mineManager.HasMineAt(position))
            {
                cellValues[position] += mineData.Value;
            }
        }
    }
} 
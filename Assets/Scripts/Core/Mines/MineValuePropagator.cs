using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Grid;

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

        // First pass: Reset all cell values to 0
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
                        cellView.SetValue(0);
                    }
                }
            }
        }

        var cellValues = new Dictionary<Vector2Int, int>();

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

        // Third pass: Apply calculated values to cells
        foreach (var kvp in cellValues)
        {
            var cellObject = gridManager.GetCellObject(kvp.Key);
            if (cellObject != null)
            {
                var cellView = cellObject.GetComponent<CellView>();
                if (cellView != null)
                {
                    cellView.SetValue(kvp.Value);
                }
            }
        }
    }

    private static void PropagateValueFromMine(Vector2Int minePosition, MineData mineData, Dictionary<Vector2Int, int> cellValues, MineManager mineManager, GridManager gridManager)
    {
        //Debug.Log($"MineValuePropagator: Propagating value from mine at position {minePosition}");
        var affectedPositions = GridShapeHelper.GetAffectedPositions(minePosition, mineData.Shape, mineData.Radius);
        foreach (var position in affectedPositions  )
        {
            //Debug.Log($"MineValuePropagator: Affected position: {position}");
        }
        foreach (var position in affectedPositions)
        {
            if (gridManager.IsValidPosition(position) && !mineManager.HasMineAt(position))
            {
                if (!cellValues.ContainsKey(position))
                {
                    cellValues[position] = 0;
                }
                cellValues[position] += mineData.Value;
                //Debug.Log($"MineValuePropagator: Propagating value {mineData.Value} to position {position}, total value: {cellValues[position]}");
            }
        }
    }
} 
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
                    //Debug.Log($"MineValuePropagator: Found mine at position {position}");
                    if (mineData != null)
                    {
                        //Debug.Log($"MineValuePropagator: Propagating value from mine at position {position}");
                        PropagateValueFromMine(position, mineData, cellValues, mineManager, gridManager);
                    }
                }
            }
        }

        // Second pass: Apply values to cells
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
        var affectedPositions = MineShapeHelper.GetShapePositions(minePosition, mineData.Shape, mineData.Radius);
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
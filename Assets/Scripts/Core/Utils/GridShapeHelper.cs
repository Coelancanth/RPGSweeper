using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.Grid
{
    public static class GridShapeHelper
    {
        public static List<Vector2Int> GetAffectedPositions(Vector2Int center, GridShape shape, int range)
        {
            var positions = new List<Vector2Int>();
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            
            switch (shape)
            {
                case GridShape.Single:
                    positions.Add(center);
                    break;
                    
                case GridShape.Cross:
                    positions.Add(center);
                    for (int i = 1; i <= range; i++)
                    {
                        positions.Add(center + new Vector2Int(i, 0));    // Right
                        positions.Add(center + new Vector2Int(-i, 0));   // Left
                        positions.Add(center + new Vector2Int(0, i));    // Up
                        positions.Add(center + new Vector2Int(0, -i));   // Down
                    }
                    break;
                    
                case GridShape.Square:
                    for (int x = -range; x <= range; x++)
                    {
                        for (int y = -range; y <= range; y++)
                        {
                            positions.Add(center + new Vector2Int(x, y));
                        }
                    }
                    break;
                    
                case GridShape.Diamond:
                    for (int x = -range; x <= range; x++)
                    {
                        for (int y = -range; y <= range; y++)
                        {
                            if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                            {
                                positions.Add(center + new Vector2Int(x, y));
                            }
                        }
                    }
                    break;
                    
                case GridShape.Line:
                    positions.Add(center);
                    for (int i = 1; i <= range; i++)
                    {
                        positions.Add(center + new Vector2Int(i, 0));    // Right
                        positions.Add(center + new Vector2Int(-i, 0));   // Left
                    }
                    break;

                case GridShape.WholeGrid:
                    if (gridManager != null)
                    {
                        // For WholeGrid, range is ignored - we use the actual grid dimensions
                        for (int x = 0; x < gridManager.Width; x++)
                        {
                            for (int y = 0; y < gridManager.Height; y++)
                            {
                                positions.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                    break;

                case GridShape.Row:
                    if (gridManager != null)
                    {
                        // For Row, affect the entire row at center.y
                        for (int x = 0; x < gridManager.Width; x++)
                        {
                            positions.Add(new Vector2Int(x, center.y));
                        }
                    }
                    break;

                case GridShape.Column:
                    if (gridManager != null)
                    {
                        // For Column, affect the entire column at center.x
                        for (int y = 0; y < gridManager.Height; y++)
                        {
                            positions.Add(new Vector2Int(center.x, y));
                        }
                    }
                    break;
            }
            //Debug.Log($"GridShapeHelper: Shape {shape} with range {range} has {positions.Count} positions");
            return positions;
        }
        
        public static bool IsPositionAffected(Vector2Int position, Vector2Int center, GridShape shape, int range)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            
            switch (shape)
            {
                case GridShape.Single:
                    return position == center;
                    
                case GridShape.Cross:
                    var diff = position - center;
                    return (diff.x == 0 && Mathf.Abs(diff.y) <= range) || 
                           (diff.y == 0 && Mathf.Abs(diff.x) <= range);
                    
                case GridShape.Square:
                    return Mathf.Abs(position.x - center.x) <= range && 
                           Mathf.Abs(position.y - center.y) <= range;
                    
                case GridShape.Diamond:
                    return Mathf.Abs(position.x - center.x) + 
                           Mathf.Abs(position.y - center.y) <= range;
                    
                case GridShape.Line:
                    diff = position - center;
                    return diff.y == 0 && Mathf.Abs(diff.x) <= range;

                case GridShape.WholeGrid:
                    if (gridManager != null)
                    {
                        // For WholeGrid, we only need to check if the position is within grid bounds
                        return gridManager.IsValidPosition(position);
                    }
                    return false;

                case GridShape.Row:
                    if (gridManager != null)
                    {
                        // For Row, check if position is in the same row and within grid bounds
                        return position.y == center.y && gridManager.IsValidPosition(position);
                    }
                    return false;

                case GridShape.Column:
                    if (gridManager != null)
                    {
                        // For Column, check if position is in the same column and within grid bounds
                        return position.x == center.x && gridManager.IsValidPosition(position);
                    }
                    return false;
                    
                default:
                    return false;
            }
        }
    }
} 
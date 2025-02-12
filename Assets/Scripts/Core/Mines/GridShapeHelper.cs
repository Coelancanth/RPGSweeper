using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.Grid
{
    public static class GridShapeHelper
    {
        public static List<Vector2Int> GetAffectedPositions(Vector2Int center, GridShape shape, int range)
        {
            var positions = new List<Vector2Int>();
            
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
            }
            //Debug.Log($"MineShapeHelper: Shape {shape} with range {range} has {positions.Count} positions");
            return positions;
        }
        
        public static bool IsPositionAffected(Vector2Int position, Vector2Int center, GridShape shape, int range)
        {
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
                    
                default:
                    return false;
            }
        }
    }
} 
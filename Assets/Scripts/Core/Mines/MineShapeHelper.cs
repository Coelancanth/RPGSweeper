using UnityEngine;
using System.Collections.Generic;

public static class MineShapeHelper
{
    public static List<Vector2Int> GetShapePositions(Vector2Int center, MineShape shape, int range)
    {
        var positions = new List<Vector2Int>();
        
        switch (shape)
        {
            case MineShape.Single:
                positions.Add(center);
                break;
                
            case MineShape.Cross:
                positions.Add(center);
                for (int i = 1; i <= range; i++)
                {
                    positions.Add(center + new Vector2Int(i, 0));    // Right
                    positions.Add(center + new Vector2Int(-i, 0));   // Left
                    positions.Add(center + new Vector2Int(0, i));    // Up
                    positions.Add(center + new Vector2Int(0, -i));   // Down
                }
                break;
                
            case MineShape.Square:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        positions.Add(center + new Vector2Int(x, y));
                    }
                }
                break;
                
            case MineShape.Diamond:
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
                
            case MineShape.Line:
                positions.Add(center);
                for (int i = 1; i <= range; i++)
                {
                    positions.Add(center + new Vector2Int(i, 0));    // Right
                    positions.Add(center + new Vector2Int(-i, 0));   // Left
                }
                break;
        }
        
        return positions;
    }
    
    public static bool IsPositionInShape(Vector2Int position, Vector2Int center, MineShape shape, int range)
    {
        switch (shape)
        {
            case MineShape.Single:
                return position == center;
                
            case MineShape.Cross:
                var diff = position - center;
                return (diff.x == 0 && Mathf.Abs(diff.y) <= range) || 
                       (diff.y == 0 && Mathf.Abs(diff.x) <= range);
                
            case MineShape.Square:
                return Mathf.Abs(position.x - center.x) <= range && 
                       Mathf.Abs(position.y - center.y) <= range;
                
            case MineShape.Diamond:
                return Mathf.Abs(position.x - center.x) + 
                       Mathf.Abs(position.y - center.y) <= range;
                
            case MineShape.Line:
                diff = position - center;
                return diff.y == 0 && Mathf.Abs(diff.x) <= range;
                
            default:
                return false;
        }
    }
} 
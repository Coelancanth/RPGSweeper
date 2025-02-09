using UnityEngine;
using System.Collections.Generic;

namespace RPGMinesweeper.Core.Mines
{
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
                        positions.Add(center + new Vector2Int(i, 0));
                        positions.Add(center + new Vector2Int(-i, 0));
                        positions.Add(center + new Vector2Int(0, i));
                        positions.Add(center + new Vector2Int(0, -i));
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
                    for (int i = -range; i <= range; i++)
                    {
                        positions.Add(center + new Vector2Int(i, 0));
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
                    Vector2Int delta = position - center;
                    return (delta.x == 0 && Mathf.Abs(delta.y) <= range) || 
                           (delta.y == 0 && Mathf.Abs(delta.x) <= range);
                    
                case MineShape.Square:
                    return Mathf.Abs(position.x - center.x) <= range && 
                           Mathf.Abs(position.y - center.y) <= range;
                    
                case MineShape.Diamond:
                    return Mathf.Abs(position.x - center.x) + 
                           Mathf.Abs(position.y - center.y) <= range;
                    
                case MineShape.Line:
                    return position.y == center.y && 
                           Mathf.Abs(position.x - center.x) <= range;
                    
                default:
                    return false;
            }
        }
    }
} 
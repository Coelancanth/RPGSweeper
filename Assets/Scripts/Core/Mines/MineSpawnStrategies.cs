using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper
{
    public class RandomMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            Vector2Int position;
            do
            {
                position = new Vector2Int(
                    Random.Range(0, gridManager.Width),
                    Random.Range(0, gridManager.Height)
                );
            } while (existingMines.ContainsKey(position));

            return position;
        }
    }

    public class EdgeMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            List<Vector2Int> edgePositions = new List<Vector2Int>();
            
            // Add top and bottom edges
            for (int x = 0; x < gridManager.Width; x++)
            {
                edgePositions.Add(new Vector2Int(x, 0));
                edgePositions.Add(new Vector2Int(x, gridManager.Height - 1));
            }
            
            // Add left and right edges (excluding corners already added)
            for (int y = 1; y < gridManager.Height - 1; y++)
            {
                edgePositions.Add(new Vector2Int(0, y));
                edgePositions.Add(new Vector2Int(gridManager.Width - 1, y));
            }
            
            // Filter out positions that already have mines
            edgePositions.RemoveAll(pos => existingMines.ContainsKey(pos));
            
            // If no edge positions are available, fallback to random position
            if (edgePositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }
            
            // Return random edge position
            return edgePositions[Random.Range(0, edgePositions.Count)];
        }
    }

    public class CornerMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            List<Vector2Int> cornerPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),                                     // Bottom-left
                new Vector2Int(gridManager.Width - 1, 0),                // Bottom-right
                new Vector2Int(0, gridManager.Height - 1),               // Top-left
                new Vector2Int(gridManager.Width - 1, gridManager.Height - 1)  // Top-right
            };

            // Filter out positions that already have mines
            cornerPositions.RemoveAll(pos => existingMines.ContainsKey(pos));

            // If no corner positions are available, fallback to random position
            if (cornerPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            // Return random corner position
            return cornerPositions[Random.Range(0, cornerPositions.Count)];
        }
    }

    public class CenterMineSpawnStrategy : IMineSpawnStrategy
    {
        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            // Define the center region (middle 1/3 of the grid)
            int minX = gridManager.Width / 3;
            int maxX = (gridManager.Width * 2) / 3;
            int minY = gridManager.Height / 3;
            int maxY = (gridManager.Height * 2) / 3;

            List<Vector2Int> centerPositions = new List<Vector2Int>();
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!existingMines.ContainsKey(pos))
                    {
                        centerPositions.Add(pos);
                    }
                }
            }

            // If no center positions are available, fallback to random position
            if (centerPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            // Return random center position
            return centerPositions[Random.Range(0, centerPositions.Count)];
        }
    }

    public class SurroundedMineSpawnStrategy : IMineSpawnStrategy
    {
        private readonly MineType m_TargetMineType;
        private readonly MonsterType? m_TargetMonsterType;
        private static readonly Vector2Int[] s_AdjacentOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
            new Vector2Int(-1, 0),                         new Vector2Int(1, 0),
            new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1)
        };

        public SurroundedMineSpawnStrategy(MineType targetMineType, MonsterType? targetMonsterType = null)
        {
            m_TargetMineType = targetMineType;
            m_TargetMonsterType = targetMonsterType;
        }

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            var targetPositions = existingMines
                .Where(kvp => IsTargetMine(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            if (targetPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            List<Vector2Int> validPositions = new List<Vector2Int>();

            foreach (var targetPos in targetPositions)
            {
                foreach (var offset in s_AdjacentOffsets)
                {
                    var adjacentPos = targetPos + offset;
                    
                    // Check if position is within grid bounds
                    if (adjacentPos.x >= 0 && adjacentPos.x < gridManager.Width &&
                        adjacentPos.y >= 0 && adjacentPos.y < gridManager.Height &&
                        !existingMines.ContainsKey(adjacentPos))
                    {
                        validPositions.Add(adjacentPos);
                    }
                }
            }

            // If no valid positions found, fallback to random
            if (validPositions.Count == 0)
            {
                return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
            }

            return validPositions[Random.Range(0, validPositions.Count)];
        }

        private bool IsTargetMine(IMine mine)
        {
            if (mine.Type != m_TargetMineType) return false;
            
            // If we're targeting a monster type, check if it matches
            if (m_TargetMineType == MineType.Monster && m_TargetMonsterType.HasValue)
            {
                if (mine is MonsterMine monsterMine)
                {
                    return monsterMine.MonsterType == m_TargetMonsterType.Value;
                }
                return false;
            }
            
            return true;
        }
    }
} 
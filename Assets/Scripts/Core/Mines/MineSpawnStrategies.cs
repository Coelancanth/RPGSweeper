using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper
{
    public class RandomMineSpawnStrategy : IMineSpawnStrategy
    {
        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Random;

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            Vector2Int position;
            int attempts = 0;
            const int maxAttempts = 100; // Prevent infinite loops

            do
            {
                position = new Vector2Int(
                    Random.Range(0, gridManager.Width),
                    Random.Range(0, gridManager.Height)
                );
                attempts++;

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("RandomMineSpawnStrategy: Max attempts reached, no valid position found.");
                    return Vector2Int.one * -1; // Signal invalid position
                }
            } while (existingMines.ContainsKey(position));

            return position;
        }
    }

    public class EdgeMineSpawnStrategy : IMineSpawnStrategy
    {
        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Edge;

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
            
            edgePositions.RemoveAll(pos => existingMines.ContainsKey(pos));
            
            if (edgePositions.Count == 0)
            {
                Debug.LogWarning("EdgeMineSpawnStrategy: No valid edge positions available.");
                return Vector2Int.one * -1;
            }
            
            return edgePositions[Random.Range(0, edgePositions.Count)];
        }
    }

    public class CornerMineSpawnStrategy : IMineSpawnStrategy
    {
        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Corner;

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            List<Vector2Int> cornerPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),                                     // Bottom-left
                new Vector2Int(gridManager.Width - 1, 0),                // Bottom-right
                new Vector2Int(0, gridManager.Height - 1),               // Top-left
                new Vector2Int(gridManager.Width - 1, gridManager.Height - 1)  // Top-right
            };

            cornerPositions.RemoveAll(pos => existingMines.ContainsKey(pos));

            if (cornerPositions.Count == 0)
            {
                Debug.LogWarning("CornerMineSpawnStrategy: No valid corner positions available.");
                return Vector2Int.one * -1;
            }

            return cornerPositions[Random.Range(0, cornerPositions.Count)];
        }
    }

    public class CenterMineSpawnStrategy : IMineSpawnStrategy
    {
        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Center;

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
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

            if (centerPositions.Count == 0)
            {
                Debug.LogWarning("CenterMineSpawnStrategy: No valid center positions available.");
                return Vector2Int.one * -1;
            }

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

        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Surrounded;

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
                Debug.LogWarning($"SurroundedMineSpawnStrategy: No target mines found of type {m_TargetMineType}" + 
                    (m_TargetMonsterType.HasValue ? $" and monster type {m_TargetMonsterType.Value}" : ""));
                return Vector2Int.one * -1;
            }

            List<Vector2Int> validPositions = new List<Vector2Int>();

            foreach (var targetPos in targetPositions)
            {
                foreach (var offset in s_AdjacentOffsets)
                {
                    var adjacentPos = targetPos + offset;
                    
                    if (IsValidPosition(adjacentPos, gridManager, existingMines))
                    {
                        validPositions.Add(adjacentPos);
                    }
                }
            }

            if (validPositions.Count == 0)
            {
                Debug.LogWarning($"SurroundedMineSpawnStrategy: No valid adjacent positions found for target mines.");
                return Vector2Int.one * -1;
            }

            return validPositions[Random.Range(0, validPositions.Count)];
        }

        private bool IsValidPosition(Vector2Int pos, GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            return pos.x >= 0 && pos.x < gridManager.Width &&
                   pos.y >= 0 && pos.y < gridManager.Height &&
                   !existingMines.ContainsKey(pos);
        }

        private bool IsTargetMine(IMine mine)
        {
            if (mine == null) return false;

            bool isCorrectType = mine.Type == m_TargetMineType;
            if (!isCorrectType) return false;

            if (m_TargetMonsterType.HasValue && mine is MonsterMine monsterMine)
            {
                return monsterMine.MonsterType == m_TargetMonsterType.Value;
            }

            return true;
        }
    }

    public class SymmetricMineSpawnStrategy : IMineSpawnStrategy
    {
        private readonly SymmetryDirection m_Direction;
        private readonly RandomMineSpawnStrategy m_RandomStrategy;
        private Vector2Int? m_LastSymmetricPosition = null;

        public SpawnStrategyPriority Priority => SpawnStrategyPriority.Symmetric;

        public SymmetricMineSpawnStrategy(SymmetryDirection direction)
        {
            m_Direction = direction;
            m_RandomStrategy = new RandomMineSpawnStrategy();
        }

        public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
        {
            // If we have a stored symmetric position, return and clear it
            if (m_LastSymmetricPosition.HasValue)
            {
                var position = m_LastSymmetricPosition.Value;
                m_LastSymmetricPosition = null;
                return position;
            }

            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                // Get a random position for the first mine
                Vector2Int position = m_RandomStrategy.GetSpawnPosition(gridManager, existingMines);
                if (position == Vector2Int.one * -1) return position;

                // Calculate the symmetric position
                Vector2Int symmetricPos = GetSymmetricPosition(position, gridManager);

                // Check if both positions are valid
                if (!existingMines.ContainsKey(symmetricPos) && 
                    !existingMines.ContainsKey(position) && 
                    IsValidPosition(symmetricPos, gridManager))
                {
                    // Store the symmetric position for next call
                    m_LastSymmetricPosition = symmetricPos;
                    return position;
                }

                attempts++;
            }

            Debug.LogWarning("SymmetricMineSpawnStrategy: Max attempts reached, no valid symmetric positions found.");
            return Vector2Int.one * -1;
        }

        private Vector2Int GetSymmetricPosition(Vector2Int position, GridManager gridManager)
        {
            switch (m_Direction)
            {
                case SymmetryDirection.Horizontal:
                    // Mirror across vertical center line
                    return new Vector2Int(
                        gridManager.Width - 1 - position.x,
                        position.y
                    );

                case SymmetryDirection.Vertical:
                    // Mirror across horizontal center line
                    return new Vector2Int(
                        position.x,
                        gridManager.Height - 1 - position.y
                    );

                default:
                    Debug.LogError($"Unsupported symmetry direction: {m_Direction}");
                    return position;
            }
        }

        private bool IsValidPosition(Vector2Int position, GridManager gridManager)
        {
            return position.x >= 0 && position.x < gridManager.Width &&
                   position.y >= 0 && position.y < gridManager.Height;
        }
    }
} 
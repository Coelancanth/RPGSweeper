using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class AdjacentSpawnStrategy : MineSpawnStrategyBase
    {
        private readonly int m_MaxDistance;
        private readonly int m_MinDistance;
        private readonly bool m_AllowDiagonal;
        private readonly int m_MinClusterSize;
        private readonly int m_MaxClusterSize;
        private readonly AdjacentDirection m_Direction;

        public override SpawnStrategyType Priority => SpawnStrategyType.Relational;

        public AdjacentSpawnStrategy(
            int minDistance = 2,
            int maxDistance = int.MaxValue,
            bool allowDiagonal = false,
            int minClusterSize = 2,
            int maxClusterSize = 4,
            AdjacentDirection direction = AdjacentDirection.Any)
        {
            m_MinDistance = Mathf.Max(1, minDistance);
            m_MaxDistance = maxDistance;
            m_AllowDiagonal = allowDiagonal;
            m_MinClusterSize = Mathf.Max(2, minClusterSize);
            m_MaxClusterSize = Mathf.Max(minClusterSize, maxClusterSize);
            m_Direction = direction;
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            var availablePositions = context.GetAvailablePositions().ToList();
            return availablePositions.Count >= m_MinClusterSize;
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var mines = new List<SpawnedMine>();
            var availablePositions = context.GetAvailablePositions().ToList();
            var remainingCount = spawnData.SpawnCount;

            while (remainingCount > 0 && availablePositions.Count >= m_MinClusterSize)
            {
                // Start a new cluster
                var clusterSize = Mathf.Min(
                    Random.Range(m_MinClusterSize, m_MaxClusterSize + 1),
                    remainingCount
                );

                var clusterMines = PlaceCluster(context, spawnData, availablePositions, clusterSize);
                if (clusterMines.Count > 0)
                {
                    mines.AddRange(clusterMines);
                    remainingCount -= clusterMines.Count;

                    // Remove used positions and their neighbors within minDistance
                    foreach (var mine in clusterMines)
                    {
                        availablePositions.RemoveAll(p => 
                            GetDistance(p, mine.Position) <= m_MinDistance);
                    }
                }
                else
                {
                    break; // Unable to place more clusters
                }
            }

            return mines.Count > 0 
                ? SpawnResult.Successful(mines) 
                : SpawnResult.Failed("Could not place any adjacent mines");
        }

        private List<SpawnedMine> PlaceCluster(
            SpawnContext context,
            MineTypeSpawnData spawnData,
            List<Vector2Int> availablePositions,
            int clusterSize)
        {
            var clusterMines = new List<SpawnedMine>();
            
            // Pick random starting position
            var startPos = availablePositions[Random.Range(0, availablePositions.Count)];
            var firstMine = CreateMine(context, startPos, spawnData, FacingDirection.Right); // Temporary facing
            clusterMines.Add(firstMine);

            var currentPositions = new List<Vector2Int> { startPos };
            var remainingSize = clusterSize - 1;

            while (remainingSize > 0 && currentPositions.Count > 0)
            {
                var basePos = currentPositions[Random.Range(0, currentPositions.Count)];
                var adjacentPos = GetValidAdjacentPosition(basePos, availablePositions, context);

                if (adjacentPos.HasValue)
                {
                    // Determine facing directions for the pair
                    var (facing1, facing2) = GetFacingDirections(basePos, adjacentPos.Value);

                    // Update facing for existing mine if it's the base position
                    var existingMineIndex = clusterMines.FindIndex(m => m.Position == basePos);
                    if (existingMineIndex >= 0)
                    {
                        clusterMines[existingMineIndex] = CreateMine(context, basePos, spawnData, facing1);
                    }

                    // Add new mine
                    clusterMines.Add(CreateMine(context, adjacentPos.Value, spawnData, facing2));
                    currentPositions.Add(adjacentPos.Value);
                    remainingSize--;

                    // Remove used position
                    availablePositions.Remove(adjacentPos.Value);
                }
                else
                {
                    currentPositions.Remove(basePos);
                }
            }

            return clusterMines;
        }

        private Vector2Int? GetValidAdjacentPosition(
            Vector2Int basePos,
            List<Vector2Int> availablePositions,
            SpawnContext context)
        {
            var possibleOffsets = GetPossibleOffsets();
            var shuffledOffsets = possibleOffsets.OrderBy(_ => Random.value).ToList();

            foreach (var offset in shuffledOffsets)
            {
                var newPos = basePos + offset;
                if (IsValidPosition(newPos, context) && availablePositions.Contains(newPos))
                {
                    return newPos;
                }
            }

            return null;
        }

        private List<Vector2Int> GetPossibleOffsets()
        {
            var offsets = new List<Vector2Int>();

            switch (m_Direction)
            {
                case AdjacentDirection.Horizontal:
                    offsets.Add(Vector2Int.right);
                    offsets.Add(Vector2Int.left);
                    break;
                case AdjacentDirection.Vertical:
                    offsets.Add(Vector2Int.up);
                    offsets.Add(Vector2Int.down);
                    break;
                case AdjacentDirection.Any:
                    offsets.Add(Vector2Int.right);
                    offsets.Add(Vector2Int.left);
                    offsets.Add(Vector2Int.up);
                    offsets.Add(Vector2Int.down);
                    if (m_AllowDiagonal)
                    {
                        offsets.Add(new Vector2Int(1, 1));
                        offsets.Add(new Vector2Int(-1, 1));
                        offsets.Add(new Vector2Int(1, -1));
                        offsets.Add(new Vector2Int(-1, -1));
                    }
                    break;
            }

            return offsets;
        }

        private (FacingDirection, FacingDirection) GetFacingDirections(Vector2Int pos1, Vector2Int pos2)
        {
            var diff = pos2 - pos1;
            
            if (diff.x > 0) return (FacingDirection.Right, FacingDirection.Left);
            if (diff.x < 0) return (FacingDirection.Left, FacingDirection.Right);
            if (diff.y > 0) return (FacingDirection.Up, FacingDirection.Down);
            if (diff.y < 0) return (FacingDirection.Down, FacingDirection.Up);

            // For diagonal cases, prioritize horizontal facing
            if (diff.x > 0 && diff.y > 0) return (FacingDirection.Right, FacingDirection.Left);
            if (diff.x < 0 && diff.y > 0) return (FacingDirection.Left, FacingDirection.Right);
            if (diff.x > 0 && diff.y < 0) return (FacingDirection.Right, FacingDirection.Left);
            if (diff.x < 0 && diff.y < 0) return (FacingDirection.Left, FacingDirection.Right);

            return (FacingDirection.Right, FacingDirection.Left); // Default
        }

        private SpawnedMine CreateMine(
            SpawnContext context,
            Vector2Int position,
            MineTypeSpawnData spawnData,
            FacingDirection facing)
        {
            var mine = context.MineFactory.CreateMine(spawnData.MineData, position);
            return SpawnedMine.Create(
                position,
                mine,
                spawnData.MineData,
                facing
            );
        }

        private bool IsValidPosition(Vector2Int pos, SpawnContext context)
        {
            return pos.x >= 0 && pos.x < context.GridWidth &&
                   pos.y >= 0 && pos.y < context.GridHeight &&
                   !context.ExistingMines.ContainsKey(pos);
        }

        private float GetDistance(Vector2Int a, Vector2Int b)
        {
            return m_AllowDiagonal
                ? Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y))  // Chebyshev distance
                : Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);           // Manhattan distance
        }
    }
}

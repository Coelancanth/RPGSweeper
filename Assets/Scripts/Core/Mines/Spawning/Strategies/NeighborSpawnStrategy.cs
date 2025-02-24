using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPGMinesweeper.Core.Mines.Spawning
{
    public class NeighborRelationship
    {
        public MineData PrimaryMineType { get; }
        public List<MineData> ValidNeighborTypes { get; }
        public Dictionary<MineData, FacingDirection> FacingDirections { get; }

        public NeighborRelationship(
            MineData primaryMineType,
            List<MineData> validNeighborTypes,
            Dictionary<MineData, FacingDirection> facingDirections)
        {
            PrimaryMineType = primaryMineType;
            ValidNeighborTypes = validNeighborTypes;
            FacingDirections = facingDirections;
        }
    }

    public class NeighborSpawnStrategy : MineSpawnStrategyBase
    {
        private readonly int m_MaxDistance;
        private readonly bool m_AllowDiagonal;
        private readonly int m_MinClusterSize;
        private readonly int m_MaxClusterSize;

        public override SpawnStrategyType Priority => SpawnStrategyType.Neighbor;

        public NeighborSpawnStrategy(MineTypeSpawnData spawnData)
        {
            m_MaxDistance = spawnData.MaxDistance;
            m_AllowDiagonal = spawnData.AllowDiagonal;
            m_MinClusterSize = spawnData.MinClusterSize;
            m_MaxClusterSize = spawnData.MaxClusterSize;
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            if (spawnData.NeighborTypes == null || spawnData.NeighborTypes.Count == 0)
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
                var clusterSize = Mathf.Min(
                    Random.Range(m_MinClusterSize, m_MaxClusterSize + 1),
                    remainingCount
                );

                var clusterMines = PlaceCluster(context, spawnData, availablePositions, clusterSize);
                if (clusterMines.Count > 0)
                {
                    mines.AddRange(clusterMines);
                    remainingCount -= clusterMines.Count;

                    foreach (var mine in clusterMines)
                    {
                        availablePositions.RemoveAll(p => 
                            GetDistance(p, mine.Position) <= m_MaxDistance);
                    }
                }
                else
                {
                    break;
                }
            }

            return mines.Count > 0 
                ? SpawnResult.Successful(mines) 
                : SpawnResult.Failed("Could not place any neighbor mines");
        }

        private List<SpawnedMine> PlaceCluster(
            SpawnContext context,
            MineTypeSpawnData spawnData,
            List<Vector2Int> availablePositions,
            int clusterSize)
        {
            var clusterMines = new List<SpawnedMine>();
            var usedPositions = new HashSet<Vector2Int>();
            
            // Place primary mine
            var startPos = availablePositions[Random.Range(0, availablePositions.Count)];
            var primaryMine = CreateMine(context, startPos, spawnData.MineData, FacingDirection.Right);
            clusterMines.Add(primaryMine);
            usedPositions.Add(startPos);
            availablePositions.Remove(startPos);

            var remainingSize = clusterSize - 1;
            var lastAddedPos = startPos;

            while (remainingSize > 0)
            {
                var adjacentPos = GetValidAdjacentPosition(lastAddedPos, availablePositions, context);
                
                if (!adjacentPos.HasValue)
                {
                    bool foundValidPosition = false;
                    foreach (var existingPos in usedPositions)
                    {
                        adjacentPos = GetValidAdjacentPosition(existingPos, availablePositions, context);
                        if (adjacentPos.HasValue)
                        {
                            lastAddedPos = existingPos;
                            foundValidPosition = true;
                            break;
                        }
                    }
                    
                    if (!foundValidPosition)
                        break;
                }

                var newPos = adjacentPos.Value;
                
                // Select random neighbor type
                var neighborSetting = spawnData.NeighborTypes[Random.Range(0, spawnData.NeighborTypes.Count)];
                var newMine = CreateMine(context, newPos, neighborSetting.NeighborMineType, neighborSetting.FacingDirection);
                clusterMines.Add(newMine);
                usedPositions.Add(newPos);
                availablePositions.Remove(newPos);
                
                lastAddedPos = newPos;
                remainingSize--;
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
            var offsets = new List<Vector2Int>
            {
                Vector2Int.right,
                Vector2Int.left,
                Vector2Int.up,
                Vector2Int.down
            };

            if (m_AllowDiagonal)
            {
                offsets.AddRange(new[]
                {
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, 1),
                    new Vector2Int(1, -1),
                    new Vector2Int(-1, -1)
                });
            }

            return offsets;
        }

        private SpawnedMine CreateMine(
            SpawnContext context,
            Vector2Int position,
            MineData mineData,
            FacingDirection facing)
        {
            var mine = context.MineFactory.CreateMine(mineData, position);
            return SpawnedMine.Create(
                position,
                mine,
                mineData,
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
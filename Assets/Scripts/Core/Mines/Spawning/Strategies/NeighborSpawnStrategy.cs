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
        private readonly NeighborRelationship m_Relationship;

        public override SpawnStrategyType Priority => SpawnStrategyType.Neighbor;

        public NeighborSpawnStrategy(MineTypeSpawnData spawnData)
        {
            m_MaxDistance = spawnData.MaxDistance;
            m_AllowDiagonal = spawnData.AllowDiagonal;
            m_MinClusterSize = spawnData.MinClusterSize;
            m_MaxClusterSize = spawnData.MaxClusterSize;

            // Create relationship from spawn data
            var validNeighborTypes = spawnData.NeighborTypes
                .Select(nt => nt.NeighborMineData)
                .ToList();

            var facingDirections = spawnData.NeighborTypes
                .ToDictionary(
                    nt => nt.NeighborMineData,
                    nt => nt.FacingDirection
                );

            m_Relationship = new NeighborRelationship(
                spawnData.MineData,
                validNeighborTypes,
                facingDirections
            );
        }

        public override bool CanExecute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!base.CanExecute(context, spawnData))
            {
                return false;
            }

            if (m_Relationship == null || !m_Relationship.ValidNeighborTypes.Any())
            {
                return false;
            }

            var availablePositions = context.GetAvailablePositions().ToList();
            // Each primary mine needs enough space for its neighbors
            var totalNeighborsPerPrimary = m_Relationship.ValidNeighborTypes.Count;
            var spaceNeededPerPrimary = 1 + totalNeighborsPerPrimary;
            return availablePositions.Count >= spaceNeededPerPrimary * spawnData.SpawnCount;
        }

        public override SpawnResult Execute(SpawnContext context, MineTypeSpawnData spawnData)
        {
            if (!ValidateSpawnData(context, spawnData))
            {
                return SpawnResult.Failed("Invalid spawn data");
            }

            var mines = new List<SpawnedMine>();
            var availablePositions = context.GetAvailablePositions().ToList();
            var remainingPrimaryMines = spawnData.SpawnCount;

            while (remainingPrimaryMines > 0 && availablePositions.Count > 0)
            {
                var clusterMines = PlaceCluster(context, spawnData, availablePositions);
                if (clusterMines.Count > 0)
                {
                    mines.AddRange(clusterMines);
                    remainingPrimaryMines--;

                    // Only remove positions around the primary mine
                    var primaryMine = clusterMines[0];
                    availablePositions.RemoveAll(p => 
                        GetDistance(p, primaryMine.Position) <= m_MaxDistance);
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
            List<Vector2Int> availablePositions)
        {
            var clusterMines = new List<SpawnedMine>();
            
            // Place primary mine
            var startPos = availablePositions[Random.Range(0, availablePositions.Count)];
            Debug.Log($"Placing primary mine of type {m_Relationship.PrimaryMineType.name} at position {startPos}");
            var primaryMine = CreateMine(context, startPos, m_Relationship.PrimaryMineType, FacingDirection.Right);
            clusterMines.Add(primaryMine);
            availablePositions.Remove(startPos);

            // Track remaining neighbor types to place
            var remainingNeighborTypes = new List<MineData>(m_Relationship.ValidNeighborTypes);
            Debug.Log($"Setting up neighbors for primary mine {m_Relationship.PrimaryMineType.name}:");
            foreach (var neighborType in m_Relationship.ValidNeighborTypes)
            {
                var facing = m_Relationship.FacingDirections[neighborType];
                Debug.Log($"  - Will spawn {neighborType.name} facing {facing}");
            }

            // Get all positions within range of primary mine
            var positionsInRange = GetPositionsInRange(startPos, m_MaxDistance, context)
                .Where(p => availablePositions.Contains(p))
                .ToList();

            // Place neighbor mines within range of primary mine
            while (positionsInRange.Count > 0 && remainingNeighborTypes.Any())
            {
                var pos = positionsInRange[Random.Range(0, positionsInRange.Count)];
                
                // Select random neighbor type from remaining types
                var neighborMineData = remainingNeighborTypes[Random.Range(0, remainingNeighborTypes.Count)];
                remainingNeighborTypes.Remove(neighborMineData);

                Debug.Log($"Selected neighbor type: {neighborMineData.name} at position {pos}, facing {m_Relationship.FacingDirections[neighborMineData]}");

                var newMine = CreateMine(context, pos, neighborMineData, m_Relationship.FacingDirections[neighborMineData]);
                clusterMines.Add(newMine);
                availablePositions.Remove(pos);
                positionsInRange.Remove(pos);
            }

            return clusterMines;
        }

        private List<Vector2Int> GetPositionsInRange(Vector2Int center, int range, SpawnContext context)
        {
            var positions = new List<Vector2Int>();
            
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    var pos = new Vector2Int(center.x + x, center.y + y);
                    if (pos != center && IsValidPosition(pos, context) && GetDistance(center, pos) <= range)
                    {
                        positions.Add(pos);
                    }
                }
            }
            
            return positions;
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
    }
} 
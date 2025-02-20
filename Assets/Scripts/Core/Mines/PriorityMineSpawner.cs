using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper.Factory;

namespace RPGMinesweeper
{
    public class PriorityMineSpawner : IMineSpawner
    {
        private readonly Dictionary<MineSpawnStrategyType, IMineSpawnStrategy> m_StrategyMap;
        private readonly List<MineTypeSpawnData> m_SpawnData;
        private static readonly Vector2Int INVALID_POSITION = Vector2Int.one * -1;

        public PriorityMineSpawner(List<MineTypeSpawnData> spawnData)
        {
            m_SpawnData = spawnData;
            m_StrategyMap = new Dictionary<MineSpawnStrategyType, IMineSpawnStrategy>
            {
                { MineSpawnStrategyType.Random, new RandomMineSpawnStrategy() },
                { MineSpawnStrategyType.Edge, new EdgeMineSpawnStrategy() },
                { MineSpawnStrategyType.Corner, new CornerMineSpawnStrategy() },
                { MineSpawnStrategyType.Center, new CenterMineSpawnStrategy() }
            };
        }

        void IMineSpawner.ValidateSpawnCounts(List<MineTypeSpawnData> spawnData, int gridWidth, int gridHeight)
        {
            ValidateSpawnCounts(spawnData, gridWidth, gridHeight);
        }

        void IMineSpawner.PlaceMines(List<MineTypeSpawnData> spawnData, IMineFactory mineFactory, GridManager gridManager,
            Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
        {
            PlaceMines(spawnData, mineFactory, gridManager, mines, mineDataMap);
        }

        public void ValidateSpawnCounts(List<MineTypeSpawnData> spawnData, int gridWidth, int gridHeight)
        {
            if (spawnData == null || spawnData.Count == 0)
            {
                Debug.LogError("PriorityMineSpawner: No mine spawn data configured!");
                return;
            }

            int totalMines = spawnData
                .Where(data => data.IsEnabled)
                .Sum(data => data.SpawnCount);

            int maxPossibleMines = gridWidth * gridHeight;

            if (totalMines > maxPossibleMines)
            {
                Debug.LogWarning($"PriorityMineSpawner: Total mine count ({totalMines}) exceeds grid capacity ({maxPossibleMines}). Mines will be scaled down proportionally.");
                float scale = (float)maxPossibleMines / totalMines;
                foreach (var data in spawnData.Where(d => d.IsEnabled))
                {
                    data.SpawnCount = Mathf.Max(1, Mathf.FloorToInt(data.SpawnCount * scale));
                }
            }
        }

        public void PlaceMines(List<MineTypeSpawnData> spawnData, IMineFactory mineFactory, GridManager gridManager, 
            Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
        {
            // Sort spawn data by strategy priority
            var orderedSpawnData = spawnData
                .Where(data => data.IsEnabled)
                .OrderBy(data => GetStrategyPriority(data.MineData.SpawnStrategy))
                .ToList();

            foreach (var data in orderedSpawnData)
            {
                PlaceMinesForData(data, mineFactory, gridManager, mines, mineDataMap);
            }

            // Handle any remaining unplaced mines with random strategy as fallback
            var unplacedMines = spawnData
                .Where(data => data.IsEnabled && GetRemainingCount(data, mines) > 0)
                .ToList();

            if (unplacedMines.Any())
            {
                Debug.LogWarning("Some mines could not be placed with their preferred strategy. Falling back to random placement.");
                foreach (var data in unplacedMines)
                {
                    PlaceMinesWithFallback(data, mineFactory, gridManager, mines, mineDataMap);
                }
            }
        }

        private void PlaceMinesForData(MineTypeSpawnData data, IMineFactory mineFactory, GridManager gridManager,
            Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
        {
            var strategy = GetOrCreateStrategy(data.MineData);
            int remainingCount = GetRemainingCount(data, mines);
            int consecutiveFailures = 0;
            const int maxConsecutiveFailures = 3;

            while (remainingCount > 0 && consecutiveFailures < maxConsecutiveFailures)
            {
                var position = strategy.GetSpawnPosition(gridManager, mines);
                
                if (IsInvalidPosition(position))
                {
                    consecutiveFailures++;
                    continue;
                }

                if (mines.ContainsKey(position))
                {
                    // Position is already occupied, try again
                    consecutiveFailures++;
                    continue;
                }

                try
                {
                    IMine mine = mineFactory.CreateMine(data.MineData, position);
                    mines.Add(position, mine);
                    mineDataMap.Add(position, data.MineData);
                    remainingCount--;
                    consecutiveFailures = 0;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to create mine at position {position}: {e.Message}");
                    consecutiveFailures++;
                }
            }

            if (remainingCount > 0)
            {
                Debug.LogWarning($"Failed to place {remainingCount} mines of type {data.MineData.Type} using {strategy.GetType().Name}");
            }
        }

        private void PlaceMinesWithFallback(MineTypeSpawnData data, IMineFactory mineFactory, GridManager gridManager,
            Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
        {
            var randomStrategy = new RandomMineSpawnStrategy();
            int remainingCount = GetRemainingCount(data, mines);
            int consecutiveFailures = 0;
            const int maxConsecutiveFailures = 10;

            while (remainingCount > 0 && consecutiveFailures < maxConsecutiveFailures)
            {
                var position = randomStrategy.GetSpawnPosition(gridManager, mines);
                
                if (IsInvalidPosition(position))
                {
                    consecutiveFailures++;
                    continue;
                }

                if (mines.ContainsKey(position))
                {
                    consecutiveFailures++;
                    continue;
                }

                try
                {
                    IMine mine = mineFactory.CreateMine(data.MineData, position);
                    mines.Add(position, mine);
                    mineDataMap.Add(position, data.MineData);
                    remainingCount--;
                    consecutiveFailures = 0;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to create mine at position {position}: {e.Message}");
                    consecutiveFailures++;
                }
            }

            if (remainingCount > 0)
            {
                Debug.LogError($"Failed to place {remainingCount} mines of type {data.MineData.Type} even with random fallback.");
            }
        }

        private IMineSpawnStrategy GetOrCreateStrategy(MineData mineData)
        {
            if (mineData.SpawnStrategy == MineSpawnStrategyType.Surrounded)
            {
                return new SurroundedMineSpawnStrategy(mineData.TargetMineType, mineData.TargetMonsterType);
            }

            if (m_StrategyMap.TryGetValue(mineData.SpawnStrategy, out var strategy))
            {
                return strategy;
            }

            Debug.LogWarning($"Unknown spawn strategy type: {mineData.SpawnStrategy}. Falling back to random strategy.");
            return new RandomMineSpawnStrategy();
        }

        private int GetRemainingCount(MineTypeSpawnData data, Dictionary<Vector2Int, IMine> existingMines)
        {
            int placedCount = existingMines.Values.Count(mine => mine.Type == data.MineData.Type);
            return data.SpawnCount - placedCount;
        }

        private SpawnStrategyPriority GetStrategyPriority(MineSpawnStrategyType strategyType)
        {
            var strategy = GetOrCreateStrategy(new MineData { SpawnStrategy = strategyType });
            return strategy.Priority;
        }

        private bool IsInvalidPosition(Vector2Int position)
        {
            return position == INVALID_POSITION;
        }
    }
} 
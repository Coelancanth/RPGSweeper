using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper;
using RPGMinesweeper.Factory;

public interface IMineSpawner
{
    void ValidateSpawnCounts(List<MineTypeSpawnData> spawnData, int gridWidth, int gridHeight);
    void PlaceMines(List<MineTypeSpawnData> spawnData, IMineFactory mineFactory, GridManager gridManager, 
        Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap);
}

public class MineSpawner : IMineSpawner
{
    private Dictionary<MineSpawnStrategyType, CompositeSpawnStrategy> m_SpawnStrategies;
    private static readonly Vector2Int INVALID_POSITION = Vector2Int.one * -1;

    public MineSpawner()
    {
        m_SpawnStrategies = new Dictionary<MineSpawnStrategyType, CompositeSpawnStrategy>();
    }

    public void ValidateSpawnCounts(List<MineTypeSpawnData> spawnData, int gridWidth, int gridHeight)
    {
        if (spawnData == null || spawnData.Count == 0)
        {
            Debug.LogError("MineSpawner: No mine spawn data configured!");
            return;
        }

        int totalMines = spawnData
            .Where(data => data.IsEnabled)
            .Sum(data => data.SpawnCount);
            
        int maxPossibleMines = gridWidth * gridHeight;

        if (totalMines > maxPossibleMines)
        {
            Debug.LogWarning($"MineSpawner: Total mine count ({totalMines}) exceeds grid capacity ({maxPossibleMines}). Mines will be scaled down proportionally.");
            float scale = (float)maxPossibleMines / totalMines;
            foreach (var data in spawnData.Where(d => d.IsEnabled))
            {
                data.SpawnCount = Mathf.Max(1, Mathf.FloorToInt(data.SpawnCount * scale));
            }
        }
    }

    public void PlaceMines(List<MineTypeSpawnData> mineSpawnData, IMineFactory mineFactory, GridManager gridManager, 
        Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
    {
        // First pass: Place all non-surrounded mines
        foreach (var data in mineSpawnData.Where(data => data.IsEnabled && data.MineData.SpawnStrategy != MineSpawnStrategyType.Surrounded))
        {
            PlaceMinesForData(data, mineFactory, gridManager, mines, mineDataMap);
        }

        // Second pass: Place all surrounded mines
        foreach (var data in mineSpawnData.Where(data => data.IsEnabled && data.MineData.SpawnStrategy == MineSpawnStrategyType.Surrounded))
        {
            PlaceMinesForData(data, mineFactory, gridManager, mines, mineDataMap);
        }
    }

    private void PlaceMinesForData(MineTypeSpawnData data, IMineFactory mineFactory, GridManager gridManager,
        Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
    {
        int remainingCount = data.SpawnCount;
        int consecutiveFailures = 0;
        const int maxConsecutiveFailures = 3;

        bool isSymmetricStrategy = data.MineData.SpawnStrategy == MineSpawnStrategyType.SymmetricHorizontal || 
                                 data.MineData.SpawnStrategy == MineSpawnStrategyType.SymmetricVertical;

        // For symmetric strategies, we need to ensure we spawn in pairs
        if (isSymmetricStrategy && remainingCount % 2 != 0)
        {
            remainingCount++;
            Debug.LogWarning($"Adjusted spawn count to be even for symmetric spawning: {remainingCount}");
        }

        while (remainingCount > 0 && consecutiveFailures < maxConsecutiveFailures)
        {
            Vector2Int position = GetSpawnPosition(data.MineData, gridManager, mines);

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
                // Place the first mine
                PlaceMineAtPosition(position, data, mineFactory, gridManager, mines, mineDataMap);
                remainingCount--;
                consecutiveFailures = 0;

                // For symmetric strategies, immediately get and place the symmetric mine
                if (isSymmetricStrategy && remainingCount > 0)
                {
                    Vector2Int symmetricPosition = GetSpawnPosition(data.MineData, gridManager, mines);
                    if (!IsInvalidPosition(symmetricPosition) && !mines.ContainsKey(symmetricPosition))
                    {
                        PlaceMineAtPosition(symmetricPosition, data, mineFactory, gridManager, mines, mineDataMap);
                        remainingCount--;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to place symmetric mine at position {symmetricPosition}");
                        consecutiveFailures++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create mine at position {position}: {e.Message}");
                consecutiveFailures++;
            }
        }

        if (remainingCount > 0)
        {
            Debug.LogWarning($"Failed to place {remainingCount} mines of type {data.MineData.Type}. Falling back to random placement.");
            PlaceMinesWithFallback(data, remainingCount, mineFactory, gridManager, mines, mineDataMap);
        }
    }

    private void PlaceMineAtPosition(Vector2Int position, MineTypeSpawnData data, IMineFactory mineFactory, 
        GridManager gridManager, Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
    {
        IMine mine = mineFactory.CreateMine(data.MineData, position);
        mines.Add(position, mine);
        mineDataMap.Add(position, data.MineData);

        // Set the raw value on the cell view with its color
        var cellObject = gridManager.GetCellObject(position);
        if (cellObject != null)
        {
            var cellView = cellObject.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.SetValue(data.MineData.Value, data.MineData.ValueColor);
            }
        }
    }

    private void PlaceMinesWithFallback(MineTypeSpawnData data, int count, IMineFactory mineFactory, GridManager gridManager,
        Dictionary<Vector2Int, IMine> mines, Dictionary<Vector2Int, MineData> mineDataMap)
    {
        var randomStrategy = new CompositeSpawnStrategy(MineSpawnStrategyType.Random);
        int remainingCount = count;
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

                // Set the raw value on the cell view with its color
                var cellObject = gridManager.GetCellObject(position);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        cellView.SetValue(data.MineData.Value, data.MineData.ValueColor);
                    }
                }

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

    private Vector2Int GetSpawnPosition(MineData mineData, GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
    {
        var strategy = GetOrCreateStrategy(mineData.SpawnStrategy, mineData);
        return strategy.GetSpawnPosition(gridManager, existingMines);
    }

    private CompositeSpawnStrategy GetOrCreateStrategy(MineSpawnStrategyType strategy, MineData mineData)
    {
        if (!m_SpawnStrategies.TryGetValue(strategy, out var spawnStrategy))
        {
            if (strategy == MineSpawnStrategyType.SymmetricHorizontal || strategy == MineSpawnStrategyType.SymmetricVertical)
            {
                var direction = strategy == MineSpawnStrategyType.SymmetricHorizontal 
                    ? SymmetryDirection.Horizontal 
                    : SymmetryDirection.Vertical;
                    
                // Create a new composite strategy with just the symmetric strategy
                spawnStrategy = new CompositeSpawnStrategy(strategy);
                m_SpawnStrategies[strategy] = spawnStrategy;
            }
            else
            {
                MineType targetType = strategy == MineSpawnStrategyType.Surrounded 
                    ? mineData.TargetMineType  // Use the configured target type
                    : MineType.Standard;

                MonsterType? targetMonsterType = (strategy == MineSpawnStrategyType.Surrounded && 
                                                mineData.TargetMineType == MineType.Monster)
                    ? mineData.TargetMonsterType
                    : null;
                    
                spawnStrategy = new CompositeSpawnStrategy(strategy, targetType, targetMonsterType);
                m_SpawnStrategies[strategy] = spawnStrategy;
            }
        }
        return spawnStrategy;
    }

    private bool IsInvalidPosition(Vector2Int position)
    {
        return position == INVALID_POSITION;
    }
} 
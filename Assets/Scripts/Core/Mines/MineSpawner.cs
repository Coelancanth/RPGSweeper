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
        foreach (var data in mineSpawnData.Where(data => data.IsEnabled))
        {
            for (int i = 0; i < data.SpawnCount; i++)
            {
                Vector2Int position = GetSpawnPosition(data.MineData, gridManager, mines);
                
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
        }
    }

    private Vector2Int GetSpawnPosition(MineData mineData, GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
    {
        var strategy = GetOrCreateStrategy(mineData.SpawnStrategy);
        return strategy.GetSpawnPosition(gridManager, existingMines);
    }

    private CompositeSpawnStrategy GetOrCreateStrategy(MineSpawnStrategyType strategy)
    {
        if (!m_SpawnStrategies.TryGetValue(strategy, out var spawnStrategy))
        {
            spawnStrategy = new CompositeSpawnStrategy(strategy);
            m_SpawnStrategies[strategy] = spawnStrategy;
        }
        return spawnStrategy;
    }
} 
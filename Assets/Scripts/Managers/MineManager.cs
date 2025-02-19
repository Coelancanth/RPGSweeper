using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper;  // For MonsterType and CompositeSpawnStrategy
using RPGMinesweeper.Grid;  // For GridShape
using RPGMinesweeper.Factory;  // For MineFactory

[System.Serializable]
public class MineTypeSpawnData
{
    public MineData MineData;
    public int SpawnCount;
    [Tooltip("When disabled, this mine type will not be spawned")]
    public bool IsEnabled = true;
}

public class MineManager : MonoBehaviour
{
    [Header("Mine Configuration")]
    [SerializeField] private List<MineTypeSpawnData> m_MineSpawnData;
    [SerializeField] private GridManager m_GridManager;
    
    private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();
    private Dictionary<Vector2Int, MineData> m_MineDataMap = new Dictionary<Vector2Int, MineData>();
    private Dictionary<MineSpawnStrategyType, CompositeSpawnStrategy> m_SpawnStrategies;
    private IMineFactory m_MineFactory;

    private void Awake()
    {
        InitializeSpawnStrategies();
        m_MineFactory = new MineFactory();
    }

    private void InitializeSpawnStrategies()
    {
        m_SpawnStrategies = new Dictionary<MineSpawnStrategyType, CompositeSpawnStrategy>();
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

    private void Start()
    {
        if (m_GridManager == null)
        {
            m_GridManager = FindFirstObjectByType<GridManager>();
            if (m_GridManager == null)
            {
                Debug.LogError("MineManager: Could not find GridManager!");
                enabled = false;
                return;
            }
        }

        ValidateSpawnCounts();
        PlaceMines();
        SubscribeToEvents();
    }

    private void ValidateSpawnCounts()
    {
        if (m_MineSpawnData == null || m_MineSpawnData.Count == 0)
        {
            Debug.LogError("MineManager: No mine spawn data configured!");
            enabled = false;
            return;
        }

        int totalMines = m_MineSpawnData
            .Where(data => data.IsEnabled)
            .Sum(data => data.SpawnCount);
            
        int maxPossibleMines = m_GridManager.Width * m_GridManager.Height;

        if (totalMines > maxPossibleMines)
        {
            Debug.LogWarning($"MineManager: Total mine count ({totalMines}) exceeds grid capacity ({maxPossibleMines}). Mines will be scaled down proportionally.");
            float scale = (float)maxPossibleMines / totalMines;
            foreach (var data in m_MineSpawnData.Where(d => d.IsEnabled))
            {
                data.SpawnCount = Mathf.Max(1, Mathf.FloorToInt(data.SpawnCount * scale));
            }
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnCellRevealed += HandleCellRevealed;
        GameEvents.OnMineRemovalAttempted += HandleMineRemoval;
        GameEvents.OnMineAddAttempted += HandleMineAdd;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnCellRevealed -= HandleCellRevealed;
        GameEvents.OnMineRemovalAttempted -= HandleMineRemoval;
        GameEvents.OnMineAddAttempted -= HandleMineAdd;
    }

    private void HandleCellRevealed(Vector2Int position)
    {
        if (m_Mines.TryGetValue(position, out IMine mine))
        {
            // Show mine sprite
            if (m_MineDataMap.TryGetValue(position, out MineData mineData))
            {
                var cellObject = m_GridManager.GetCellObject(position);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        // First show the mine sprite (this sets the HasMine flag)
                        cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                        // Then reveal the cell (this will use the correct state based on HasMine)
                        cellView.UpdateVisuals(true);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"MineManager: No MineData found for position {position}");
            }

            // Trigger mine effect
            var playerComponent = FindFirstObjectByType<PlayerComponent>();
            if (playerComponent != null)
            {
                mine.OnTrigger(playerComponent);
            }
        }
    }

    private void HandleMineRemoval(Vector2Int position)
    {
        if (m_Mines.TryGetValue(position, out IMine mine))
        {
            // Trigger mine's destroy effects first
            mine.OnDestroy();

            // Remove the mine from both dictionaries
            m_Mines.Remove(position);
            m_MineDataMap.Remove(position);

            // Get the cell view and handle the visual update
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject != null)
            {
                var cellView = cellObject.GetComponent<CellView>();
                if (cellView != null)
                {
                    cellView.HandleMineRemoval();
                }
            }

            // Update all grid values
            UpdateAllGridValues();
        }
    }

    private MineData FindMineDataByMonsterType(MonsterType monsterType)
    {
        return m_MineSpawnData
            .Where(data => data.IsEnabled)
            .Select(data => data.MineData)
            .OfType<MonsterMineData>()
            .FirstOrDefault(data => data.MonsterType == monsterType);
    }

    private void HandleMineAdd(Vector2Int position, MineType type, MonsterType? monsterType = null)
    {
        // Check if the position is valid and empty
        if (!m_GridManager.IsValidPosition(position))
        {
            Debug.LogError($"MineManager: Attempted to add mine at invalid position {position}");
            return;
        }

        if (m_Mines.ContainsKey(position))
        {
            Debug.LogError($"MineManager: Attempted to add mine at occupied position {position}");
            return;
        }

        // Find the corresponding MineData
        MineData mineData;
        if (type == MineType.Monster && monsterType.HasValue)
        {
            mineData = FindMineDataByMonsterType(monsterType.Value);
        }
        else
        {
            mineData = m_MineSpawnData
                .Where(data => data.IsEnabled && data.MineData.Type == type)
                .FirstOrDefault()?.MineData;
        }

        if (mineData == null)
        {
            string errorDetails = type == MineType.Monster && monsterType.HasValue 
                ? $"type {type} and monster type {monsterType.Value}" 
                : $"type {type}";
            Debug.LogError($"MineManager: No MineData found for {errorDetails}");
            return;
        }

        // Create and add the new mine using MineFactory
        IMine mine = m_MineFactory.CreateMine(mineData, position);
        m_Mines[position] = mine;
        m_MineDataMap[position] = mineData;

        // Get the cell view for visual updates
        var cellObject = m_GridManager.GetCellObject(position);
        if (cellObject != null)
        {
            var cellView = cellObject.GetComponent<CellView>();
            if (cellView != null)
            {
                // Set the raw value and color
                cellView.SetValue(mineData.Value, mineData.ValueColor);

                // If the cell is already revealed, show the mine sprite immediately
                if (cellView.IsRevealed)
                {
                    cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                    cellView.UpdateVisuals(true);
                }
            }
        }

        // Update all grid values after adding the mine
        UpdateAllGridValues();
    }

    private void PlaceMines()
    {
        foreach (var spawnData in m_MineSpawnData.Where(data => data.IsEnabled))
        {
            for (int i = 0; i < spawnData.SpawnCount; i++)
            {
                Vector2Int position = GetSpawnPosition(spawnData.MineData);
                
                IMine mine = m_MineFactory.CreateMine(spawnData.MineData, position);
                m_Mines.Add(position, mine);
                m_MineDataMap.Add(position, spawnData.MineData);

                // Set the raw value on the cell view with its color
                var cellObject = m_GridManager.GetCellObject(position);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        cellView.SetValue(spawnData.MineData.Value, spawnData.MineData.ValueColor);
                    }
                }
            }
        }

        // Update all grid values after placing mines
        UpdateAllGridValues();
    }

    private Vector2Int GetSpawnPosition(MineData mineData)
    {
        var strategy = GetOrCreateStrategy(mineData.SpawnStrategy);
        return strategy.GetSpawnPosition(m_GridManager, m_Mines);
    }

    private void UpdateAllGridValues()
    {
        MineValuePropagator.PropagateValues(this, m_GridManager);
    }

    #if UNITY_EDITOR
    public IReadOnlyDictionary<Vector2Int, IMine> GetMines() => m_Mines;
    #endif

    public bool HasMineAt(Vector2Int position) => m_Mines.ContainsKey(position);

    public MineData GetMineDataAt(Vector2Int position)
    {
        if (m_MineDataMap.TryGetValue(position, out MineData mineData))
        {
            return mineData;
        }
        return null;
    }

    /// <summary>
    /// Interface for mine copying strategies
    /// </summary>
    private interface IMineCopyStrategy
    {
        void CopyMineProperties(IMine sourceMine, IMine targetMine);
    }

    /// <summary>
    /// Strategy for copying standard mines
    /// </summary>
    private class StandardMineCopyStrategy : IMineCopyStrategy
    {
        public void CopyMineProperties(IMine sourceMine, IMine targetMine)
        {
            // Standard mines don't have additional properties to copy
        }
    }

    /// <summary>
    /// Strategy for copying monster mines
    /// </summary>
    private class MonsterMineCopyStrategy : IMineCopyStrategy
    {
        public void CopyMineProperties(IMine sourceMine, IMine targetMine)
        {
            if (sourceMine is MonsterMine sourceMonster && targetMine is MonsterMine targetMonster)
            {
                targetMonster.CurrentHp = sourceMonster.CurrentHp;
            }
        }
    }

    /// <summary>
    /// Factory for creating mine copy strategies
    /// </summary>
    private static class MineCopyStrategyFactory
    {
        public static IMineCopyStrategy CreateStrategy(MineType type)
        {
            return type switch
            {
                MineType.Monster => new MonsterMineCopyStrategy(),
                MineType.Standard => new StandardMineCopyStrategy(),
                _ => new StandardMineCopyStrategy() // Default to standard strategy
            };
        }
    }

    /// <summary>
    /// Creates a deep copy of a mine at a new position.
    /// </summary>
    /// <param name="_sourcePosition">The position of the source mine to copy</param>
    /// <param name="_targetPosition">The position where the copy should be placed</param>
    /// <returns>True if the copy was successful, false otherwise</returns>
    public bool CopyMine(Vector2Int _sourcePosition, Vector2Int _targetPosition)
    {
        if (!ValidateCopyPositions(_sourcePosition, _targetPosition)) return false;

        // Get source mine and data
        var sourceMine = m_Mines[_sourcePosition];
        var sourceMineData = m_MineDataMap[_sourcePosition];

        // Create and place the new mine using MineFactory
        IMine newMine = m_MineFactory.CreateMine(sourceMineData, _targetPosition);
        
        // Copy mine-type specific properties
        var copyStrategy = MineCopyStrategyFactory.CreateStrategy(sourceMineData.Type);
        copyStrategy.CopyMineProperties(sourceMine, newMine);

        // Add the new mine to collections
        m_Mines[_targetPosition] = newMine;
        m_MineDataMap[_targetPosition] = sourceMineData;

        // Update visuals
        UpdateCellView(_targetPosition, sourceMineData, newMine);

        // Update grid values
        UpdateAllGridValues();
        return true;
    }

    /// <summary>
    /// Validates positions for mine copying
    /// </summary>
    private bool ValidateCopyPositions(Vector2Int sourcePosition, Vector2Int targetPosition)
    {
        // Validate positions are within grid
        if (!m_GridManager.IsValidPosition(sourcePosition) || !m_GridManager.IsValidPosition(targetPosition))
        {
            Debug.LogError($"MineManager: Invalid position for mine copy. Source: {sourcePosition}, Target: {targetPosition}");
            return false;
        }

        // Check if source has a mine
        if (!m_Mines.ContainsKey(sourcePosition))
        {
            Debug.LogError($"MineManager: No mine found at source position {sourcePosition}");
            return false;
        }

        // Check if target is empty
        if (m_Mines.ContainsKey(targetPosition))
        {
            Debug.LogError($"MineManager: Target position {targetPosition} is already occupied");
            return false;
        }

        // Check if source has mine data
        if (!m_MineDataMap.ContainsKey(sourcePosition))
        {
            Debug.LogError($"MineManager: No mine data found for position {sourcePosition}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the cell view for a copied mine
    /// </summary>
    private void UpdateCellView(Vector2Int position, MineData mineData, IMine mine)
    {
        var cellObject = m_GridManager.GetCellObject(position);
        if (cellObject == null) return;

        var cellView = cellObject.GetComponent<CellView>();
        if (cellView == null) return;

        cellView.SetValue(mineData.Value, mineData.ValueColor);
        
        if (cellView.IsRevealed)
        {
            cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
            cellView.UpdateVisuals(true);
        }
    }
} 
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MineTypeSpawnData
{
    public MineData MineData;
    public int SpawnCount;
}

public class MineManager : MonoBehaviour
{
    [Header("Mine Configuration")]
    [SerializeField] private List<MineTypeSpawnData> m_MineSpawnData;
    [SerializeField] private GridManager m_GridManager;
    
    private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();
    private Dictionary<Vector2Int, MineData> m_MineDataMap = new Dictionary<Vector2Int, MineData>();
    private Dictionary<MineSpawnStrategyType, IMineSpawnStrategy> m_SpawnStrategies;

    private void Awake()
    {
        InitializeSpawnStrategies();
    }

    private void InitializeSpawnStrategies()
    {
        m_SpawnStrategies = new Dictionary<MineSpawnStrategyType, IMineSpawnStrategy>
        {
            { MineSpawnStrategyType.Random, new RandomMineSpawnStrategy() },
            { MineSpawnStrategyType.Edge, new EdgeMineSpawnStrategy() }
        };
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

        int totalMines = m_MineSpawnData.Sum(data => data.SpawnCount);
        int maxPossibleMines = m_GridManager.Width * m_GridManager.Height;

        if (totalMines > maxPossibleMines)
        {
            Debug.LogWarning($"MineManager: Total mine count ({totalMines}) exceeds grid capacity ({maxPossibleMines}). Mines will be scaled down proportionally.");
            float scale = (float)maxPossibleMines / totalMines;
            foreach (var data in m_MineSpawnData)
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
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnCellRevealed -= HandleCellRevealed;
        GameEvents.OnMineRemovalAttempted -= HandleMineRemoval;
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
                        cellView.ShowMineSprite(mineData.MineSprite);
                        // Set the raw value from MineData with its color
                        cellView.SetRawValue(mineData.Value, mineData.MineValueColor);
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

    private void PlaceMines()
    {
        foreach (var spawnData in m_MineSpawnData)
        {
            for (int i = 0; i < spawnData.SpawnCount; i++)
            {
                Vector2Int position = GetSpawnPosition(spawnData.MineData);
                
                IMine mine = CreateMine(spawnData.MineData, position);
                m_Mines.Add(position, mine);
                m_MineDataMap.Add(position, spawnData.MineData);

                // Set the raw value on the cell view with its color
                var cellObject = m_GridManager.GetCellObject(position);
                if (cellObject != null)
                {
                    var cellView = cellObject.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        cellView.SetRawValue(spawnData.MineData.Value, spawnData.MineData.MineValueColor);
                    }
                }
            }
        }

        // Update all grid values after placing mines
        UpdateAllGridValues();
    }

    private Vector2Int GetSpawnPosition(MineData mineData)
    {
        if (m_SpawnStrategies.TryGetValue(mineData.SpawnStrategy, out IMineSpawnStrategy strategy))
        {
            return strategy.GetSpawnPosition(m_GridManager, m_Mines);
        }
        
        Debug.LogWarning($"MineManager: No spawn strategy found for {mineData.SpawnStrategy}, falling back to random strategy");
        return m_SpawnStrategies[MineSpawnStrategyType.Random].GetSpawnPosition(m_GridManager, m_Mines);
    }

    private IMine CreateMine(MineData mineData, Vector2Int position)
    {
        switch (mineData.Type)
        {
            case MineType.Standard:
                return new StandardMine(mineData, position);
            default:
                Debug.LogWarning($"Mine type {mineData.Type} not implemented yet. Using StandardMine as fallback.");
                return new StandardMine(mineData, position);
        }
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
} 
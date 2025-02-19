using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper;  // For MonsterType, CompositeSpawnStrategy, IMineSpawner, and MineSpawner
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
    private IMineFactory m_MineFactory;
    private IMineSpawner m_MineSpawner;

    private void Awake()
    {
        m_MineFactory = new MineFactory();
        m_MineSpawner = new MineSpawner();
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

        m_MineSpawner.ValidateSpawnCounts(m_MineSpawnData, m_GridManager.Width, m_GridManager.Height);
        m_MineSpawner.PlaceMines(m_MineSpawnData, m_MineFactory, m_GridManager, m_Mines, m_MineDataMap);
        MineValuePropagator.PropagateValues(this, m_GridManager);
        SubscribeToEvents();
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
            MineValuePropagator.PropagateValues(this, m_GridManager);
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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper;  // For MonsterType
using RPGMinesweeper.Grid;  // For GridShape
using RPGMinesweeper.Factory;  // For MineFactory
using RPGMinesweeper.Core.Mines;
using RPGMinesweeper.Core.Mines.Spawning;
using Sirenix.OdinInspector;

public class MineManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Mine Configuration")]
    [ListDrawerSettings(
        ShowIndexLabels = false,
        ShowPaging = true,
        ShowItemCount = false,
        Expanded = true,
        DraggableItems = true,
        HideRemoveButton = false,
        HideAddButton = false
    )]
    [PropertySpace(8)]
    [SerializeField] private List<MineTypeSpawnData> m_MineSpawnData;

    [Required]
    [SerializeField] private GridManager m_GridManager;
    #endregion

    #region Private Fields
    private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();
    private Dictionary<Vector2Int, MineData> m_MineDataMap = new Dictionary<Vector2Int, MineData>();
    private IMineFactory m_MineFactory;
    private IMineSpawner m_MineSpawner;
    private IMineVisualManager m_VisualManager;
    private IMineEventHandler m_EventHandler;
    private IMineConfigurationProvider m_ConfigProvider;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (!ValidateComponents()) return;
        InitializeComponents();
    }

    private void Start()
    {
        m_ConfigProvider.ValidateConfiguration(m_GridManager.Width, m_GridManager.Height);
        var config = m_ConfigProvider.GetConfiguration();
        m_MineSpawner.PlaceMines(config.SpawnData, m_MineFactory, m_GridManager, m_Mines, m_MineDataMap);
        
        // Update visuals for all placed mines
        foreach (var kvp in m_Mines)
        {
            if (m_MineDataMap.TryGetValue(kvp.Key, out var mineData))
            {
                m_VisualManager.UpdateCellView(kvp.Key, mineData, kvp.Value);
            }
        }

        MineValuePropagator.PropagateValues(this, m_GridManager);
        m_EventHandler.SubscribeToEvents();
    }

    private void OnDestroy()
    {
        m_EventHandler?.UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    private bool ValidateComponents()
    {
        if (m_GridManager == null)
        {
            m_GridManager = FindFirstObjectByType<GridManager>();
            if (m_GridManager == null)
            {
                Debug.LogError("MineManager: Could not find GridManager!");
                enabled = false;
                return false;
            }
        }
        return true;
    }

    private void InitializeComponents()
    {
        m_MineFactory = new MineFactory();
        m_MineSpawner = new MineSpawner();
        m_ConfigProvider = new MineConfigurationProvider(m_MineSpawnData);
        
        // Initialize visual manager after GridManager is validated
        m_VisualManager = new MineVisualManager(m_GridManager, m_MineSpawnData.ToArray());
        m_EventHandler = new MineEventHandler(this, m_VisualManager);
    }
    #endregion

    #region Public Methods
    public bool HasMineAt(Vector2Int position) => m_Mines.ContainsKey(position);

    public MineData GetMineDataAt(Vector2Int position)
    {
        return m_MineDataMap.TryGetValue(position, out MineData mineData) ? mineData : null;
    }

    public bool TryGetMine(Vector2Int position, out IMine mine, out MineData mineData)
    {
        mine = null;
        mineData = null;
        
        if (!m_Mines.TryGetValue(position, out mine)) return false;
        if (!m_MineDataMap.TryGetValue(position, out mineData)) return false;
        
        return true;
    }

    public void RemoveMineAt(Vector2Int position)
    {
        m_Mines.Remove(position);
        m_MineDataMap.Remove(position);
    }

    public void AddMine(Vector2Int position, MineType type, MonsterType? monsterType = null)
    {
        if (!ValidateAddPosition(position)) return;

        MineData mineData = FindAppropriateMinData(type, monsterType);
        if (mineData == null) return;

        IMine mine = m_MineFactory.CreateMine(mineData, position);
        m_Mines[position] = mine;
        m_MineDataMap[position] = mineData;

        m_VisualManager.UpdateCellView(position, mineData, mine);
        UpdateAllGridValues();
    }

    public void TeleportMine(Vector2Int fromPosition, Vector2Int toPosition)
    {
        if (!ValidateTeleportPositions(fromPosition, toPosition)) return;

        if (!m_Mines.TryGetValue(fromPosition, out var sourceMine) || 
            !m_MineDataMap.TryGetValue(fromPosition, out var sourceMineData))
        {
            Debug.LogWarning($"MineManager: No mine found at position {fromPosition} to teleport");
            return;
        }

        // Create a new mine at the target position with the same data
        IMine newMine = m_MineFactory.CreateMine(sourceMineData, toPosition);
        m_Mines[toPosition] = newMine;
        m_MineDataMap[toPosition] = sourceMineData;

        // Update the visual for the new mine
        m_VisualManager.UpdateCellView(toPosition, sourceMineData, newMine);

        // Trigger the new mine to initialize its effects (including teleport effect)
        var player = GameObject.FindFirstObjectByType<PlayerComponent>();
        if (player != null)
        {
            newMine.OnTrigger(player);
        }

        // Remove the original mine by raising the event (this will handle cleanup properly)
        GameEvents.RaiseMineRemovalAttempted(fromPosition);

        UpdateAllGridValues();
    }

    public void UpdateAllGridValues()
    {
        MineValuePropagator.PropagateValues(this, m_GridManager);
    }

    #if UNITY_EDITOR
    public IReadOnlyDictionary<Vector2Int, IMine> GetMines() => m_Mines;
    #endif
    #endregion

    #region Private Helper Methods
    private bool ValidateAddPosition(Vector2Int position)
    {
        if (!m_GridManager.IsValidPosition(position))
        {
            Debug.LogError($"MineManager: Invalid position for mine add: {position}");
            return false;
        }

        if (m_Mines.ContainsKey(position))
        {
            Debug.LogError($"MineManager: Position {position} is already occupied");
            return false;
        }

        return true;
    }

    private bool ValidateTeleportPositions(Vector2Int fromPosition, Vector2Int toPosition)
    {
        if (!m_GridManager.IsValidPosition(fromPosition))
        {
            Debug.LogError($"MineManager: Invalid source position for teleport: {fromPosition}");
            return false;
        }

        if (!m_GridManager.IsValidPosition(toPosition))
        {
            Debug.LogError($"MineManager: Invalid target position for teleport: {toPosition}");
            return false;
        }

        if (m_Mines.ContainsKey(toPosition))
        {
            Debug.LogError($"MineManager: Target position {toPosition} is already occupied");
            return false;
        }

        return true;
    }

    private MineData FindAppropriateMinData(MineType type, MonsterType? monsterType)
    {
        if (type == MineType.Monster && monsterType.HasValue)
        {
            return FindMineDataByMonsterType(monsterType.Value);
        }
        
        return m_MineSpawnData
            .Where(data => data.IsEnabled && data.MineData.Type == type)
            .FirstOrDefault()?.MineData;
    }

    private MineData FindMineDataByMonsterType(MonsterType monsterType)
    {
        return m_MineSpawnData
            .Where(data => data.IsEnabled)
            .Select(data => data.MineData)
            .OfType<MonsterMineData>()
            .FirstOrDefault(data => data.MonsterType == monsterType);
    }
    #endregion

    public MineVisualManager GetVisualManager() => m_VisualManager as MineVisualManager;
} 
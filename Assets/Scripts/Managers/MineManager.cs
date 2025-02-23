using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RPGMinesweeper;  // For MonsterType, CompositeSpawnStrategy, IMineSpawner, and MineSpawner
using RPGMinesweeper.Grid;  // For GridShape
using RPGMinesweeper.Factory;  // For MineFactory
using RPGMinesweeper.Core.Mines;
using RPGMinesweeper.Core.Mines.Spawning;

[System.Serializable]
public class MineTypeSpawnData
{   
    public string Description;
    public MineData MineData;
    public int SpawnCount;
    [Tooltip("Strategy to use when spawning this mine type")]
    public SpawnStrategyType SpawnStrategy = SpawnStrategyType.Random;
    [Tooltip("When disabled, this mine type will not be spawned")]
    public bool IsEnabled = true;
}

public class MineManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Mine Configuration")]
    [SerializeField] private List<MineTypeSpawnData> m_MineSpawnData;
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
        m_VisualManager = new MineVisualManager(m_GridManager);
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
} 
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MineManager : MonoBehaviour
{
    [SerializeField] private List<MineData> m_MineDatas;
    [SerializeField] private int m_MineCount = 10;
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
            m_GridManager = FindObjectOfType<GridManager>();
            if (m_GridManager == null)
            {
                Debug.LogError("MineManager: Could not find GridManager!");
                enabled = false;
                return;
            }
        }

        PlaceMines();
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
            var playerComponent = FindObjectOfType<PlayerComponent>();
            if (playerComponent != null)
            {
                mine.OnTrigger(playerComponent);
            }
        }
    }

    private void HandleMineRemoval(Vector2Int position)
    {
        if (m_Mines.ContainsKey(position))
        {
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
        for (int i = 0; i < m_MineCount; i++)
        {
            MineData randomMineData = m_MineDatas[Random.Range(0, m_MineDatas.Count)];
            Vector2Int position = GetSpawnPosition(randomMineData);
            
            IMine mine = CreateMine(randomMineData, position);
            m_Mines.Add(position, mine);
            m_MineDataMap.Add(position, randomMineData);
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
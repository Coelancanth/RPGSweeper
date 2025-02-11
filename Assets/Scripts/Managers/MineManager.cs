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
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnCellRevealed -= HandleCellRevealed;
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

    private void PlaceMines()
    {
        for (int i = 0; i < m_MineCount; i++)
        {
            MineData randomMineData = m_MineDatas[Random.Range(0, m_MineDatas.Count)];
            Vector2Int position = GetRandomEmptyPosition();
            
            IMine mine = CreateMine(randomMineData, position);
            m_Mines.Add(position, mine);
            m_MineDataMap.Add(position, randomMineData);
        }

        // Propagate values after all mines are placed
        MineValuePropagator.PropagateValues(this, m_GridManager);
    }

    private Vector2Int GetRandomEmptyPosition()
    {
        Vector2Int position;
        do
        {
            position = new Vector2Int(
                Random.Range(0, m_GridManager.Width),
                Random.Range(0, m_GridManager.Height)
            );
        } while (m_Mines.ContainsKey(position));

        return position;
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
} 
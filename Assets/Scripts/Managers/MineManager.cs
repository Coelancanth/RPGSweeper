using UnityEngine;
using System.Collections.Generic;
using RPGMinesweeper.Core.Mines;

public class MineManager : MonoBehaviour
{
    [Header("Mine Settings")]
    [Tooltip("List of available mine types")]
    [SerializeField] private List<MineData> m_MineDatas;
    
    [Tooltip("Number of mines to place in the grid")]
    [SerializeField] private int m_MineCount = 10;

    [Header("References")]
    [Tooltip("Reference to the GridManager")]
    [SerializeField] private GridManager m_GridManager;
    
    private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();

    #region Public Methods
    public int CalculateCellValue(Vector2Int position)
    {
        // Don't calculate for cells that contain mines
        if (HasMine(position))
            return 0;

        int sum = 0;
        // Check all 8 adjacent cells
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the center cell
                
                Vector2Int checkPos = position + new Vector2Int(x, y);
                if (IsValidPosition(checkPos) && HasMine(checkPos))
                {
                    // Get the mine's value from MineData
                    IMine mine = m_Mines[checkPos];
                    MineData mineData = ((BaseMine)mine).GetMineData();
                    sum += mineData.Value;
                }
            }
        }
        return sum;
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < m_GridManager.Width &&
               position.y >= 0 && position.y < m_GridManager.Height;
    }

    public bool HasMine(Vector2Int position)
    {
        return m_Mines.ContainsKey(position);
    }
    #endregion

    private void Start()
    {
        if (m_GridManager == null)
        {
            m_GridManager = FindObjectOfType<GridManager>();
        }
        
        PlaceMines();
    }

    private void PlaceMines()
    {
        if (m_MineDatas == null || m_MineDatas.Count == 0)
        {
            Debug.LogError("No mine data configured in MineManager!");
            return;
        }

        for (int i = 0; i < m_MineCount; i++)
        {
            MineData randomMineData = m_MineDatas[Random.Range(0, m_MineDatas.Count)];
            Vector2Int position = GetRandomEmptyPosition();
            
            IMine mine = CreateMine(randomMineData, position);
            m_Mines.Add(position, mine);

            // Set mine in CellView
            GameObject cellObject = m_GridManager.GetCellObject(position);
            if (cellObject != null)
            {
                CellView cellView = cellObject.GetComponent<CellView>();
                if (cellView != null)
                {
                    cellView.SetMine(randomMineData);
                }
            }
        }
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
            case MineType.Healing:
                return new HealingMine(mineData, position);
            case MineType.Experience:
                return new ExperienceMine(mineData, position);
            case MineType.MultiTrigger:
                return new MultiTriggerMine(mineData, position);
            case MineType.AreaReveal:
                return new AreaRevealMine(mineData, position);
            default:
                return new StandardMine(mineData, position);
        }
    }
} 
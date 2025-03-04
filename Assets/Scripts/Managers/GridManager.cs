using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int m_Width = 10;
    [SerializeField] private int m_Height = 10;
    [SerializeField] private GameObject m_CellPrefab;
    [SerializeField] private Transform m_GridParent;
    [SerializeField] private float m_CellSize = 1f;
    
    private Grid m_Grid;
    private GameObject[,] m_CellObjects;

    public float CellSize => m_CellSize;
    public int Width => m_Width;
    public int Height => m_Height;

    private void Awake()
    {
        InitializeGrid();
        CreateVisualGrid();
        SubscribeToEvents();
        CenterGrid();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeGrid()
    {
        m_Grid = new Grid(m_Width, m_Height);
        m_CellObjects = new GameObject[m_Width, m_Height];
    }

    private void CreateVisualGrid()
    {
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                Vector3 position = new Vector3(
                    x * m_CellSize, 
                    y * m_CellSize, 
                    0
                );
                GameObject cellObject = Instantiate(m_CellPrefab, position, Quaternion.identity, m_GridParent);
                cellObject.name = $"Cell_{x}_{y}";
                
                cellObject.transform.localScale = new Vector3(m_CellSize, m_CellSize, 1f);
                
                CellView cellView = cellObject.GetComponent<CellView>();
                cellView.Initialize(new Vector2Int(x, y));
                
                m_CellObjects[x, y] = cellObject;
            }
        }
    }

    private void CenterGrid()
    {
        float gridWidth = m_Width * m_CellSize;
        float gridHeight = m_Height * m_CellSize;
        
        Vector3 centerOffset = new Vector3(
            -gridWidth / 2f + m_CellSize / 2f,
            -gridHeight / 2f + m_CellSize / 2f,
            0f
        );
        
        m_GridParent.position = centerOffset;
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnCellRevealed += HandleCellRevealed;
        GameEvents.OnEffectApplied += HandleEffectApplied;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnCellRevealed -= HandleCellRevealed;
        GameEvents.OnEffectApplied -= HandleEffectApplied;
    }

    private void HandleCellRevealed(Vector2Int position)
    {
        if (m_Grid.IsValidPosition(position))
        {
            CellView cellView = m_CellObjects[position.x, position.y].GetComponent<CellView>();
            cellView.UpdateVisuals(true);
        }
    }

    private void HandleEffectApplied(Vector2Int position)
    {
        if (m_Grid.IsValidPosition(position))
        {
            CellView cellView = m_CellObjects[position.x, position.y].GetComponent<CellView>();
            cellView.ApplyEffect();
        }
    }

    public GameObject GetCellObject(Vector2Int position)
    {
        if (!m_Grid.IsValidPosition(position))
            return null;
            
        return m_CellObjects[position.x, position.y];
    }
} 
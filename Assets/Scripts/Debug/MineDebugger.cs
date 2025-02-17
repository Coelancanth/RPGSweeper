#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public class MineDebugger : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private MineManager m_MineManager;
    [SerializeField] private KeyCode m_DebugKey = KeyCode.D;
    [SerializeField] private KeyCode m_InspectKey = KeyCode.I;  // New key for cell inspection
    [SerializeField] private KeyCode m_RevealAllKey = KeyCode.R;  // New key for revealing all cells
    [SerializeField] private GridManager m_GridManager;
    #endregion

    #region Private Fields
    private Dictionary<Vector2Int, CellView> m_CellViews = new Dictionary<Vector2Int, CellView>();
    private bool m_IsDebugMode = false;
    private Dictionary<Vector2Int, int> m_CachedValues = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, Color> m_CachedColors = new Dictionary<Vector2Int, Color>();
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (m_MineManager == null)
        {
            m_MineManager = FindFirstObjectByType<MineManager>();
            if (m_MineManager == null)
            {
                Debug.LogError("MineDebugger: Could not find MineManager in scene!");
                enabled = false;
                return;
            }
        }

        if (m_GridManager == null)
        {
            m_GridManager = FindFirstObjectByType<GridManager>();
            if (m_GridManager == null)
            {
                Debug.LogError("MineDebugger: Could not find GridManager in scene!");
                enabled = false;
                return;
            }
        }

        // Cache all cell views
        var cellViews = FindObjectsOfType<CellView>();
        foreach (var cellView in cellViews)
        {
            m_CellViews.Add(cellView.GridPosition, cellView);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_DebugKey))
        {
            m_IsDebugMode = !m_IsDebugMode;
            ToggleDebugVisuals();
        }

        // Add cell inspection on key press
        if (Input.GetKeyDown(m_InspectKey))
        {
            InspectCellUnderMouse();
        }

        // Add reveal all cells on key press
        if (Input.GetKeyDown(m_RevealAllKey))
        {
            RevealAllCells();
        }
    }
    #endregion

    #region Private Methods
    private void ToggleDebugVisuals()
    {
        if (m_MineManager == null || m_GridManager == null)
        {
            Debug.LogError("Required references not set in MineDebugger!");
            return;
        }

        // Calculate values for all cells if debug mode is enabled
        if (m_IsDebugMode)
        {
            CalculateAllCellValues();
            // Show the values
            foreach (var kvp in m_CachedValues)
            {
                if (m_CellViews.TryGetValue(kvp.Key, out var cellView))
                {
                    var (value, color) = MineValueModifier.ModifyValueAndGetColor(kvp.Key, kvp.Value);
                    cellView.SetValue(value, color);
                }
            }
        }
        else
        {
            // Hide all values
            foreach (var cellView in m_CellViews.Values)
            {
                cellView.SetValue(0);
            }
        }
    }

    private void CalculateAllCellValues()
    {
        m_CachedValues.Clear();

        // Initialize all positions with 0
        for (int x = 0; x < m_GridManager.Width; x++)
        {
            for (int y = 0; y < m_GridManager.Height; y++)
            {
                var pos = new Vector2Int(x, y);
                m_CachedValues[pos] = 0;
            }
        }

        // For each mine, add its value to affected cells
        var mines = m_MineManager.GetMines();
        foreach (var kvp in mines)
        {
            var position = kvp.Key;
            var mineData = m_MineManager.GetMineDataAt(position);
            if (mineData != null)
            {
                var affectedPositions = mineData.GetAffectedPositions(position);
                foreach (var pos in affectedPositions)
                {
                    if (m_GridManager.IsValidPosition(pos) && !m_MineManager.HasMineAt(pos))
                    {
                        if (!m_CachedValues.ContainsKey(pos))
                        {
                            m_CachedValues[pos] = 0;
                        }
                        m_CachedValues[pos] += mineData.Value;
                    }
                }
            }
        }
    }

    private void InspectCellUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            var cellView = hit.collider.GetComponent<CellView>();
            if (cellView != null)
            {
                var pos = cellView.GridPosition;
                string valueInfo = m_IsDebugMode && m_CachedValues.ContainsKey(pos) 
                    ? $"Value: {m_CachedValues[pos]}, " 
                    : "";

                Debug.Log($"Cell at position {pos}:\n" +
                         $"{valueInfo}" +
                         $"IsRevealed: {cellView.GetType().GetField("m_IsRevealed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cellView)}\n" +
                         $"HasMine: {cellView.GetType().GetField("m_HasMine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cellView)}\n" +
                         $"BackgroundRenderer enabled: {cellView.BackgroundRenderer?.enabled}\n" +
                         $"MineRenderer enabled: {cellView.MineRenderer?.enabled}");
            }
        }
    }

    private void RevealAllCells()
    {
        if (m_GridManager == null || m_MineManager == null)
        {
            Debug.LogError("Required references not set in MineDebugger!");
            return;
        }

        foreach (var cellView in m_CellViews.Values)
        {
            if (cellView != null)
            {
                // Get mine data if there is a mine at this position
                var position = cellView.GridPosition;
                var mineData = m_MineManager.GetMineDataAt(position);
                var mine = m_MineManager.GetMines().TryGetValue(position, out IMine value) ? value : null;

                // Update visuals directly without triggering events
                if (mine != null && mineData != null)
                {
                    cellView.ShowMineSprite(mineData.MineSprite, mine, mineData);
                }
                cellView.UpdateVisuals(true);
            }
        }
    }
    #endregion
}
#endif
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public class MineDebugger : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private MineManager m_MineManager;
    [SerializeField] private KeyCode m_DebugKey = KeyCode.D;
    [SerializeField] private KeyCode m_InspectKey = KeyCode.I;  // New key for cell inspection
    [SerializeField] private Color m_HighlightColor = Color.red;
    #endregion

    #region Private Fields
    private Dictionary<Vector2Int, CellView> m_CellViews = new Dictionary<Vector2Int, CellView>();
    private bool m_IsDebugMode = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (m_MineManager == null)
        {
            m_MineManager = FindObjectOfType<MineManager>();
            if (m_MineManager == null)
            {
                Debug.LogError("MineDebugger: Could not find MineManager in scene!");
                enabled = false;
                return;
            }
        }

        // Cache all cell views
        var cellViews = FindObjectsOfType<CellView>();
        //Debug.Log($"MineDebugger: Found {cellViews.Length} cell views");
        
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
            //Debug.Log($"MineDebugger: Debug mode {(m_IsDebugMode ? "enabled" : "disabled")}");
            ToggleDebugVisuals();
        }

        // Add cell inspection on key press
        if (Input.GetKeyDown(m_InspectKey))
        {
            InspectCellUnderMouse();
        }
    }
    #endregion

    #region Private Methods
    private void ToggleDebugVisuals()
    {
        if (m_MineManager == null)
        {
            Debug.LogError("MineManager reference not set in MineDebugger!");
            return;
        }

        var mines = m_MineManager.GetMines();
        Debug.Log($"MineDebugger: Found {mines.Count} mines");
        
        foreach (var kvp in m_CellViews)
        {
            var cellView = kvp.Value;
            if (m_IsDebugMode && mines.ContainsKey(kvp.Key))
            {
                //Debug.Log($"MineDebugger: Highlighting mine at {kvp.Key}");
                cellView.ShowDebugHighlight(m_HighlightColor);
            }
            else
            {
                cellView.HideDebugHighlight();
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
                Debug.Log($"Cell at position {cellView.GridPosition}:\n" +
                         $"IsRevealed: {cellView.GetType().GetField("m_IsRevealed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cellView)}\n" +
                         $"HasMine: {cellView.GetType().GetField("m_HasMine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cellView)}\n" +
                         $"CurrentState: {cellView.GetType().GetField("m_CurrentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cellView)}\n" +
                         $"BackgroundRenderer enabled: {cellView.BackgroundRenderer?.enabled}\n" +
                         $"MineRenderer enabled: {cellView.MineRenderer?.enabled}");
            }
        }
    }
    #endregion
}
#endif
using UnityEngine;

namespace RPGMinesweeper.Input
{
    public class InteractionHandler : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private GridManager m_GridManager;
        [SerializeField] private bool m_DebugMode;
        [SerializeField] private MarkPanel m_MarkPanel;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_DebugMode) Debug.Log("[InteractionHandler] Initializing...");
            
            if (m_GridManager == null)
            {
                m_GridManager = FindFirstObjectByType<GridManager>();
                if (m_GridManager == null)
                {
                    Debug.LogError("[InteractionHandler] Could not find GridManager!");
                    enabled = false;
                    return;
                }
                if (m_DebugMode) Debug.Log("[InteractionHandler] Found GridManager");
            }
            
            if (m_MarkPanel == null)
            {
                m_MarkPanel = FindFirstObjectByType<MarkPanel>();
                if (m_MarkPanel == null)
                {
                    Debug.LogWarning("[InteractionHandler] Could not find MarkPanel. Mark feature will be disabled.");
                }
                else if (m_DebugMode)
                {
                    Debug.Log("[InteractionHandler] Found MarkPanel");
                }
            }
            
            InputManager.OnCellClicked += HandleCellClicked;
            InputManager.OnCellRightClicked += HandleCellRightClicked;
            if (m_DebugMode) Debug.Log("[InteractionHandler] Subscribed to input events");
        }

        private void OnDestroy()
        {
            if (m_DebugMode) Debug.Log("[InteractionHandler] Unsubscribing from events");
            InputManager.OnCellClicked -= HandleCellClicked;
            InputManager.OnCellRightClicked -= HandleCellRightClicked;
        }
        #endregion

        #region Private Methods
        private void HandleCellClicked(Vector2Int position)
        {
            if (m_DebugMode) Debug.Log($"[InteractionHandler] Handling left click at position {position}");
            
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject != null)
            {
                if (m_DebugMode) Debug.Log($"[InteractionHandler] Found cell object: {cellObject.name}");
                var interactable = cellObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (m_DebugMode) Debug.Log($"[InteractionHandler] Found IInteractable, CanInteract: {interactable.CanInteract}");
                    interactable.OnInteract();
                }
                else if (m_DebugMode)
                {
                    Debug.LogError($"[InteractionHandler] Cell object {cellObject.name} does not implement IInteractable!");
                }
            }
            else if (m_DebugMode)
            {
                Debug.LogError($"[InteractionHandler] No cell object found at position {position}");
            }
        }
        
        private void HandleCellRightClicked(Vector2Int position)
        {
            if (m_DebugMode) Debug.Log($"[InteractionHandler] Handling right click at position {position}");
            
            if (m_MarkPanel == null)
            {
                if (m_DebugMode) Debug.LogWarning("[InteractionHandler] Cannot show mark panel - no MarkPanel reference");
                return;
            }
            
            var cellObject = m_GridManager.GetCellObject(position);
            if (cellObject != null)
            {
                if (m_DebugMode) Debug.Log($"[InteractionHandler] Found cell object: {cellObject.name}");
                var cellView = cellObject.GetComponent<CellView>();
                if (cellView != null)
                {
                    if (!cellView.IsRevealed)
                    {
                        if (m_DebugMode) Debug.Log($"[InteractionHandler] Showing mark panel for cell at {position}");
                        m_MarkPanel.ShowAtCell(cellView);
                    }
                    else if (m_DebugMode)
                    {
                        Debug.Log($"[InteractionHandler] Cell at {position} is already revealed - ignoring right click");
                    }
                }
                else if (m_DebugMode)
                {
                    Debug.LogError($"[InteractionHandler] Cell object {cellObject.name} does not have CellView component!");
                }
            }
            else if (m_DebugMode)
            {
                Debug.LogError($"[InteractionHandler] No cell object found at position {position}");
            }
        }
        #endregion
    }
} 
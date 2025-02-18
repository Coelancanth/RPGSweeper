using UnityEngine;

namespace RPGMinesweeper.Input
{
    public class InteractionHandler : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private GridManager m_GridManager;
        [SerializeField] private bool m_DebugMode;
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
            
            InputManager.OnCellClicked += HandleCellClicked;
            if (m_DebugMode) Debug.Log("[InteractionHandler] Subscribed to OnCellClicked event");
        }

        private void OnDestroy()
        {
            if (m_DebugMode) Debug.Log("[InteractionHandler] Unsubscribing from events");
            InputManager.OnCellClicked -= HandleCellClicked;
        }
        #endregion

        #region Private Methods
        private void HandleCellClicked(Vector2Int position)
        {
            if (m_DebugMode) Debug.Log($"[InteractionHandler] Handling click at position {position}");
            
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
        #endregion
    }
} 